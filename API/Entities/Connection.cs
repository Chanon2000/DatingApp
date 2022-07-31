namespace API.Entities
{
    public class Connection
    {
        // สร้าง default constructor เอาไว้ แบบ no parameters
        public Connection()
        {
        }

        public Connection(string connectionId, string username) // การสร้าง construtor มาทำแบบนี้ ทำให้เราสามารถ new แล้วใส่ตัวแปรลง {} ได้ง่ายๆ ง่ายกว่าใส่ค่าลงตัวแปรที่ละตัว
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string ConnectionId { get; set; } // ถ้าคุณใส่ ชื่อ class ตามด้วยคำว่า Id จะทำให้ Entity Framework คิดว่านี้คือ primary key ทันที
        public string Username { get; set; }
    }
}