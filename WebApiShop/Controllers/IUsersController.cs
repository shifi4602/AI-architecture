using Microsoft.AspNetCore.Mvc;

namespace Enteties.Controllers
{
    public interface IUsersController
    {
        void Delete(int id);
        IEnumerable<string> Get();
        string Get(int id);
        ActionResult<Users> login([FromBody] UpdateUser value);
        ActionResult<Users> Post([FromBody] Users value);
        IActionResult UpdateUser(int id, [FromBody] Users userToUpdate);
    }
}