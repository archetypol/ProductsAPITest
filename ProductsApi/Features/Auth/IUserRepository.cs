using ProductsApi.Common.Models;
using ProductsApi.Auth.Models;

namespace ProductsApi.Auth.Repository
{
    public interface IUserRepository
    {
        Task<Result<ProductsApiUser>> QueryByEmail(string email);
        Task<Result<ProductsApiUser>> CreateNewUser(string email, string password, bool IsAdmin);
        Task<Result<ProductsApiUser>> CheckUserCredentials(string email, string password);
    }
}