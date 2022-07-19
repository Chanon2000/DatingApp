using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    // ทำให้เป็น generic โดยการใส่ <T> คือหมายถึงเป็น Type อะไรก็ได้ของ entity
    public class PagedList<T> : List<T> // ให้ PagedList เป็น List เพื่อดึงความสามารถทุกอย่างจาก list มา
    {
        // คลิก Generate constructor... ที่ PagedList
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        // items เป็นอะไรก็ได้ เลยใส่เป็น IEnumerable ไปก่อน
        {
            CurrentPage = pageNumber;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        // สร้าง static method เพื่อที่จะสามารถใช้ได้ทุกที่
        // method นี้จะรับ query มา เลยใส่ IQueryable
        // IQueryable<T> T อาจเป็น User หรืออะไรก็ได้
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync(); // คือจำนวนทุก record ใน table ที่ ทำการ query นะ
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); // ToListAsync() ทำการ run query ด้วยคำสั่งนี้
            // CountAsync(), ToListAsync() จะทำการสร้าง database call
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}