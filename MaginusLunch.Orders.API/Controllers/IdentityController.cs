using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MaginusLunch.Orders.API.Controllers
{
    /// <summary>
    /// An internal for test only activity.
    /// </summary>
    [Produces("application/json")]
    [Route("api/identity")]
    public class IdentityController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Contruct an instance of the IdentityController [sic]
        /// </summary>
        /// <param name="authorizationService">Its dependent upon the IAuthorizationService</param>
        public IdentityController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        private bool RequestByHttpClient(HttpContext context)
        {
            if (context != null
                && context.Connection != null
                && context.Connection.RemoteIpAddress != null)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Returns the list of Claims associated with the Requesting Identity.
        /// </summary>
        /// <returns>An array of Claims (types and values).</returns>
        /// <response code="200">Returns the array of Claims</response>
        /// <response code="401">The Client needs to authenticate.</response>
        /// <response code="403">The Client fails authorization checks.</response>
        [HttpGet]
        [ProducesResponseType(typeof(Claim[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Get()
        {
            if (RequestByHttpClient(HttpContext))
            {
                return Challenge(); // prevent actual calls from a HttpClient
            }
            var approved = await _authorizationService.AuthorizeAsync(User, "IsMaginusEmployee").ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            //var claims = from c in User.Claims select new { c.Type, c.Value }
            return new JsonResult(User.Claims);
        }
    }
}
