using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.SignalR;
// เริ่มจากสร้าง folder เอง แล้วก็สร้าง class นี้เองเลย
// SignalR มาพร้อม project ตอน new อยู่แล้ว
namespace API.SignalR
{
    public class PresenceHub : Hub // คือ Hub class จาก SignalR
    // คลิกที่ Hub เข้าไปดูใน meta คุณจะเห็น method ที่คุณสามารถเขียน override ได้
    {
        public override async Task OnConnectedAsync()
        {
            
            await Clients.Others.SendAsync("userIsOnline", Context.User.GetUsername());
            // ใน Hub เราสามารถเข้าถึงตัวแปร Clients ได้
            // Clients.Others คือทุกคนยกเว้นคนที่ connection ที่ triggered การทำงานครั้งนี้
            // "userIsOnline" จะเป็นชื่อ method ที่เราจะใช้ที่ client แล้วก็ส่ง Username ไปด้วย
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        // เนื่องจาก method ยี้ req 1 parameter นั้นก็คือ exception
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            await base.OnDisconnectedAsync(exception); // ถ้ามันเกิด ex ก็ ส่งมันไปที่ base (หรือ parent class **)
        }
    }
}