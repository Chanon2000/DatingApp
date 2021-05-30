namespace API.Entities
{
    public class AppUser
    {
        // keyword สร้าง property คือ "prop"
        // public + type + name property
        // เป็นแบบสั้น
        public int Id { get; set; } 
        public string UserName { get; set; }

    // เขียน property แบบเต็ม (keywork ในการสร้างคือ "propfull")
        // เป็นแบบเต็ม
        // private int myVar; 
        // public int MyProperty
        // {
        //     get { return myVar; }
        //     set { myVar = value; }
        // }

        // public เหมือนถึง property นี้สามารถ get ที่ class ไหนก็ได้
        // protected คือ property นี้สามารถเข้าถึงได้จาก class นี้ และจากclass อื่นที่inherit class นี้
        // private คือ สามารถ เข้าถึงแก้ไข จากclassนี้เท่านั้น
        // ใช้ public เพราะว่า เราต้องการให้ entity framework ทำการ get, set property นี้ด้วย ดังนั้นเราจึงใช้ public

    // #6.1
        public byte[] PasswordHash { get; set; } // จะถูก return เมื่อเราคำนวณ hash แล้ว
        public byte[] PasswordSalt { get; set; }
    }
}