using Microsoft.AspNetCore.Mvc;
using NoventiqApplication.Interface;

namespace NoventiqApplication.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILocalizationService _localizationService;

        public RoleController(IRoleService roleService, ILocalizationService localizationService)
        {
            _roleService = roleService;
            _localizationService = localizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                // Use ILocalizationService to get the localized message
                var message = _localizationService.GetLocalizedString("WelcomeMessage");

                // Example roles from service
                var roles = await _roleService.GetAllRolesAsync();

                return Ok(new
                {
                    Message = message,
                    Roles = roles
                });
            }
            catch (Exception ex)
            {
                var errorMessage = _localizationService.GetLocalizedString("ErrorMessage1");
                return StatusCode(500, new { Message = errorMessage, Details = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null) return NotFound("Role not found.");
                return Ok(role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the role. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleDTO role)
        {
            try
            {
                await _roleService.AddRoleAsync(role);
                return Ok("Role added successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding the role. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleDTO role)
        {
            try
            {
                var existingRole = await _roleService.GetRoleByIdAsync(id);
                if (existingRole == null) return NotFound("Role not found.");

                role.Id = id;
                await _roleService.UpdateRoleAsync(role);
                return Ok("Role updated successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the role. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var existingRole = await _roleService.GetRoleByIdAsync(id);
                if (existingRole == null) return NotFound("Role not found.");

                await _roleService.DeleteRoleAsync(id);
                return Ok("Role deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the role. Please try again later.");
            }
        }

    }

}
