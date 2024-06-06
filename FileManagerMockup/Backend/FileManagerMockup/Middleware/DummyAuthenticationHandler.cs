using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace FileManagerMockup.Middleware
{
    public class DummyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public DummyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, "dummy_user"),
            new Claim(ClaimTypes.Name, "dummy_user")
        };
            var identity = new ClaimsIdentity(claims, "DummyScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "DummyScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
