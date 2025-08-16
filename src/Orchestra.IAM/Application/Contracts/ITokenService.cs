using System.Security.Claims;
using Orchestra.IAM.Domain.Entities;

namespace Orchestra.IAM.Application.Contracts;

public interface ITokenService
{
    string GenerateToken(AppUser user);
}