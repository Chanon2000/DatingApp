using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FallbackController : Controller // Controller เป็น class สำหรับ mvc controller ที่มีการ support view (ซึ่งครั้งนี้เราจะให้ angular เป็น view)
    {
        // นั้นคือเราจะทำการส่ง route ต่างๆไปที่ index.html เพราะ route พวกนี้จะมีแค่ angular เท่านั้นที่รู้จัก routes พวกนี้ ( routes ที่เราขึ้นต้นด้วย /api )
        public ActionResult Index()
        {
            // return index.html เพื่อให้ index จัดการ route ต่อ
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
                "wwwroot", "index.html"), "text/HTML");
                // กำหนด folder => wwwroot
                // กำหนด file => index.html
                // กำหนด file type => text/HTML
        }
    }
}