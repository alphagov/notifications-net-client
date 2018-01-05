using System;
using System.Collections.Generic;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Notify.Exceptions;

namespace Notify.Authentication
{
    public class Authenticator
    {
        public static string CreateToken(string secret, string serviceId)
        {
            ValidateGuids(new [] { secret, serviceId });

            var payload = new Dictionary<string, object>()
            {
                { "iss", serviceId },
                { "iat", GetCurrentTimeAsSeconds() }
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var notifyToken = encoder.Encode(payload, secret);

            return notifyToken;
        }

        public static Double GetCurrentTimeAsSeconds()
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);
        }

        public static IDictionary<string, Object> DecodeToken(string token, string secret)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var jsonPayload = decoder.DecodeToObject<IDictionary<string, object>>(token, secret, verify: true);

                return jsonPayload;
            }
            catch (Exception e) when (e is JWT.SignatureVerificationException || e is ArgumentException)
            {
                throw new NotifyAuthException(e.Message);
            } 
            catch(Exception e)
            {
                throw e;
            }
        }

        public static void ValidateGuids(String[] stringGuids)
        {
            Guid newGuid;
            if (stringGuids != null)
            {
                foreach (var stringGuid in stringGuids)
                {
                    if (!Guid.TryParse(stringGuid, out newGuid))
                        throw new NotifyAuthException("Invalid secret or serviceId. Please check that your API Key is correct");
                }
            }
        }
    }
    
}
