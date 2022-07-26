using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            // เนื่องจากเราใช้ include ใน FindAsync ไม่ได้ เลยใช้ SingleOrDefaultAsync
            return await _context.Messages
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username 
                    && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username
                    && u.SenderDeleted == false),
                _ => query.Where(u => u.Recipient.UserName == 
                    messageParams.Username && u.RecipientDeleted == false && u.DateRead == null) // default message ก็คือ message ที่ยังไม่ได้อ่าน
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider); // ProjectTo สามารถใช้ตอนที่มันเป็น query (IQueryable) ได้ แต่ _mapper.Map ใช้ได้แค่ตอนที่มี data แล้ว

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, 
            string recipientUsername)
        {
            // ทำการ get message conversation ระหว่าง 2 users
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos) // ไปที่ Sender แล้วเอา Photos ของ Sender มาด้วย
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false
                        && m.Sender.UserName == recipientUsername
                        || m.Recipient.UserName == recipientUsername
                        && m.Sender.UserName == currentUsername && m.SenderDeleted == false
                )
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

                // เราจะทำการ mark message ให้เป็นสีแดง ทุก message ที่ user get จาก message thread ทุก message จะถูก mark ว่าอ่านแล้ว

            // หา message ที่ current user ยังไม่ได้อ่าน
            var unreadMessages = messages.Where(m => m.DateRead == null 
                && m.Recipient.UserName == currentUsername).ToList();

            // mark message ให้เป็นสีแดง
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }
            
            // แล้ว return ส่งไปให้ user
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}