using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Group
    {
        public Group() // เนื่องจากตอนสร้าง table มันจะต้องการ empty constructor
        {
        }

        public Group(string name)
        {
            Name = name;
        }

        [Key]// ซึ่งให้ Name เป็น primary key
        public string Name { get; set; } 
        public ICollection<Connection> Connections { get; set; } = new List<Connection>(); // new เลยเพราะว่าเมื่อเราสร้าง Group ใหม่ เราแน่นอนต้องการ Connections list ใน group นั้น
    }
}