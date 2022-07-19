using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // เพื่อเปลี่ยนตัวอักษรเป็นแบบ CamelCase
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            // อาจตั้งชื่อว่า X-Pagination แต่เราไม่ได้มี requried ให้ตั้งชื่อแบบนั้น
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination"); // add อีก 1 header เป็น core header เพื่อทำให้ Pagination header available ได้
            // Access-Control-Expose-Headers ต้องใส่ชื่อตามนี้ให้ถูกต้อง
        }
    }
}