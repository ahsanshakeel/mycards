using System.Threading.Tasks;

namespace MyCard.Web.Security
{
    public interface IAuthProvider
    {
        Task<string> GetUserAccessTokenAsync();
    }
}