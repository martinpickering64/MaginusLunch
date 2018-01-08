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
    [Route("api/menu/calendar")]
    public class CalendarController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Contruct an instance of the CalendarController [sic]
        /// </summary>
        /// <param name="authorizationService">Its dependent upon the IAuthorizationService</param>
        public CalendarController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Obtain the Calendar that is in use by, and governs most of the operations of, the Maginus Luch Application.
        /// </summary>
        /// <response code="200">The Calendar in use by Maginus Lunch.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no Calendar as of yet!</response>
        [HttpGet]
        [ProducesResponseType(typeof(Calendar), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> GetCalendarAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the Calendar to put into use by, and to govern most of the operations of, the Maginus Luch Application.
        /// </summary>
        /// <response code="201">The identity of the Calendar created.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), 201)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public Task<IActionResult> CreateCalendarAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Activate the date on the Calendar.
        /// </summary>
        /// <param name="affectedDate">The date to activate on the Calendar as yyyyMMdd.</param>
        /// <response code="204">The date has been activated.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no Calendar as of yet!</response>
        [HttpPut("{affectedDate}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> ActivateDateAsync(string affectedDate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deactivate the date on the Calendar.
        /// </summary>
        /// <param name="affectedDate">The date to deactivate on the Calendar as yyyyMMdd.</param>
        /// <response code="204">The date has been deactivated.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no Calendar as of yet!</response>
        [HttpDelete("{affectedDate}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> DeactivateDateAsync(string affectedDate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Activate the day on the Calendar.
        /// </summary>
        /// <param name="affectedDay">The day to activate on the Calendar.</param>
        /// <remarks>The {affectedDay} should be a string representing a valid System.DayOfWeek Enuerator label, 
        /// e.g. Monday, Tuesday etc.</remarks>
        /// <response code="204">The day has been activated.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no Calendar as of yet!</response>
        [HttpPut("day/{affectedDay}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> ActivateDayAsync(string affectedDay)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deactivate the day on the Calendar.
        /// </summary>
        /// <param name="affectedDay">The day to deactivate on the Calendar.</param>
        /// <remarks>The {affectedDay} should be a string representing a valid System.DayOfWeek Enuerator label, 
        /// e.g. Monday, Tuesday etc.</remarks>
        /// <response code="204">The day has been deactivated.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no Calendar as of yet!</response>
        [HttpDelete("day/{affectedDay}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> DeactivateDayAsync(string affectedDay)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the most recent delivery date from which point Lunch Orders can still be accepted for.
        /// </summary>
        /// <param name="affectedDate">The date of the new Open Order (delivery) Date as yyyyMMdd.</param>
        /// <response code="204">The date has been activated.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no Calendar as of yet!</response>
        [HttpPut("OpenOrderDate/{affectedDay}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public Task<IActionResult> AmendOpenOrderDateAsync(string affectedDate)
        {
            throw new NotImplementedException();
        }
    }
}