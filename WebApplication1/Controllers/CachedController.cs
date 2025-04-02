using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Filters;

namespace WebApplication1.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CachedController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Message = "This is a cached response!", Timestamp = DateTime.UtcNow });
        }

        [ServiceFilter(typeof(ResponseCacheFilter))]
        [HttpGet("appointments")]
        public IActionResult GetAppointments()
        {
            return Ok(new { Data = "Cached appointments list" });
        }
    }
}
