using Fido2NetLib;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAuthnAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CredentialsController : ControllerBase
    {
        [HttpPost, Route("register")]
        public async Task<IActionResult> Post([FromBody] RegisterCredentials credentialsForRegistration)
        {
            var json = JsonConvert.SerializeObject(credentialsForRegistration);
            var authenticatorAttestationRawResponse =
                JsonConvert.DeserializeObject<AuthenticatorAttestationRawResponse>(json);

            return await Task.FromResult(Ok());
        }

        [HttpPost, Route("initiate-registration")]
        public async Task<IActionResult> Post()
        {


            return await Task.FromResult(Ok());
        }
    }

    public class RegisterCredentials
    {
        public string Id { get; set; }
        public string RawId { get; set; }
        public AuthenticatorResponse Response { get; set; }
        public string Type { get; set; }
    }

    public class AuthenticatorResponse
    {

        public string AttestationObject { get; set; }
        public string ClientDataJSON { get; set; }
    }
}
