using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<PresenceHub> _presenceHub; // เราสามารถใช้ hub อื่น ในที่ใหนก็ได้ โดยการ inject มันเข้ามาแล้วหุ้มด้วย IHubContext
        private readonly PresenceTracker _tracker;
        public MessageHub(IMessageRepository messageRepository, IMapper mapper, 
            IUserRepository userRepository, IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            _tracker = tracker;
            _presenceHub = presenceHub;
            _userRepository = userRepository;
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync() // สร้าง hub ใหม่ขึ้นมาเลยสำหรับเรื่อง message
        {
            // สร้าง group ให้แต่ละ user
            // เราจะตั้ง group name โดยการรวมกันระหว่าง username ทั้ง 2 คน
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);  // เพิ่ม user เข้า group
            var group = await AddToGroup(groupName);
            // Context.ConnectionId คือ เอา ConnectionId จาก context มา
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _messageRepository.
                GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages); // Clients.Group(groupName) ส่ง เฉพาะใน groupName เท่านั้น
            // Caller หรือก็คือคนที่เข้ามาในห้องแชด
            // ตัวอย่างการ optimization เช่น เมื่อ Todd เข้ามา มันจะส่ง UpdatedGroup ไปให้ทั้งคู่แต่หลักๆคือเพื่อทำการ mark message ของ lisa ว่า todd อ่านแล้วที่ clientของ lisaเลย จากนั้นก็ทำการส่ง messages ทั้งหมด เฉพาะ Todd (Caller) เท่านั้น 
            // (optimization ครั้งนี้ทำให้คุณไม่ต้องส่ง message ทั้งหมด ให้ lisa เมื่อ Todd เข้ามาอ่าน message ไงละ)
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception); // ลบ ออกจาก group นั้นๆ ที่ user คนนั้นกำลังอยู่โดยที่ไม่ต้องบอกมันว่า group อะไร
        }

        // ส่ง message กันใน hub (ซึ่งคุณไปเรียกใช้มันที่ service ของ angular นั้นเอง ด้วยคำสั่ง invoke)
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You cannot send messages to yourself");
                // return BadRequest("You cannot send messages to yourself");
                // ใน hub เราจะไม่สามารถเข้าถึง API responses (HTTP response) ได้ เช่น BadRequest(), NotFound()
            
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) throw new HubException("Not found user");
            // return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _messageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow; // UtcNow => time on this computer
            }
            else
            {
                var connections = await _tracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null) // ทำเมื่อ user online แต่ไม่ได้ connect ใน group นั้นๆ
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                        new {username = sender.UserName, knownAs = sender.KnownAs });
                    // new{} คือ anonymous object ขึ้นมา
                    // .Clients(connections) คือเราจะทำการ invoke method NewMessageReceived ไปที่ connections ที่ระบุใน Clients()
                }
            }

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) 
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        private async Task<Group> AddToGroup(string groupName)
        // HubCallerContext context ไม่จำเป็นเพราะเราอยู่ใน Hub file (แต่ถ้าไม่ได้อยู่ใน hub ถึงจะควรใส่)
        // HubCallerContext ซึ่งจะทำให้เราเข้าถึง current username และ connectionId
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await _messageRepository.SaveAllAsync()) return group; // SaveAllAsync return boolean

            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            // อยู่ใน hub เอา ConnectionId จะ Context ได้เลย
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to remove from group");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0; // ใช้ alphabetical order ในการเทียบ
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}