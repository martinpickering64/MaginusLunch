using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MaginusLunch.Menu.Domain;
using MaginusLunch.Core.Validation;

namespace MaginusLunch.Menu.API.Controllers
{
    /// <summary>
    /// Manages the API Request Scope for Maginus Lunch Menu
    /// </summary>
    [Produces("application/json")]
    [Route("api/menu/supplier")]
    public class LunchProviderController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Contruct an instance of the LunchProviderController [sic]
        /// </summary>
        /// <param name="authorizationService">Its dependent upon the IAuthorizationService</param>
        public LunchProviderController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Obtain all of the Suppliers (LunchProviders) of Maginus Lunches.
        /// </summary>
        /// <response code="200">A JSON Array of LunchProviders.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        [HttpGet]
        [ProducesResponseType(typeof(LunchProvider[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public Task<IActionResult> GetAllLunchProvidersAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtain a specific Supplier (LunchProvider) of Maginus Lunches.
        /// </summary>
        /// <param name="lunchProviderId">The identity of the required LunchProvider (Guid).</param>
        /// <response code="200">The LunchProvider.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">The LunchProvider does not exist.</response>
        [HttpGet("{lunchProviderId}")]
        [ProducesResponseType(typeof(LunchProvider), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> GetLunchProvidersAsync(Guid lunchProviderId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a LunchProvider.
        /// </summary>
        /// <response code="201">The identity of the LunchProvider created.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        [HttpPost("{lunchProviderId}")]
        [ProducesResponseType(typeof(Guid), 201)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public Task<IActionResult> CreateLunchProviderAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Activate the LunchProvider for the given date.
        /// </summary>
        /// <param name="lunchProviderId">The identity of the LunchProvider (Guid).</param>
        /// <param name="affectedDate">The date to activate for the LunchProvider as yyyyMMdd.</param>
        /// <response code="204">The date has been activated.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no such LunchProvider as of yet!</response>
        [HttpPut("{lunchProviderId}/{affectedDate}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> ActivateDateAsync(Guid lunchProviderId, string affectedDate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deactivate the LunchProvider for the given date.
        /// </summary>
        /// <param name="lunchProviderId">The identity of the required LunchProvider (Guid).</param>
        /// <param name="affectedDate">The date to deactivate for the LunchProvider as yyyyMMdd.</param>
        /// <response code="204">The date has been deactivated.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no such LunchProvider as of yet!</response>
        [HttpDelete("{lunchProviderId}/{affectedDate}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> DeactivateDateAsync(Guid lunchProviderId, string affectedDate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Activate the LunchProvider.
        /// </summary>
        /// <param name="lunchProviderId">The identity of the LunchProvider (Guid).</param>
        /// <response code="204">The LunchProvider has been activated.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no such LunchProvider as of yet!</response>
        [HttpPut("{lunchProviderId}/available")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> ActivateAsync(Guid lunchProviderId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deactivate the LunchProvider.
        /// </summary>
        /// <param name="lunchProviderId">The identity of the LunchProvider (Guid).</param>
        /// <response code="204">The LunchProvider has been deactivated.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no such LunchProvider as of yet!</response>
        [HttpDelete("{lunchProviderId}/available")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> WithdrawAsync(Guid lunchProviderId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change the Name of the LunchProvider.
        /// </summary>
        /// <param name="lunchProviderId">The identity of the LunchProvider (Guid).</param>
        /// <param name="newName">The new value for the LunchProvider's Name.</param>
        /// <response code="204">The Name has been changed.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no such LunchProvider as of yet!</response>
        [HttpPut("{lunchProviderId}/Name/{newName}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> ChangeNameAsync(Guid lunchProviderId, string newName)
        {
            throw new NotImplementedException();
        }
    }
}