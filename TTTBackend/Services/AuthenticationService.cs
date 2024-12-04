using Shared.Models.DTOs;
using Shared.Models;
using TTTBackend.Data;
using TTTBackend.Services.CommonServices;
using Shared.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared.Interfaces.Data;
using Shared.Models.Extensions;
using Shared.Constants;
using Shared.Enums;
using Shared.Models.Common;
using Shared.Interfaces.Services.CommonServices;
using TTTBackend.Services.Helpers;

namespace TTTBackend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationData _authData;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly AuthenticationServiceHelper _helper;

        public AuthenticationService(
            IAuthenticationData authData,
            IPasswordHashingService passwordHashingService,
            ILogger<AuthenticationService> logger
        )
        {
            _authData = authData;
            _passwordHashingService = passwordHashingService;
            _logger = logger;
            _helper = new AuthenticationServiceHelper(authData, passwordHashingService, logger);
        }

        public async Task<ServiceResult<string>> RegisterUserAsync(UserRegistrationDTO registrationDTO)
        {
            var validationResult = await _helper.ValidateRegistrationAsync(registrationDTO);
            if (!validationResult.Success)
            {
                return ServiceResult<string>.Failure(validationResult.ErrorMessage, validationResult.HttpStatusCode);
            }

            try
            {
                var newUser = registrationDTO.ToUserFromRegistrationDTO(_passwordHashingService);
                await _authData.RegisterUserAsync(newUser);

                return ServiceResult<string>.SuccessResult(
                    SuccessMessages.GetSuccessMessage(SuccessCode.Register),
                    HttpStatusCode.Created
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while registering user. Username: {Username}, Email: {Email}", registrationDTO.Username, registrationDTO.Email);
                return ServiceResult<string>.Failure(
                    ErrorMessages.GetErrorMessage(ErrorCode.UnknownError),
                    HttpStatusCode.InternalServerError
                );
            }
        }

        public async Task<(ServiceResult<string>, string? token)> ValidateUserAsync(UserLoginDTO loginDTO)
        {
            var validationResult = await _helper.ValidateLoginAsync(loginDTO);
            if (!validationResult.Success)
            {
                return (ServiceResult<string>.Failure(validationResult.ErrorMessage, validationResult.HttpStatusCode), null);
            }

            try
            {
                var user = validationResult.Data;
                var token = _helper.GenerateJwtToken(user.Id, user.Username);
                return (
                    ServiceResult<string>.SuccessResult(
                        SuccessMessages.GetSuccessMessage(SuccessCode.Login),
                        HttpStatusCode.OK
                    ),
                    token
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while validating user. Username: {Username}", loginDTO.Username);
                return (ServiceResult<string>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError),HttpStatusCode.InternalServerError), null);
            }
        }
    }
}
