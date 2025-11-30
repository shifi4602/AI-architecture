using System.ComponentModel.DataAnnotations;

namespace Enteties
{
    public class Users
    {
        public int Id { get; set; }
        [EmailAddress, Required]
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
