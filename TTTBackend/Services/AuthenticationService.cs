using Shared.Models.DTOs;
using Shared.Models.Extensions;
using Shared.Models.Common.Extensions;
using Shared.Interfaces.Services;
using Shared.Interfaces.Data;
using Shared.Constants;
using Shared.Enums;
using Shared.Models.Common;
using Shared.Interfaces.Services.CommonServices;
using TTTBackend.Services.Helpers;
using Shared.Factories;
using Shared.Models;
using Shared.Interfaces.Services.Helpers;

namespace TTTBackend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationData _authData;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IAuthenticationServiceHelper _helper;

        public AuthenticationService(
            IAuthenticationData authData,
            IPasswordHashingService passwordHashingService,
            ILogger<AuthenticationService> logger,
            IAuthenticationServiceHelper authenticationServiceHelper
        )
        {
            _authData = authData;
            _passwordHashingService = passwordHashingService;
            _logger = logger;
            _helper = authenticationServiceHelper;
        }

        public async Task<ServiceResult<RegisterResponseDTO>> RegisterUserAsync(UserRegistrationDTO registrationDTO)
        {
            var validationResult = await _helper.ValidateRegistrationAsync(registrationDTO);
            if (!validationResult.Success)
            {
                return validationResult.ToFailureResult<RegisterResponseDTO>();
            }

            try
            {
                var newUser = registrationDTO.ToUserFromRegistrationDTO(_passwordHashingService);
                await _authData.RegisterUserAsync(newUser);

                return ServiceResult<RegisterResponseDTO>.SuccessResult(
                    newUser.ToRegisterResponseDTO(),
                    SuccessMessages.GetSuccessMessage(SuccessCode.Register),
                    SuccessMessagesUser.GetSuccessMessage(SuccessCodeUser.Register),
                    HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while registering user. Username: {Username}, Email: {Email}", registrationDTO.Username, registrationDTO.Email);
                return PredefinedFailures.GetFailure<RegisterResponseDTO>(ErrorCode.InternalServerError);
            }
        }

        public async Task<ServiceResult<LoginResponseDTO>> ValidateUserAsync(UserLoginDTO loginDTO)
        {
            var validationResult = await _helper.ValidateLoginAsync(loginDTO);
            if (!validationResult.Success)
            {
                return validationResult.ToFailureResult<LoginResponseDTO>();
            }

            try
            {
                var user = validationResult.Data;
                var getToken = _helper.GenerateJwtToken(user!.Id, user.Username);
                if (!getToken.Success)
                {
                    return validationResult.ToFailureResult<LoginResponseDTO>();
                }
                return ServiceResult<LoginResponseDTO>.SuccessResult(
                    getToken.Data!.ToLoginResponseDTO(),
                    SuccessMessages.GetSuccessMessage(SuccessCode.Login),
                    SuccessMessagesUser.GetSuccessMessage(SuccessCodeUser.Login),
                    HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while validating user. Username: {Username}", loginDTO.Username);
                return PredefinedFailures.GetFailure<LoginResponseDTO>(ErrorCode.InternalServerError);
            }
        }

        public async Task<ServiceResult<User>> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var user = await _authData.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return PredefinedFailures.GetFailure<User>(ErrorCode.ResourceNotFound);
                }

                return ServiceResult<User>.SuccessResult(
                    user,
                    SuccessMessages.GetSuccessMessage(SuccessCode.Success),
                    httpStatusCode: HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting user. Guid: {userId}", userId);
                return PredefinedFailures.GetFailure<User>(ErrorCode.InternalServerError);
            }
        }
    }
}
