using MaginusLunch.Core.Validation;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Repository;

namespace MaginusLunch.Orders.Service.Validators
{
    public class OrderCommandValidatorFactory : IValidateCommandsFactory<OrderAggregate>
    {
        private readonly OrderCommandValidator _orderCommandValidator;
        public OrderCommandValidatorFactory(ICalendarService calendarService,
            IOrderRepository orderRepository,
            IMenuService menuService)
        {
            _orderCommandValidator = new OrderCommandValidator(calendarService, orderRepository, menuService);
        }

        IValidateCommands<CommandType, OrderAggregate> IValidateCommandsFactory<OrderAggregate>.GetValidatorFor<CommandType>()
        {
            return _orderCommandValidator as IValidateCommands<CommandType, OrderAggregate>;
        }
    }
}
