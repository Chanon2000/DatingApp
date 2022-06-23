using API.Entities;

namespace API.Interfaces
{
// #12. Adding a token service
    // 12.1 สร้าง interface ของ TokenService
    public interface ITokenService
    {
        string CreateToken(AppUser user); // return แค่ string
    }
}