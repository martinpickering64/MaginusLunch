using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MaginusLunch.Orders.Domain;
using Microsoft.AspNetCore.Authorization;
using MaginusLunch.Orders.API.Extensions;
using MaginusLunch.Orders.Messages.Commands;
using Microsoft.AspNetCore.Http;
using MaginusLunch.Orders.API.Models;
using MaginusLunch.Orders.Repository;
using MaginusLunch.Orders.API.Authorization;
using MaginusLunch.Orders.Service;
using MaginusLunch.Core.Messages.Commands;
using System.Linq;
using MaginusLunch.Orders.Service.Validators;
using MaginusLunch.Core.Validation;
using System.Collections.Generic;

namespace MaginusLunch.Orders.API.Controllers
{
    /// <summary>
    /// Manages the API Request Scope for Maginus Lunch Users
    /// </summary>
    [Produces("application/json")]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IOrderService _orderService;
        private readonly ICalendarService _calendarService;

        /// <summary>
        /// Contruct an instance of the UsersController [sic]
        /// </summary>
        /// <param name="authorizationService">Its dependent upon the IAuthorizationService</param>
        /// <param name="calendarService"></param>
        /// <param name="orderService">Its dependent on the Order Service</param>
        public UsersController(IAuthorizationService authorizationService,
            ICalendarService calendarService,
            IOrderService orderService)
        {
            _authorizationService = authorizationService;
            _orderService = orderService;
        }

        /// <summary>
        /// Produce a list of all Lunch Orders for the requested Delivery Date and User.
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <response code="200">The Lunch Order for the identified User on the Delivery Date.</response>
        /// <response code="400">The Route Data (userId and deliveryDate) does not validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">There is no order for that User for delivery on that date.</response>
        [HttpGet("{userId}/{deliveryDate}")]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOrderForUserAsync(string userId, string deliveryDate)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(deliveryDate)
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
            var lunchOrder = await _orderService.GetOrderForRecipientAsync(
                recipientUserId: userId, 
                forDeliveryDate: deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            if (lunchOrder == null)
            {
                return NotFound();
            }
            return new JsonResult(lunchOrder);
        }

