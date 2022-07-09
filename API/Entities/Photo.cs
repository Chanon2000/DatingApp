using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
  [Table("Photos")] // แต่เราต้องการให้เรียก Photos ใน database อยู่ เลยใส่ Attribute นี้
  // 
  public class Photo
  {
    public int Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    public string PublicId { get; set; }
    // fully defining the relationship. เพื่อบอกสิ่งต่างๆเกี่ยวกับ AppUser class (ที่สัมพันกับ Photos)
    public AppUser AppUser { get; set; } // มันจะทำให้ใน Photo ก็ return AppUser ออกมาด้วย แล้วใน AppUser ก็มี Photo อีกทำให้มันวนลูป ซึ่งทำให้ controller จะ return 500 ไปเลย
    public int AppUserId { get; set; }
  }
}