using Blog.Models;
using System.Security.Claims;

namespace Blog.Extensions
{
    public static class RoleClaimsExtension
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var result = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.Email, user.Email)
            };

            result.AddRange(
                collection: user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name))
            );
            
            return result;
        }
    }
}
