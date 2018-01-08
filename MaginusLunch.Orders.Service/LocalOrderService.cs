using MaginusLunch.Core.Extensions;
using MaginusLunch.Core.EventStore;
using MaginusLunch.Core.Messages.Commands;
using MaginusLunch.Core.Validation;
using MaginusLunch.Logging;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Messages.Commands;
using MaginusLunch.Orders.Repository;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MaginusLunch.Orders.Service
{
    public class LocalOrderService : IOrderService
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LocalOrderService));

        private readonly IOrderRepository _orderRepository;
        private readonly Core.EventStore.IRepository _eventStore;
        private readonly IValidateCommandsFactory<OrderAggregate> _validatorFactory;

        public LocalOrderService(
            IOrderRepository orderRepository,
            Core.EventStore.IRepository eventStore,
            IValidateCommandsFactory<OrderAggregate> validatorFactory)
        {
            _orderRepository = orderRepository;
            _eventStore = eventStore;
            _validatorFactory = validatorFactory;
        }

        #region IOrderService

        public Task<CommandStatus> HandleForUserAsync(ClaimsPrincipal user, AnOrderCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("Must pass in a ClaimsPrincipal");
            }
            var emailClaim = user.Claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim == null)
            {
                throw new ArgumentException($"The ClaimsPrincipal does not have an Email Claim; {0}.", 
                    user.Identity == null ? "anonymous user" : user.Identity.Name);
            }
            if (string.IsNullOrWhiteSpace(emailClaim.Value))
            {
                throw new ArgumentException($"The ClaimsPrincipal does not have a valid Email Claim as its null or empty; {0}.", user.Identity.Name);
            }
            command.OrderingUserId = emailClaim.Value.EmailAddressToUserId();
            var commandType = command.GetType();
            switch(commandType.Name)
            {
                case "AddOrder":
                    return HandleCommandAsync(command as AddOrder, cancellationToken);
                case "AddBreadToOrder":
                    return HandleCommandAsync(command as AddBreadToOrder, cancellationToken);
                case "RemoveBreadFromOrder":
                    return HandleCommandAsync(command as RemoveBreadFromOrder, cancellationToken);
                case "AddFillingToOrder":
                    return HandleCommandAsync(command as AddFillingToOrder, cancellationToken);
                case "RemoveFillingFromOrder":
                    return HandleCommandAsync(command as RemoveFillingFromOrder, cancellationToken);
                case "AddMenuOptionToOrder":
                    return HandleCommandAsync(command as AddMenuOptionToOrder, cancellationToken);
                case "RemoveMenuOptionFromOrder":
                    return HandleCommandAsync(command as RemoveMenuOptionFromOrder, cancellationToken);
                case "MarkOrderAsComplete":
                    return HandleCommandAsync(command as MarkOrderAsComplete, cancellationToken);
                case "MarkOrderAsIncomplete":
                    return HandleCommandAsync(command as MarkOrderAsIncomplete, cancellationToken);
                case "CancelOrder":
                    return HandleCommandAsync(command as CancelOrder, cancellationToken);
                default:
                    _logger.ErrorFormat("The Order Command received is of an unknown type: {0}", commandType.Name);
                    var returnStatus = new CommandStatus();
                    returnStatus.Reasons.Add(new ValidationStatus.Reason(403, $"Unknown Order Command received [{commandType.Name}]"));
                    return Task.FromResult(returnStatus);
            }
        }


        public Task<Order> GetOrderAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken)) 
            => _orderRepository.GetOrderAsync(id, cancellationToken);

        public Task<Order> GetOrderForRecipientAsync(string recipientUserId, DateTime forDeliveryDate, CancellationToken cancellationToken = default(CancellationToken)) 
            => _orderRepository.GetOrderForRecipientAsync(recipientUserId, forDeliveryDate, cancellationToken);

        public Task<IEnumerable<Order>> GetOrdersAsync(DateTime forDeliveryDate, CancellationToken cancellationToken = default(CancellationToken))
            => _orderRepository.GetOrdersAsync(forDeliveryDate, cancellationToken);

        #endregion IOrderService

        #region internal implementation

        public async Task<CommandStatus> HandleCommandAsync(AddOrder command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = Guid.NewGuid();
                _logger.WarnFormat("AddOrder Command received with no Identity. Going to use identity [{0}].", command.Id, command.RecipientUserId, command.DeliveryDate);
            }
            var newAggregate = new OrderAggregate(command);
            var validationStatus = await _validatorFactory.GetValidatorFor<AddOrder>().IsValidAsync(command, newAggregate).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(newAggregate, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        public async Task<CommandStatus> HandleCommandAsync(AddBreadToOrder command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<OrderAggregate>(command.Id);
            var validationStatus = await _validatorFactory.GetValidatorFor<AddBreadToOrder>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.AddBreadToOrder(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        public async Task<CommandStatus> HandleCommandAsync(RemoveBreadFromOrder command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<OrderAggregate>(command.Id);
            var validationStatus = await _validatorFactory.GetValidatorFor<RemoveBreadFromOrder>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.RemoveBreadFromOrder();
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        public async Task<CommandStatus> HandleCommandAsync(AddFillingToOrder command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<OrderAggregate>(command.Id);
            var validationStatus = await _validatorFactory.GetValidatorFor<AddFillingToOrder>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.AddFillingToOrder(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        public async Task<CommandStatus> HandleCommandAsync(RemoveFillingFromOrder command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<OrderAggregate>(command.Id);
            var validationStatus = await _validatorFactory.GetValidatorFor<RemoveFillingFromOrder>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.RemoveFillingFromOrder();
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        public async Task<CommandStatus> HandleCommandAsync(AddMenuOptionToOrder command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<OrderAggregate>(command.Id);
            var validationStatus = await _validatorFactory.GetValidatorFor<AddMenuOptionToOrder>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.AddMenuOptionToOrder(command);
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        public async Task<CommandStatus> HandleCommandAsync(RemoveMenuOptionFromOrder command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<OrderAggregate>(command.Id);
            var validationStatus = await _validatorFactory.GetValidatorFor<RemoveMenuOptionFromOrder>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.RemoveMenuOptionFromOrder();
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        public async Task<CommandStatus> HandleCommandAsync(MarkOrderAsComplete command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<OrderAggregate>(command.Id);
            var validationStatus = await _validatorFactory.GetValidatorFor<MarkOrderAsComplete>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.MarkOrderAsCompleted();
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        public async Task<CommandStatus> HandleCommandAsync(MarkOrderAsIncomplete command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<OrderAggregate>(command.Id);
            var validationStatus = await _validatorFactory.GetValidatorFor<MarkOrderAsIncomplete>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.MarkOrderAsIncomplete();
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        public async Task<CommandStatus> HandleCommandAsync(CancelOrder command, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _eventStore.GetByIdAsync<OrderAggregate>(command.Id);
            var validationStatus = await _validatorFactory.GetValidatorFor<CancelOrder>().IsValidAsync(command, aggregateRoot).ConfigureAwait(false);
            if (!validationStatus.IsValid)
            {
                return new CommandStatus(validationStatus);
            }
            aggregateRoot.CancelOrder();
            var commitId = Guid.NewGuid();
            await _eventStore.SaveAsync(aggregateRoot, commitId).ConfigureAwait(false);
            return CommandStatus.CommandOk;
        }

        #endregion internal implementation
    }
}
