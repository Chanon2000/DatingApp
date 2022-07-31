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
        public MessageHub(IMessageRepository messageRepository, IMapper mapper, 
            IUserRepository userRepository)
        {
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
            // Context.ConnectionId คือ เอา ConnectionId จาก context มา

            var messages = await _messageRepository.
                GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages); // Clients.Group(groupName) ส่ง เฉพาะใน groupName เท่านั้น
            // สังเกตว่าแบบนี้จะยังไม่ optimization เพราะว่า message เก่าที่ user มี อยู่แล้วคุณก็ยังส่งไปอยู่ โดยไม่จำเป็น
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
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

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) 
            {
                var group = GetGroupName(sender.UserName, recipient.UserName);
                await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0; // ใช้ alphabetical order ในการเทียบ
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}