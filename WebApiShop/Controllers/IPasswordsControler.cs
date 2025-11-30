using Microsoft.AspNetCore.Mvc;

namespace Enteties.Controllers
{
    public interface IPasswordsControler
    {
        ActionResult<PassEntity> CheckPasswordStrength([FromBody] string pass);
        void Delete(int id);
        IEnumerable<string> Get();
        string Get(int id);
        void Get(string pass);
        void Put(int id, [FromBody] string value);
    }
}