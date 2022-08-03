using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MessagesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Method นี้ เหมือนคนสอนจะลบไปนานแล้ว
        // [HttpPost]
        // public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        // {
        //     var username = User.GetUsername();

        //     if (username == createMessageDto.RecipientUsername.ToLower())
        //         return BadRequest("You cannot send messages to yourself");
            
        //     var sender = await _userRepository.GetUserByUsernameAsync(username);
        //     var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        //     if (recipient == null) return NotFound();

        //     var message = new Message
        //     {
        //         Sender = sender,
        //         Recipient = recipient,
        //         SenderUsername = sender.UserName,
        //         RecipientUsername = recipient.UserName,
        //         Content = createMessageDto.Content
        //     };

        //     _unitOfWork.MessageRepository.AddMessage(message);

        //     if (await _unitOfWork.MessageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

        //     return BadRequest("Failed to send message");
        // } 

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, 
                messages.TotalCount, messages.TotalPages);

            return messages;
        }

        // [HttpGet("thread/{username}")] // เอาออก เพราะเราไม่ใช้แล้ว เราใช้ hub ทำหน้าที่นี้แทน
        // public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        // {
        //     var currentUsername = User.GetUsername();

        //     return Ok(await _unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
        // }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();

            if (message.Sender.UserName == username) message.SenderDeleted = true;

            if (message.Recipient.UserName == username)  message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted) 
                _unitOfWork.MessageRepository.DeleteMessage(message);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}