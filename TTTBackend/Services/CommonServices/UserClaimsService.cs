using Shared.Constants;
using Shared.Enums;
using Shared.Factories;
using Shared.Interfaces.Services.CommonServices;
using Shared.Models.Common;
using System.Security.Claims;

namespace TTTBackend.Services.CommonServices
{
    public class UserClaimsService : IUserClaimsService
    {
        public ServiceResult<Guid> GetUserIdFromClaims(ClaimsPrincipal user)
        {
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return PredefinedFailures.GetFailure<Guid>(ErrorCode.JWTNullOrEmpty);
            }

            if (!Guid.TryParse(userIdClaim!.Value, out var userId))
            {
                return PredefinedFailures.GetFailure<Guid>(ErrorCode.UnauthorizedToken);
            }

            return ServiceResult<Guid>.SuccessResult(userId);
        }
    }
}
