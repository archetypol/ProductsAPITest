using Microsoft.AspNetCore.Identity;
using ProductsApi.Common.Models;
using ProductsApi.Auth.Models;

namespace ProductsApi.Auth.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ProductsApiUser> _userManager;

        public UserRepository(UserManager<ProductsApiUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<ProductsApiUser>> QueryByEmail(string email)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
                return Result<ProductsApiUser>.Failure(["Email Does Not Exist"]);

            return Result<ProductsApiUser>.Success(existingUser);

        }

        public async Task<Result<ProductsApiUser>> CreateNewUser(string email, string password, bool IsAdmin)
        {
            // tiny bit of code dupication here but not hurting anyone
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return Result<ProductsApiUser>.Failure(["Email already signed up"]);

            var newUser = new ProductsApiUser
            {
                UserName = email,
                Email = email,
                IsAdmin = IsAdmin,
            };

            var newUserCreated = await _userManager.CreateAsync(newUser, password);
            if (!newUserCreated.Succeeded)
            {
                var errors = newUserCreated.Errors.Select(e => e.Description).ToList();

                return Result<ProductsApiUser>.Failure(errors);
            }
            ;

            return Result<ProductsApiUser>.Success(newUser);
        }

        public async Task<Result<ProductsApiUser>> CheckUserCredentials(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
                return Result<ProductsApiUser>.Failure(["Email Does Not Exist"]);

            var passwordIsValid = await _userManager.CheckPasswordAsync(
                existingUser,
                password
            );
            if (!passwordIsValid)
                return Result<ProductsApiUser>.Failure(["Invalid Password"]);

            return Result<ProductsApiUser>.Success(existingUser);
            
        }
    }
}