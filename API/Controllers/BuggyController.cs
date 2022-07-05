using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class BuggyController : BaseApiController
  {
    private readonly DataContext _context;

    public BuggyController(DataContext context)
    {
      _context = context;
    }

    [Authorize] // คือต้องการให้ user authenticated ก่อนยิงเข้า เส้นนี้
    [HttpGet("auth")]
    public ActionResult<string> GetSecret() // ทำเส้นนี้ขึ้นมาเพื่อทดสอบ 401 unauthorized responses
    { 
        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<string> GetNotFound()
    { 
            var thing = _context.Users.Find(-1); // หา member ที่มี ID = -1 (ซึ่งเราตั้งใจให้มันหาไม่เจอ)

            if (thing == null) return NotFound();

            return Ok(thing);
    }

    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    { 
        var thing = _context.Users.Find(-1);

        var thingToReturn = thing.ToString();

        return thingToReturn;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    { 
        return BadRequest("This was not a good request");
    }
  }
}