        /// <summary>
        /// Create a new Lunch Order
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <response code="201">The identity of the Order that has been added.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="409">The delivery date has been closed such that the Add Order Command cannot be accepted.</response>
        [HttpPost("{userId}/{deliveryDate}")]
        [ProducesResponseType(typeof(Guid), 201)]
        [ProducesResponseType(typeof(IEnumerable<ValidationStatus.Reason>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ValidationStatus.Reason), 409)]
        public async Task<IActionResult> AddOrder(string userId, string deliveryDate)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(deliveryDate)
                || !deliveryDate.IsValidDeliveryDate())
            {
                return BadRequest(new [] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var asDate = deliveryDate.ConvertToDeliveryDate();
            var command = new AddOrder
            {
                Id = Guid.NewGuid(),
                RecipientUserId = userId,
                DeliveryDate = asDate
            };
            var approved = await _authorizationService.AuthorizeAsync(
                                    User,
                                    command,
                                    AuthorizationPolicies.CanAddOrder).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var order = await _orderService.GetOrderForRecipientAsync(userId, asDate).ConfigureAwait(false);
            if (order != null)
            {
                //_logger.WarnFormat("AddOrder Command received when an Order for [{0}] on [{1}] already exists; Order.Id = {2}.", userId, asDate, order.Id);
                return new JsonResult(order.Id);
            }
            var commandStatus = await _orderService.HandleForUserAsync(User, command).ConfigureAwait(false);
            if (commandStatus.AckNack == CommandStatus.AckNackStatus.ACK)
            {
                return new JsonResult(command.Id);
            }
            if (commandStatus.Reasons.Any(r => r.Code == OrderCommandValidator.AggregateWithIdExistsCode))
            {
                //_logger.WarnFormat("AddOrder Command received when an Order for [{0}] on [{1}] already exists; Order.Id = {2}.", userId, asDate, order.Id);
                return new JsonResult(order.Id);
            }
            var reason = commandStatus.Reasons.FirstOrDefault(r => r.Code == OrderCommandValidator.DeliveryDateUnavailableCode);
            if (reason != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, reason);
            }
            return BadRequest(commandStatus.Reasons);
        }

        /// <summary>
        /// Add Bread to an existing Lunch Order
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <param name="breadId">The identity of the Bread option to add to the Order.</param>
        /// <response code="204">The Order has been amended.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">The Order does not exist.</response>
        /// <response code="409">An Order cannot accept Bread or the delivery date has been closed such that
        /// the Command cannot be accepted.</response>
        [HttpPut("{userId}/{deliveryDate}/bread")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ValidationStatus.Reason), 409)]
        public async Task<IActionResult> AddBread(string userId, string deliveryDate, [FromBody]Guid breadId)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(deliveryDate)
                || !deliveryDate.IsValidDeliveryDate()
                || breadId == Guid.Empty)
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var asDate = deliveryDate.ConvertToDeliveryDate();
            var order = await _orderService.GetOrderForRecipientAsync(
                                                    recipientUserId: userId,
                                                    forDeliveryDate: asDate)
                                                .ConfigureAwait(false);
            if (order == null) { return NotFound(); }
            var command = new AddBreadToOrder
            {
                Id = order.Id,
                BreadId = breadId
            };
            var approved = await _authorizationService.AuthorizeAsync(
                                    User,
                                    new OrderCommandAuthorizationResource(order, command),
                                    AuthorizationPolicies.WillAcceptOrderCommand).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var commandStatus = await _orderService.HandleForUserAsync(User, command).ConfigureAwait(false);
            if (commandStatus.AckNack == CommandStatus.AckNackStatus.ACK)
            {
                return NoContent();
            }
            var reason = commandStatus.Reasons.FirstOrDefault(r => r.Code == OrderCommandValidator.DeliveryDateUnavailableCode
                                                                   || r.Code == OrderCommandValidator.BreadNotAllowed.Code
                                                                   || r.Code == OrderCommandValidator.BreadUnavailable.Code);
            if (reason != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, reason);
            }
            return BadRequest(commandStatus.Reasons);
        }

        /// <summary>
        /// Remove Bread from an existing Lunch Order
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <response code="204">The Order has been amended.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">The Order does not exist.</response>
        /// <response code="409">The delivery date has been closed such that Command cannot be accepted.</response>
        [HttpDelete("{userId}/{deliveryDate}/bread")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ValidationStatus.Reason), 409)]
        public async Task<IActionResult> RemoveBread(string userId, string deliveryDate)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(deliveryDate)
                || !deliveryDate.IsValidDeliveryDate())
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var order = await _orderService.GetOrderForRecipientAsync(
                recipientUserId: userId,
                forDeliveryDate: deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            if (order == null)
            {
                return NotFound();
            }
            var command = new RemoveBreadFromOrder
            {
                Id = order.Id
            };
            var approved = await _authorizationService.AuthorizeAsync(
                                    User,
                                    new OrderCommandAuthorizationResource(order, command),
                                    AuthorizationPolicies.WillAcceptOrderCommand).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var commandStatus = await _orderService.HandleForUserAsync(User, command).ConfigureAwait(false);
            if (commandStatus.AckNack == CommandStatus.AckNackStatus.ACK)
            {
                return NoContent();
            }
            var reason = commandStatus.Reasons.FirstOrDefault(r => r.Code == OrderCommandValidator.DeliveryDateUnavailableCode);
            if (reason != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, reason);
            }
            return BadRequest(commandStatus.Reasons);
        }

        /// <summary>
        /// Add a Menu Option to an existing Lunch Order
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <param name="menuOptionId">The identity of the Menu Option to add to the Order.</param>
        /// <response code="204">The Order has been amended.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">The Order does not exist.</response>
        /// <response code="409">An Order cannot accept Menu Options or the delivery date has been closed such that
        /// the Command cannot be accepted.</response>
        [HttpPut("{userId}/{deliveryDate}/menuoption")]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ValidationStatus.Reason), 409)]
        public async Task<IActionResult> AddMenuOption(string userId, string deliveryDate, [FromBody]Guid menuOptionId)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(deliveryDate)
                || !deliveryDate.IsValidDeliveryDate()
                || menuOptionId == Guid.Empty)
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var order = await _orderService.GetOrderForRecipientAsync(
                recipientUserId: userId,
                forDeliveryDate: deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            if (order == null)
            {
                return NotFound();
            }
            var command = new AddMenuOptionToOrder
            {
                Id = order.Id,
                MenuOptionId = menuOptionId
            };
            var approved = await _authorizationService.AuthorizeAsync(
                                    User,
                                    new OrderCommandAuthorizationResource(order, command),
                                    AuthorizationPolicies.WillAcceptOrderCommand).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var commandStatus = await _orderService.HandleForUserAsync(User, command).ConfigureAwait(false);
            if (commandStatus.AckNack == CommandStatus.AckNackStatus.ACK)
            {
                return NoContent();
            }
            var reason = commandStatus.Reasons.FirstOrDefault(r => r.Code == OrderCommandValidator.DeliveryDateUnavailableCode
                                                                   || r.Code == OrderCommandValidator.MenuOptionNotAllowed.Code
                                                                   || r.Code == OrderCommandValidator.MenuOptionUnavailable.Code);
            if (reason != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, reason);
            }
            return BadRequest(commandStatus.Reasons);
        }

        /// <summary>
        /// Remove Menu Option from an existing Lunch Order
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <response code="204">The Order has been amended.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">The Order does not exist.</response>
        /// <response code="409">The delivery date has been closed such that Command cannot be accepted.</response>
        [HttpDelete("{userId}/{deliveryDate}/menuoption")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ValidationStatus.Reason), 409)]
        public async Task<IActionResult> RemoveMenuOption(string userId, string deliveryDate)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(deliveryDate)
                || !deliveryDate.IsValidDeliveryDate())
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var order = await _orderService.GetOrderForRecipientAsync(
                recipientUserId: userId,
                forDeliveryDate: deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            if (order == null)
            {
                return NotFound();
            }
            var command = new RemoveMenuOptionFromOrder
            {
                Id = order.Id
            };
            var approved = await _authorizationService.AuthorizeAsync(
                                    User,
                                    new OrderCommandAuthorizationResource(order, command),
                                    AuthorizationPolicies.WillAcceptOrderCommand).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var commandStatus = await _orderService.HandleForUserAsync(User, command).ConfigureAwait(false);
            if (commandStatus.AckNack == CommandStatus.AckNackStatus.ACK)
            {
                return NoContent();
            }
            var reason = commandStatus.Reasons.FirstOrDefault(r => r.Code == OrderCommandValidator.DeliveryDateUnavailableCode);
            if (reason != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, reason);
            }
            return BadRequest(commandStatus.Reasons);
        }

        /// <summary>
        /// Add a Filling to an existing Lunch Order
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <param name="fillingId">The identity of the Filling to add to the Order.</param>
        /// <response code="204">The Order has been amended.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">The Order does not exist.</response>
        /// <response code="409">The delivery date has been closed such that the Command cannot be accepted.</response>
        [HttpPut("{userId}/{deliveryDate}/filling")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ValidationStatus.Reason), 409)]
        public async Task<IActionResult> AddFilling(string userId, string deliveryDate, [FromBody]Guid fillingId)
        {
            if (fillingId == Guid.Empty)
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var order = await _orderService.GetOrderForRecipientAsync(
                recipientUserId: userId,
                forDeliveryDate: deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            if (order == null)
            {
                return NotFound();
            }
            var command = new AddFillingToOrder
            {
                Id = order.Id,
                FillingId = fillingId
            };
            var approved = await _authorizationService.AuthorizeAsync(
                                    User,
                                    new OrderCommandAuthorizationResource(order, command),
                                    AuthorizationPolicies.WillAcceptOrderCommand).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var commandStatus = await _orderService.HandleForUserAsync(User, command).ConfigureAwait(false);
            if (commandStatus.AckNack == CommandStatus.AckNackStatus.ACK)
            {
                return NoContent();
            }
            var reason = commandStatus.Reasons.FirstOrDefault(r => r.Code == OrderCommandValidator.DeliveryDateUnavailableCode
                                                                   || r.Code == OrderCommandValidator.FillingUnavailable.Code);
            if (reason != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, reason);
            }
            return BadRequest(commandStatus.Reasons);
        }

        /// <summary>
        /// Remove Filling from an existing Lunch Order
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <response code="204">The Order has been amended.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">The Order does not exist.</response>
        /// <response code="409">The delivery date has been closed such that Command cannot be accepted.</response>
        [HttpDelete("{userId}/{deliveryDate}/filling")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ValidationStatus.Reason), 409)]
        public async Task<IActionResult> RemoveFilling(string userId, string deliveryDate)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(deliveryDate)
                || !deliveryDate.IsValidDeliveryDate())
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var order = await _orderService.GetOrderForRecipientAsync(
                recipientUserId: userId,
                forDeliveryDate: deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            if (order == null)
            {
                return NotFound();
            }
            var command = new RemoveFillingFromOrder
            {
                Id = order.Id
            };
            var approved = await _authorizationService.AuthorizeAsync(
                                    User,
                                    new OrderCommandAuthorizationResource(order, command),
                                    AuthorizationPolicies.WillAcceptOrderCommand).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var commandStatus = await _orderService.HandleForUserAsync(User, command).ConfigureAwait(false);
            if (commandStatus.AckNack == CommandStatus.AckNackStatus.ACK)
            {
                return NoContent();
            }
            var reason = commandStatus.Reasons.FirstOrDefault(r => r.Code == OrderCommandValidator.DeliveryDateUnavailableCode);
            if (reason != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, reason);
            }
            return BadRequest(commandStatus.Reasons);
        }

        /// <summary>
        /// Cancel an existing Lunch Order
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <response code="204">The Order has been amended.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">The Order does not exist.</response>
        /// <response code="409">The delivery date has been closed such that the Command cannot be accepted.</response>
        [HttpPut("{userId}/{deliveryDate}/cancel")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ValidationStatus.Reason), 409)]
        public async Task<IActionResult> CancelOrder(string userId, string deliveryDate)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(deliveryDate)
                || !deliveryDate.IsValidDeliveryDate())
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var order = await _orderService.GetOrderForRecipientAsync(
                recipientUserId: userId,
                forDeliveryDate: deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            if (order == null)
            {
                return NotFound();
            }
            var command = new CancelOrder
            {
                Id = order.Id
            };
            var approved = await _authorizationService.AuthorizeAsync(
                                    User,
                                    new OrderCommandAuthorizationResource(order, command),
                                    AuthorizationPolicies.WillAcceptOrderCommand).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var commandStatus = await _orderService.HandleForUserAsync(User, command).ConfigureAwait(false);
            if (commandStatus.AckNack == CommandStatus.AckNackStatus.ACK)
            {
                return NoContent();
            }
            var reason = commandStatus.Reasons.FirstOrDefault(r => r.Code == OrderCommandValidator.DeliveryDateUnavailableCode);
            if (reason != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, reason);
            }
            return BadRequest(commandStatus.Reasons);
        }

        /// <summary>
        /// Complete an existing Lunch Order
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <response code="204">The Order has been amended.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">The Order does not exist.</response>
        /// <response code="409">The delivery date has been closed such that the Command cannot be accepted.</response>
        [HttpPut("{userId}/{deliveryDate}/complete")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ValidationStatus.Reason), 409)]
        public async Task<IActionResult> CompleteOrder(string userId, string deliveryDate)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(deliveryDate)
                || !deliveryDate.IsValidDeliveryDate())
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var order = await _orderService.GetOrderForRecipientAsync(
                recipientUserId: userId,
                forDeliveryDate: deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            if (order == null)
            {
                return NotFound();
            }
            var command = new MarkOrderAsComplete
            {
                Id = order.Id
            };
            var approved = await _authorizationService.AuthorizeAsync(
                                    User,
                                    new OrderCommandAuthorizationResource(order, command),
                                    AuthorizationPolicies.WillAcceptOrderCommand).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var commandStatus = await _orderService.HandleForUserAsync(User, command).ConfigureAwait(false);
            if (commandStatus.AckNack == CommandStatus.AckNackStatus.ACK)
            {
                return NoContent();
            }
            var reason = commandStatus.Reasons.FirstOrDefault(r => r.Code == OrderCommandValidator.DeliveryDateUnavailableCode);
            if (reason != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, reason);
            }
            return BadRequest(commandStatus.Reasons);
        }

        /// <summary>
        /// Flag an existing Lunch Order as incomplete
        /// </summary>
        /// <param name="userId">The identity of the User to filter the Order List by, e.g. martin.pickering</param>
        /// <param name="deliveryDate">DeliveryDate as yyyyMMdd</param>
        /// <response code="204">The Order has been amended.</response>
        /// <response code="400">The Route Data fails to validate.</response>
        /// <response code="401">This Request needs authentication.</response>
        /// <response code="403">The Requesting User is not authorized for this operation.</response>
        /// <response code="404">The Order does not exist.</response>
        /// <response code="409">The delivery date has been closed such that the Command cannot be accepted.</response>
        [HttpPut("{userId}/{deliveryDate}/incomplete")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationStatus.Reason[]), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ValidationStatus.Reason), 409)]
        public async Task<IActionResult> FlagAsIncomplete(string userId, string deliveryDate)
        {
            if (string.IsNullOrWhiteSpace(userId)
                || string.IsNullOrWhiteSpace(deliveryDate)
                || !deliveryDate.IsValidDeliveryDate())
            {
                return BadRequest(new[] { OrderCommandValidator.RouteDataFailsToValidate });
            }
            var order = await _orderService.GetOrderForRecipientAsync(
                recipientUserId: userId,
                forDeliveryDate: deliveryDate.ConvertToDeliveryDate()).ConfigureAwait(false);
            if (order == null)
            {
                return NotFound();
            }
            var command = new MarkOrderAsIncomplete
            {
                Id = order.Id
            };
            var approved = await _authorizationService.AuthorizeAsync(
                                    User,
                                    new OrderCommandAuthorizationResource(order, command),
                                    AuthorizationPolicies.WillAcceptOrderCommand).ConfigureAwait(false);
            if (!approved.Succeeded)
            {
                return Challenge();
            }
            var commandStatus = await _orderService.HandleForUserAsync(User, command).ConfigureAwait(false);
            if (commandStatus.AckNack == CommandStatus.AckNackStatus.ACK)
            {
                return NoContent();
            }
            var reason = commandStatus.Reasons.FirstOrDefault(r => r.Code == OrderCommandValidator.DeliveryDateUnavailableCode);
            if (reason != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, reason);
            }
            return BadRequest(commandStatus.Reasons);
        }
    }
}
