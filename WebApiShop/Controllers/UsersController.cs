using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using Services;

namespace Enteties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase, IUsersController
    {        
        IpasswordServices _iPasswordsServices;
        
        public UsersController(IUsersService usersServicies, IpasswordServices passwordServices)
        {
            _iPasswordsServices = passwordServices;
            _usersServicies = usersServicies;
        }
        
        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return Ok("value");
        }
        
        // POST api/<UsersController>
        [HttpPost]
        public ActionResult<Users> Post([FromBody] Users user)
        {
            Users result = _usersServicies.AddNewUser(user);
            if (result == null)
                return BadRequest("Password is not strong enough");
            return CreatedAtAction(nameof(Get), new { result.Id }, result);
        }
        
        [HttpPost("login")]
        public ActionResult<Users> Login([FromBody] UpdateUser value)
        {
            Users user = _usersServicies.Login(value);
            if (user != null)
            {
                return Ok(user);
            }
            return Unauthorized();
        }
        
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        
        public IActionResult UpdateUser(int id, [FromBody] Users userToUpdate)
        {
            bool passwordsStrenght = _usersServicies.UpdateUser(id, userToUpdate);
            if (passwordsStrenght)
            {
                return Ok(userToUpdate);
            }
            return NoContent();
        
        }
        
        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            //_usersService.UpdateUser(id, userToUpdate);
            //return NoContent();
        }
    }
}
