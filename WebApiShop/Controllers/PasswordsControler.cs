using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using Services;

namespace Enteties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PasswordsController : ControllerBase
    {
        IpasswordServices _iPasswordService;
        public PasswordsController(IpasswordServices iPasswordController)
        {
            _iPasswordService = iPasswordController;
        }
   
        [HttpGet]
        public void Get(string pass)
        {
        
        }
        
        // POST api/<PasswordController>
        [HttpPost("CheckPasswordStrength")]
        public ActionResult<PassEntity> CheckPasswordStrength([FromBody] string pass)
        {
            PassEntity password = _iPasswordService.GetStrength(pass);
        
            if (password == null)
                return BadRequest("Invalid password");
            return Ok(password);
        }
    }        
}
