using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CreateMessageDto
    {
        public string RecipentUsername { get; set; }
        public string Content { get; set; }
    }
}