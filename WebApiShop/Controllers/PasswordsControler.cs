using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using Services;

namespace Enteties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PasswordsController : ControllerBase
    {
        private readonly passwordServices _passwordService = new passwordServices();

        // POST api/<PasswordsController>
        [HttpPost("{pass}")]
        public ActionResult<PassEntity> CheckPasswordStrength([FromBody] string pass)
        {
            PassEntity password = _passwordService.GetStrength(pass);

    public class PasswordsController : ControllerBase, IPasswordsController
    {
        IpasswordServices _iPasswordService;
        public PasswordsController(IpasswordServices iPasswordController)
        {
            _iPasswordService = iPasswordController;
        }
        // GET: api/<PasswordController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<PasswordController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
