using SanjeshP.Mosahebe.Entities.User;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Services.JWTService
{
    public interface IJwtService
    {
        Task<string> GenerateAsync(User user, UserToken tokens);
    }
}