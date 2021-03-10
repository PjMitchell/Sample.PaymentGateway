using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace PaymentGateway.Auth
{
    /// <summary>
    /// Dev authentication Handler, will need to create a production Handler that gets merchant id and permissions from api key 
    /// </summary>
    public class DevAuthenticationHandler : AuthenticationHandler<DevAuthenticationOptions>
    {
        public const string Schema = "DevAuth";
        public DevAuthenticationHandler(IOptionsMonitor<DevAuthenticationOptions> options, ILoggerFactory loggerFactory, UrlEncoder encoder, ISystemClock clock) : base(options, loggerFactory, encoder, clock)
        {

        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var devClaims = new List<Claim>
            {
                new Claim(MerchantClaims.MechantIdClaimType, Options.MerchantId)
            };
            if (Options.CanReadRequest)
                devClaims.Add(new Claim(MerchantClaims.PaymentRequestClaimType, MerchantClaims.PaymentRequestReadValue));
            if (Options.CanSubmitRequest)
                devClaims.Add(new Claim(MerchantClaims.PaymentRequestClaimType, MerchantClaims.PaymentRequestSubmitValue));
            var identity = new ClaimsIdentity(devClaims, nameof(DevAuthenticationHandler));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
        }
    }
}
