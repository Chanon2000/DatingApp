namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; } 
        public string UserName { get; set; }
        // ใช้ public เพราะว่า เราต้องการให้ entity framework ทำการ get, set property นี้ด้วย ดังนั้นเราจึงใช้ public

    // #6.1
        public byte[] PasswordHash { get; set; } // จะถูก return เมื่อเราคำนวณ hash แล้ว
        public byte[] PasswordSalt { get; set; }
    }
}