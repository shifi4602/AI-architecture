using Enteties;

namespace Services
{
    public interface IpasswordServices
    {
        PassEntity GetStrength(string password);
    }
}