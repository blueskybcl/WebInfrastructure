﻿namespace Skeleton.Web.Authorisation.JwtBearer.Configuration
{
    using System;
    using Microsoft.IdentityModel.Tokens;

    public static class TokensIssuingOptionsExtensions
    {
        public static TokensIssuingOptions WithGetEndpotint(this TokensIssuingOptions options, string endpoint)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException(nameof(endpoint));

            options.GetEndpotint = endpoint;
            return options;
        }

        public static TokensIssuingOptions WithSigningKey(this TokensIssuingOptions options, string signingAlgorithmName, SecurityKey signingKey)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if(string.IsNullOrWhiteSpace(signingAlgorithmName))
                throw new ArgumentNullException(nameof(signingAlgorithmName));
            if (signingKey == null)
                throw new ArgumentNullException(nameof(signingKey));

            options.SigningAlgorithmName = signingAlgorithmName;
            options.SigningKey = signingKey;
            return options;
        }

        public static TokensIssuingOptions WithLifeTime(this TokensIssuingOptions options, TimeSpan? lifeTime)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.LifeTime = lifeTime;
            return options;
        }
    }
}