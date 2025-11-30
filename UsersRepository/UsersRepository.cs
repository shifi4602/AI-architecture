using System.Text.Json;
using Enteties;

namespace Repositories
{
    public class UsersRepository : IUsersRepository
    {

        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "users.txt");
        
        public UsersRepository() { }
        string filePath = "M:\\WebApi\\WebApiShop\\WebApiShop\\users.txt";

        public Users AddUser(Users value)
        {
            int numberOfUsers = System.IO.File.ReadLines(_filePath).Count();
            value.Id = numberOfUsers + 1;
            string userJson = JsonSerializer.Serialize(value);
            System.IO.File.AppendAllText(_filePath, userJson + Environment.NewLine);
            return value;
        }

        public Users Login(UpdateUser value)
        {
            using (StreamReader reader = System.IO.File.OpenText(_filePath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    Users userFromFile = JsonSerializer.Deserialize<Users>(currentUserInFile);
                    if (userFromFile.Email == value.Email && userFromFile.Password == value.Password)
                        return userFromFile;
                }
            }
            return null;
        }

        public void UpdateUser(int id, Users userToUpdate)
        {
            string textToReplace = string.Empty;
            using (StreamReader reader = System.IO.File.OpenText(_filePath))
            {
                string currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {

                    Users user = JsonSerializer.Deserialize<Users>(currentUserInFile);
                    if (user.Id == id)
                        textToReplace = currentUserInFile;
                }
            }

            if (textToReplace != string.Empty)
            {
                string text = System.IO.File.ReadAllText(_filePath);
                text = text.Replace(textToReplace, JsonSerializer.Serialize(userToUpdate));
                System.IO.File.WriteAllText(_filePath, text);
            }
        }
    }
}
