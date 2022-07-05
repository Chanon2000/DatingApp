using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class ApiException // คลิดที่เหลืองๆ แล้วก็เลือก Generate constructor... ก็จะได้ constructor มา
    {
        public ApiException(int statusCode, string message = null, string details = null) // ถ้าไม่ได้ใส่ค่าให้ใน message, details มันก็จะใส่เป็น null เป็นค่าเริ่มต้น
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}