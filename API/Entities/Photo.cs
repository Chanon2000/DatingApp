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
  }
}