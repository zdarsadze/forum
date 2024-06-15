using GITA.Data;
using GITA.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GITA.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("ban")]
        public async Task<IActionResult> BanUser([FromBody] BanUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            user.IsBanned = model.IsBanned;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Failed to update user status.", Errors = result.Errors });
            }

            return Ok(new { Message = $"User {(model.IsBanned ? "banned" : "unbanned")} successfully." });
        }
    }
}
