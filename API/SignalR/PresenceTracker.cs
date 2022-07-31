using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker // PresenceTracker เป็นเหมือน service ที่เราสร้างเพื่อ shared ในทุก connection ที่เข้ามาใน server
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = 
            new Dictionary<string, List<string>>();
            // key, value
            // kay => จะเป็น Username
            // value => List<string> เราจะเก็บ list ของ connection ID string ตรงนี้
            // ทุกครั้งที่มีการ connection เขาจะให้ connection ID มาด้วย

        
        public Task<bool> UserConnected(string username, string connectionId)
        {
            // เนื่องจากdictionary ไม่ใช่ที่เก็บข้อมูลที่ดี ดังนั้น
            // ถ้าเรามี หลาย user ต้องการจะเข้ามา update ข้อมูล พร้อมกันนั้นจะทำให้มันเกิด error ขึ้น ดังนั้นจึงต้องทำการ lock ด้วย
            bool isOnline = false; // ค่าเริ่มต้น
            lock (OnlineUsers) // lock ตัวแปรนี้ จนกว่าจะทำใน {} เสร็จ
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string>{connectionId}); // new key ใหม่พร้อมกับ connectionId เลย
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOffline = false;
            lock(OnlineUsers)
            {
                // ถ้าไม่มี key ใน dictionary แสดงว่า offline อยู่
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                OnlineUsers[username].Remove(connectionId);
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock(OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username); // จะ return null ถ้าไม่มี username ใน connection
            }

            return Task.FromResult(connectionIds);
        }
    }
}