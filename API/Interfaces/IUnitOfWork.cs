using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; } // ใส่แค่ get method
        IMessageRepository MessageRepository { get; }
        ILikesRepository LikesRepository { get; }
        Task<bool> Complete(); // จะเป็น method ที่ saveChange เราทั้งหมด
        bool HasChanges(); // เพื่อ change ว่า entity framework มีการ track changes ซักอันมั้ย
    }
}