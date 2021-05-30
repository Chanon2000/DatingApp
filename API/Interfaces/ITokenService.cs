using API.Entities;

namespace API.Interfaces
{
// 12. Adding a token service
    // 12.1 สร้าง interface ของ TokenService
        // interface คือ ตัวในการสื่อสารระหว่างมันกับ class อื่น ด้วยการใน class อื่น implements
        // ซึ่งมันจะเก็บแค่ signatures ของ class ที่มันเป็น interface ให้ แต่ไม่เก็บ logic
        // เติม I ที่ชื่อ เพื่อบอกว่ามันเป็น interface
    public interface ITokenService
    {
        string CreateToken(AppUser user); // return แค่ string
    }
}