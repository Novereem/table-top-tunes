using Shared.Constants;
using Shared.Enums;
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
                return ServiceResult<Guid>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.MissingToken), HttpStatusCode.BadRequest);
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return ServiceResult<Guid>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UnauthorizedToken), HttpStatusCode.Unauthorized);
            }

            return ServiceResult<Guid>.SuccessResult(userId);
        }
    }
}
