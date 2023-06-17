using Core.Utilities.Security.Jwt;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Utilities.Security.Jwt
{
    public interface ITokenHelper
    {
        string CreateToken(User user, List<OperationClaim> operationClaims);

        bool Validate(string token);
        string GetClaim(string token, ClaimNames claimType);
    }
}
