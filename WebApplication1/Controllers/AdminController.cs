using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Filters;

namespace WebApplication1.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        [RoleAuthorize("Admin")]
        public IActionResult GetAdminDashboard()
        {
            return Ok(new { Message = "Welcome, Admin!" });
        }

        [HttpGet("profile")]
        [RoleAuthorize("User")] // 🔹 Another role example
        public IActionResult GetProfile()
        {
            return Ok(new { Message = "User profile data" });
        }
    }
}
