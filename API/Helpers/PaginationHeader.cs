using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationHeader // เก็บ info ที่เราต้องการจะส่งไปที่ client ผ่าน header
    { // info จะเหมือนกับที่ PagedList แต่เราใช้อันนั้นทำอย่างอื่นไปแล้วเลยมาสร้างใหม่ตรงนี้่
        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}