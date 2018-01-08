using MaginusLunch.Core.Validation;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Messages.Commands;
using MaginusLunch.Orders.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Service.Validators
{
    public class OrderCommandValidator : CommandValidatorBase,
        IValidateCommands<AddOrder, OrderAggregate>,
        IValidateCommands<AddBreadToOrder, OrderAggregate>,
        IValidateCommands<RemoveBreadFromOrder, OrderAggregate>,
        IValidateCommands<AddFillingToOrder, OrderAggregate>,
        IValidateCommands<RemoveFillingFromOrder, OrderAggregate>,
        IValidateCommands<AddMenuOptionToOrder, OrderAggregate>,
        IValidateCommands<RemoveMenuOptionFromOrder, OrderAggregate>,
        IValidateCommands<MarkOrderAsComplete, OrderAggregate>,
        IValidateCommands<MarkOrderAsIncomplete, OrderAggregate>,
        IValidateCommands<CancelOrder, OrderAggregate>
    {
        public const int RecipientUserIdNullCode = 103;
        public const int OrderingUserIdNullCode = 104;
        public const int DeliveryDateUnavailableCode = 105;
        public const int OrderExistsCode = 108;
        public const int OrderDoesNotExistCode = 109;
        public const int BreadNotAllowedCode = 120;
        public const int BreadUnavailableCode = 121;
        public const int MenuOptionNotAllowedCode = 130;
        public const int MenuOptionUnavailableCode = 131;
        public const int FillingUnavailableCode = 140;

        public static ValidationStatus.Reason BreadNotAllowed = new ValidationStatus.Reason(BreadNotAllowedCode, "The selected Filling does not allow Bread to be added to the Order as well.");
        public static ValidationStatus.Reason BreadUnavailable = new ValidationStatus.Reason(BreadUnavailableCode, "The selected Bread is no longer avaialble.");
        public static ValidationStatus.Reason MenuOptionNotAllowed = new ValidationStatus.Reason(MenuOptionNotAllowedCode, "The selected Filling does not allow Menu Options to be added to the Order as well, or the selected Menu Option is no longer available.");
        public static ValidationStatus.Reason MenuOptionUnavailable = new ValidationStatus.Reason(MenuOptionUnavailableCode, "The selected Menu Option is no longer avaialble.");
        public static ValidationStatus.Reason FillingUnavailable = new ValidationStatus.Reason(FillingUnavailableCode, "The selected Filling is no longer available.");


        private readonly ICalendarService _calendarService;
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuService _menuService;

        public OrderCommandValidator(ICalendarService calendarService, IOrderRepository orderRepository, IMenuService menuService)
        {
            _calendarService = calendarService;
            _orderRepository = orderRepository;
            _menuService = menuService;
        }

        async Task<ValidationStatus> IValidateCommands<AddOrder, OrderAggregate>.IsValidAsync(AddOrder command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = new ValidationStatus();
            if (command.Id != aggregate.Id)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateRootIdInvalidCode,
                    $"The Order Id for the Command ({command.Id}) does not match with Order Id for the Order ({aggregate.Id}). Contact Support!"));
                return returnStatus;
            }
            if (aggregate.Version > 0)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateVersionInvalidCode,
                    $"Cannot create the same OrderAggregate more than once. Version attempted was {aggregate.Version}."));
            }
            if (string.IsNullOrWhiteSpace(command.RecipientUserId))
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(RecipientUserIdNullCode,
                    "The AddOrder Command must have a RecipientUserId."));
            }
            if (string.IsNullOrWhiteSpace(command.OrderingUserId))
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(OrderingUserIdNullCode,
                    "The AddOrder Command must have a OrderingUserId."));
            }
            if (!returnStatus.IsValid)
            {
                return returnStatus;
            }
            if (!(await _calendarService.IsAvailableAsync(command.DeliveryDate, cancellationToken).ConfigureAwait(false)))
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(DeliveryDateUnavailableCode,
                    $"The requested Delivery Date of {command.DeliveryDate.Date} is no longer available."));
            }
            else if ((await _orderRepository.GetOrderAsync(command.Id, cancellationToken).ConfigureAwait(false)) != null)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateWithIdExistsCode,
                    $"An Order for that Id already exists; Id =[{command.Id}]."));
            }
            else
            {
                var order = await _orderRepository.GetOrderForRecipientAsync(command.RecipientUserId, command.DeliveryDate, cancellationToken).ConfigureAwait(false);
                if (order != null)
                {
                    returnStatus.Reasons.Add(new ValidationStatus.Reason(OrderExistsCode,
                        $"An Order for [{command.RecipientUserId}] on [{command.DeliveryDate}] already exists; Id =[{order.Id}]."));
                }
            }
            return returnStatus;
        }

        Task<ValidationStatus> IValidateCommands<CancelOrder, OrderAggregate>.IsValidAsync(CancelOrder command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            return CommonValidation(command, aggregate, cancellationToken);
        }

        async Task<ValidationStatus> IValidateCommands<AddBreadToOrder, OrderAggregate>.IsValidAsync(AddBreadToOrder command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = await CommonValidation(command, aggregate, cancellationToken).ConfigureAwait(false);
            if (! returnStatus.IsValid) { return returnStatus; }
            if (!await _menuService.IsBreadAvailableAsync(command.BreadId, aggregate.DeliveryDate, cancellationToken).ConfigureAwait(false))
            {
                returnStatus.Reasons.Add(BreadUnavailable);
            }
            if (aggregate.FillingId != Guid.Empty
                && !await _menuService.CanHaveBreadAsync(aggregate.FillingId, cancellationToken).ConfigureAwait(false))
            {
                returnStatus.Reasons.Add(BreadNotAllowed);
            }
            return returnStatus;
        }

        Task<ValidationStatus> IValidateCommands<RemoveBreadFromOrder, OrderAggregate>.IsValidAsync(RemoveBreadFromOrder command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            return CommonValidation(command, aggregate, cancellationToken);
        }

        async Task<ValidationStatus> IValidateCommands<AddFillingToOrder, OrderAggregate>.IsValidAsync(AddFillingToOrder command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = await CommonValidation(command, aggregate, cancellationToken).ConfigureAwait(false);
            if (!returnStatus.IsValid) { return returnStatus; }
            if (!await _menuService.IsFillingAvailableAsync(command.FillingId, aggregate.DeliveryDate, cancellationToken).ConfigureAwait(false))
            {
                returnStatus.Reasons.Add(FillingUnavailable);
            }
            return returnStatus;
        }

        Task<ValidationStatus> IValidateCommands<RemoveFillingFromOrder, OrderAggregate>.IsValidAsync(RemoveFillingFromOrder command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            return CommonValidation(command, aggregate, cancellationToken);
        }

        async Task<ValidationStatus> IValidateCommands<AddMenuOptionToOrder, OrderAggregate>.IsValidAsync(AddMenuOptionToOrder command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = await CommonValidation(command, aggregate, cancellationToken).ConfigureAwait(false);
            if (!returnStatus.IsValid) { return returnStatus; }
            if (!await _menuService.IsMenuOptionAvailableAsync(command.MenuOptionId, aggregate.DeliveryDate, cancellationToken).ConfigureAwait(false))
            {
                returnStatus.Reasons.Add(MenuOptionUnavailable);
            }
            if (aggregate.FillingId != Guid.Empty
                && !await _menuService.CanHaveMenuOptionAsync(aggregate.FillingId, cancellationToken).ConfigureAwait(false))
            {
                returnStatus.Reasons.Add(MenuOptionNotAllowed);
            }
            return returnStatus;
        }

        Task<ValidationStatus> IValidateCommands<RemoveMenuOptionFromOrder, OrderAggregate>.IsValidAsync(RemoveMenuOptionFromOrder command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            return CommonValidation(command, aggregate, cancellationToken);
        }

        Task<ValidationStatus> IValidateCommands<MarkOrderAsComplete, OrderAggregate>.IsValidAsync(MarkOrderAsComplete command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            return CommonValidation(command, aggregate, cancellationToken);
        }

        Task<ValidationStatus> IValidateCommands<MarkOrderAsIncomplete, OrderAggregate>.IsValidAsync(MarkOrderAsIncomplete command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            return CommonValidation(command, aggregate, cancellationToken);
        }

        public async Task<ValidationStatus> CommonValidation(AnOrderCommand command, OrderAggregate aggregate, CancellationToken cancellationToken)
        {
            var returnStatus = new ValidationStatus();
            if (aggregate == null)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateWithIdDoesNotExistCode,
                    $"An Order for that Id does not exist; Id =[{command.Id}]."));
                return returnStatus;
            }
            if (command.Id != aggregate.Id)
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(AggregateRootIdInvalidCode,
                    $"The Order Id for the Command ({command.Id}) does not match with Order Id for the Order ({aggregate.Id}). Contact Support!"));
                return returnStatus;
            }
            if (!(await _calendarService.IsAvailableAsync(aggregate.DeliveryDate, cancellationToken).ConfigureAwait(false)))
            {
                returnStatus.Reasons.Add(new ValidationStatus.Reason(DeliveryDateUnavailableCode,
                    $"Changes for the Delivery Date of {aggregate.DeliveryDate.Date} can no longer be accepted."));
            }
            return returnStatus;
        }

    }
}
