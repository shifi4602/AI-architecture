using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using Services;

namespace Enteties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        IUsersService _iUsersServicies;
        IpasswordServices _iPasswordsServices;
        
        public UsersController(IUsersService usersServicies, IpasswordServices passwordServices)
        {
            _iPasswordsServices = passwordServices;
            _iUsersServicies = usersServicies;
        }
        
        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return Ok("value");
        }
        
        // POST api/<UsersController>
        [HttpPost]

        public async Task<ActionResult<User>> Post([FromBody] User value)
        {
            User user = await _iUsersServicies.AddNewUser(value);
            if (user == null)
                return BadRequest("Password is too weak");
            return CreatedAtAction(nameof(Get), new { user.Id }, user);
        }
        
        [HttpPost("login")]
        public  async Task<ActionResult<User>> login([FromBody] UpdateUser value)
        {
            User user = await _iUsersServicies.Login(value);
            if (user != null)
            {
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
            }
            return Unauthorized();
        }
        
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User userToUpdate)
        {
            bool passwordsStrenght = await _iUsersServicies.UpdateUser(id, userToUpdate);
            if (passwordsStrenght)
            {
                return Ok(userToUpdate);
            }
            return NoContent();
        
        }
    }
}
