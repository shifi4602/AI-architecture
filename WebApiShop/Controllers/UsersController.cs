using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using Services;
using DTO_s;

namespace Enteties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        IUsersService _iUsersServicies;
        IpasswordServices _iPasswordsServices;
        private readonly ILogger<UsersController> _logger;
        
        public UsersController(IUsersService usersServicies, IpasswordServices passwordServices, ILogger<UsersController> logger)
        {
            _iPasswordsServices = passwordServices;
            _iUsersServicies = usersServicies;
            _logger = logger;
        }
        
        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return Ok("value");
        }
        
        // POST api/<UsersController>
        [HttpPost]

        public async Task<ActionResult<UserDTO>> Post([FromBody] UserDTO value)
        {
            UserDTO user = await _iUsersServicies.AddNewUser(value);
            if (user == null)
                return BadRequest("Password is too weak");
            return CreatedAtAction(nameof(Get), new { user.id }, user);
        }
        
        [HttpPost("login")]
        public  async Task<ActionResult<User>> login([FromBody] ExisitingUser value)
        {
            User user = await _iUsersServicies.Login(value);
            if (user != null)
            {
                _logger.LogInformation("Login attempted with User Email: " + value.Email  + "and password: " + value.Password);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
            }
            _logger.LogError("Error in getting user");
            return Unauthorized();
        }
        
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userToUpdate, string password)
        {
            bool passwordsStrenght = await _iUsersServicies.UpdateUser(id, userToUpdate, password);
            if (passwordsStrenght)
            {
                return Ok(userToUpdate);
            }
            return NoContent();        
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetById(int id)
        {
            UserDTO user = await _iUsersServicies.GetById(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

    }
}
