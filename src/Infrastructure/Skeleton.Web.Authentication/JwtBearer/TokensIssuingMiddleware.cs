﻿namespace Skeleton.Web.Authentication.JwtBearer
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Models;
    using Newtonsoft.Json;
    using Serialization;
    using UserClaimsProvider;
    using UserClaimsProvider.Exceptions;

    public class TokensIssuingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserClaimsProvider _userClaimsProvider;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        private readonly SigningCredentials _signingCredentials;
        private readonly PathString _getEndpointPath;
        private readonly TimeSpan? _lifetime;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public TokensIssuingMiddleware(RequestDelegate next, IUserClaimsProvider userClaimsProvider, IOptions<TokensIssuingOptions> options)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            if (userClaimsProvider == null)
                throw new ArgumentNullException(nameof(userClaimsProvider));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.Value.GetEndpotint))
                throw new ArgumentNullException(nameof(options.Value.GetEndpotint));
            if (string.IsNullOrWhiteSpace(options.Value.SigningAlgorithmName))
                throw new ArgumentNullException(nameof(options.Value.SigningAlgorithmName));
            if (options.Value.SigningKey == null)
                throw new ArgumentNullException(nameof(options.Value.SigningKey));

            _next = next;
            _userClaimsProvider = userClaimsProvider;
            _tokenHandler = new JwtSecurityTokenHandler();

            _getEndpointPath = new PathString(options.Value.GetEndpotint);
            _signingCredentials = new SigningCredentials(options.Value.SigningKey, options.Value.SigningAlgorithmName);
            _lifetime = options.Value.Lifetime;

            _jsonSerializerSettings = new JsonSerializerSettings().UseDefaultSettings();
        }

        private async Task<TokenRequestModel> DeserializeModel(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body))
                return JsonConvert.DeserializeObject<TokenRequestModel>(await reader.ReadToEndAsync(), _jsonSerializerSettings);
        }

        private static bool IsRequestValid(string requestMethod, TokenRequestModel requestModel)
        {
            return requestMethod == HttpMethods.Post
                   && string.IsNullOrWhiteSpace(requestModel.Login) == false
                   && string.IsNullOrWhiteSpace(requestModel.Password) == false;
        }

        private static void WriteErrorMessageToResponse(HttpResponse response, HttpStatusCode statusCode, string message)
        {
            response.StatusCode = (int)statusCode;
            response.ContentType = "text/plain";
            using (var output = new StreamWriter(response.Body, Encoding.UTF8, 4096, true))
                output.WriteLine(message);
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Equals(_getEndpointPath) == false)
            {
                await _next(context);
                return;
            }

            var requestModel = await DeserializeModel(context.Request);
            if (IsRequestValid(context.Request.Method, requestModel) == false)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            try
            {
                var claims = await _userClaimsProvider.GetClaimsAsync(requestModel.Login, requestModel.Password);
                var notBefore = DateTime.UtcNow;
                var expires = _lifetime.HasValue ? notBefore.Add(_lifetime.Value) : (DateTime?) null;
                var token = new JwtSecurityToken(claims: claims, notBefore: DateTime.UtcNow, expires: expires,
                    signingCredentials: _signingCredentials);

                var response = new TokenResponseModel {Token = _tokenHandler.WriteToken(token), ExpirationDate = expires};
                context.Response.ContentType = "application/json; charset=utf-8";
                using (var output = new StreamWriter(context.Response.Body, Encoding.UTF8, 4096, true))
                    output.WriteLine(JsonConvert.SerializeObject(response, _jsonSerializerSettings));
            }
            catch (LoginNotFoundException exception)
            {
                WriteErrorMessageToResponse(context.Response, HttpStatusCode.NotFound, exception.Message);
            }
            catch (IncorrectPasswordException exception)
            {
                WriteErrorMessageToResponse(context.Response, HttpStatusCode.Forbidden, exception.Message);
            }
        }
    }
}