using GITA.Data;
using GITA.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace GITA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Address,
                user.PhoneNumber
            });
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User information updated successfully!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User deleted successfully!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("find")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> FindUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Address,
                user.PhoneNumber
            });
        }
    }
}
