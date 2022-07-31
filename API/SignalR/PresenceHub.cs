using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
// เริ่มจากสร้าง folder เอง แล้วก็สร้าง class นี้เองเลย
// SignalR มาพร้อม project ตอน new อยู่แล้ว
namespace API.SignalR
{
    // เราจะจัดการ signalR (หรือ web sockets) แตกต่างจาก api controller ตรงที่ว่า web sockets ไม่ส่ง authentication header
    // ดังนั้นสิ่งที่เราจะใช้คือ query string ของ signal
    [Authorize] // ทำให้ anonymous users เข้ามาไม่ได้
    public class PresenceHub : Hub // คือ Hub class จาก SignalR
    // คลิกที่ Hub เข้าไปดูใน meta คุณจะเห็น method ที่คุณสามารถเขียน override ได้
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            // ใน Hub เราสามารถเข้าถึงตัวแปร Clients ได้
            // Clients.Others คือทุกคนยกเว้นคนที่ connection ที่ triggered การทำงานครั้งนี้
            // "userIsOnline" จะเป็นชื่อ method ที่เราจะใช้ที่ client แล้วก็ส่ง Username ไปด้วย

            // คุณจะเห็นว่าเราไม่มีทางจะรู้ได้เลยว่า มีใครกำลัง connect อยู่บ้าง
            // Radius สามารถ tracking เรื่องนี้ได้จาก database 

            // สิ่งที่เราจะทำคือเราจะเก็บคนที่ connected อยู่ ให้ dictionary เลย วิธีนี้ไม่ scalable นั้นหมายความว่ามันทำงานไม่ได้เมื่อมีหลายน server แต่ทำงานได้ใน single server แต่ถ้าคุณจะใช้ในหลายๆ server คุณต้องใช้ service เช่น Radius หรืออาจใช้ database ในการเก็บข้อมูลเรื่องนี้ได้

            var currentUsers = await _tracker.GetOnlineUsers();
            // เรา return list ของ online user ให้กับทุกคนและคุณเวลา เราควรจะส่งให้เฉพาะคนที่พึงเข้ามา connect เป็นต้น (เราเลยเปลี่ยนเป็นส่งให้เฉพาะ Caller แทน)
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers); // แล้วก็ return ให้กับ user ที่ online ทั้งหมด
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        // เนื่องจาก method ยี้ req 1 parameter นั้นก็คือ exception
        {
            var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            await base.OnDisconnectedAsync(exception); // ถ้ามันเกิด ex ก็ ส่งมันไปที่ base (หรือ parent class **)
        }
    }
}