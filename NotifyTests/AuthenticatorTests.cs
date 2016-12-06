using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notify.Authentication;
using Notify.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyUnitTests
{
    [TestClass()]
    public class AuthenticatorTests
    {
        private const String NOTIFY_SECRET_ID = "d2d425c1-d1d7-4af2-b727-fbd7b612aae1";
        private const String NOTIFY_SERVICE_ID = "9b8c5eef-0b4a-4d59-b8e4-0a7a4267e6c3";
        private const String INVALID_TOKEN = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJjbGFpbTEiOjAsImNsYWltMiI6ImNsYWltMi12YWx1ZSJ9.8pwBI_HtXqI3UgQHQ_rDRnSQRxFL1SR8fbQoS-5kM5s";

        [TestMethod()]
        [TestCategory("Unit/AuthenticationTests")]
        [ExpectedException(typeof(NotifyAuthException), "A token was generated with an invalid secret Id")]
        public void CreateTokenWithInvalidSecretThrowsAuthException()
        {
            try
            {
                Authenticator.CreateToken("invalidsecret", NOTIFY_SERVICE_ID);
            }
            catch(Exception e)
            {
                Assert.AreEqual(e.Message, "Invalid secret or serviceId. Please check that your API Key is correct");
                throw;
            }
        }

        [TestMethod()]
        [TestCategory("Unit/AuthenticationTests")]
        [ExpectedException(typeof(NotifyAuthException), "A token was generated with an invalid service Id")]
        public void CreateTokenWithInvalidServiceIdThrowsAuthException()
        {
            try
            {
                Authenticator.CreateToken(NOTIFY_SECRET_ID, "invalid service id");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Invalid secret or serviceId. Please check that your API Key is correct");
                throw;
            }
        }

        [TestMethod()]
        [TestCategory("Unit/AuthenticationTests")]
        public void CreateTokenWithCredentialsShouldGenerateValidJWT()
        {
            String token = Authenticator.CreateToken(NOTIFY_SECRET_ID, NOTIFY_SERVICE_ID);

            // Is correct string
            if(String.IsNullOrWhiteSpace(token) || token.Contains(" "))
            {
                Assert.Fail();
            }

            // Validate can decode and payload matches
            IDictionary<String, Object> jsonPayload = Authenticator.DecodeToken(token, NOTIFY_SECRET_ID);
            Assert.AreEqual(jsonPayload["iss"], NOTIFY_SERVICE_ID);

            // Validate token issed time is within reasonable time
            Double currentTimeAsSeconds = Authenticator.GetCurrentTimeAsSeconds();
            Double tokenIssuedAt = Convert.ToDouble(jsonPayload["iat"]);
            if(tokenIssuedAt < (currentTimeAsSeconds - 15) || 
                tokenIssuedAt > (currentTimeAsSeconds + 15))
            {
                Assert.Fail();
            }

        }

        [TestMethod()]
        [TestCategory("Unit/AuthenticationTests")]
        [ExpectedException(typeof(NotifyAuthException), "An invalid token was decoded successfully")]
        public void DecodeInvalidTokenWithNoDotsShouldThrowAuthException()
        {
            try
            {
                Authenticator.DecodeToken("tokenwithnodots", NOTIFY_SECRET_ID);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Token must consist from 3 delimited by dot parts");
                throw;
            }
        }

        [TestMethod()]
        [TestCategory("Unit/AuthenticationTests")]
        [ExpectedException(typeof(NotifyAuthException), "An invalid token was decoded successfully")]
        public void DecodeInvalidTokenShouldThrowAuthException()
        {
            try
            {
                Authenticator.DecodeToken(INVALID_TOKEN, NOTIFY_SECRET_ID);
            }
            catch (Exception e)
            {
                StringAssert.StartsWith(e.Message, "Invalid signature");
                throw;
            }
        }

    }
}
