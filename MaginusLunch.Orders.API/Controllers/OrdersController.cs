using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MaginusLunch.Orders.Domain;
using Microsoft.AspNetCore.Authorization;
using MaginusLunch.Orders.API.Authorization;
using MaginusLunch.Orders.API.Extensions;
using MaginusLunch.Orders.Service.Validators;
using MaginusLunch.Core.Validation;
using MaginusLunch.Orders.Service;

namespace MaginusLunch.Orders.API.Controllers
{
    /// <summary>
    /// Manages the API Request Scope for Lunch Orders
    /// </summary>
    [Produces("application/json")]
    [Route("api/orders")]
    public class OrdersController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IOrderService _orderService;


        /// <summary>
        /// Contruct an instance of the OrdersController [sic]
        /// </summary>
        /// <param name="authorizationService">Its dependent upon the IAuthorizationService</param>
        /// <param name="orderService">Its dependent on IOrderService</param>
        public OrdersController(IAuthorizationService authorizationService,
            IOrderService orderService)
        {
            _authorizationService = authorizationService;
            _orderService = orderService;
        }

        /// <summary>
        /// Produce a list of all Lunch Orders for the requested Delivery Date.
        /// </summary>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <response code="200">A JSON Array of Lunch Orders.</response>
        /// <response code="400">The route data (Delivery Date) does not validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        [HttpGet("{deliveryDate}")]
        [ProducesResponseType(typeof(Order[]), 200)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllOrdersForDateAsync(string deliveryDate)
        {
            if (!deliveryDate.IsValidDeliveryDate())
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var approved = await _authorizationService.AuthorizeAsync(
                User, 
                new UserRetrievalAuthorizationResource(string.Empty), 
                AuthorizationPolicies.CanAccessOrders).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var orders = await _orderService.GetOrdersAsync(deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            return new JsonResult(orders);
        }

        /// <summary>
        /// Produce a list of all Lunch Orders for the requested Delivery Date and User.
        /// </summary>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <returns>For the Delivery Date and the User, returns their Lunch Order or HTTP404 if there is not one.</returns>
        /// <response code="200">The Lunch Order.</response>
        /// <response code="400">The route data (User and Delivery Date) does not validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no Order for that user for delivery on that date.</response>
        [HttpGet("{deliveryDate}/{userId}")]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOrderForUserAsync(string deliveryDate, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || !deliveryDate.IsValidDeliveryDate())
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var approved = await _authorizationService.AuthorizeAsync(
                User, 
                new UserRetrievalAuthorizationResource(userId), 
                AuthorizationPolicies.CanAccessOrders).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var lunchOrder = await _orderService.GetOrderForRecipientAsync(userId, deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            if (lunchOrder == null)
            {
                return NotFound();
            }
            return new JsonResult(lunchOrder);
        }
    }
}
