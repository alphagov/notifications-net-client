using Notify.Exceptions;
using System;
using System.Collections.Generic;
using Notify.Interfaces;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Authentication
{
    public class Authenticator
    {
        public static String CreateToken(String secret, String serviceId)
        {
            ValidateGuids(new String[] { secret, serviceId });

            var payload = new Dictionary<String, object>()
            {
                { "iss", serviceId },
                { "iat", GetCurrentTimeAsSeconds() }
            };

            String notifyToken = JWT.JsonWebToken.Encode(payload, secret, JWT.JwtHashAlgorithm.HS256);
            return notifyToken;
        }

        public static double GetCurrentTimeAsSeconds()
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);
        }

        public static IDictionary<String, Object> DecodeToken(String token, String secret)
        {
            try
            {
                var jsonPayload = JWT.JsonWebToken.DecodeToObject(token, secret) as IDictionary<String, Object>;
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
