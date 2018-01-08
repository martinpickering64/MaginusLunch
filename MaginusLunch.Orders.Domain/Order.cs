using System;
using MaginusLunch.Core;
using MaginusLunch.Core.Entities;

namespace MaginusLunch.Orders.Domain
{
    public class Order : VersionedEntity
    {
        public Order(Guid id)
            : base(id)
        { }

        public string OrderingUserId { get; set; }
        public string OrderedBy { get; set; }
        public string RecipientUserId { get; set; }
        public string Recipient { get; set; }

        private DateTime _deliveryDateUtc;
        /// <summary>
        /// Delivery Date
        /// </summary>
        public DateTime DeliveryDate
        {
            get { return _deliveryDateUtc; }
            set
            {
                _deliveryDateUtc = value.Kind == DateTimeKind.Utc
                    ? value
                    : value.ToUniversalTime();
            }
        }
        public Guid LunchProviderId { get; set; }
        public string LunchProvider { get; set; }
        public Guid FillingId { get; set; }
        public string Filling { get; set; }
        public Guid BreadId { get; set; }
        public string Bread { get; set; }
        public Guid MenuOptionId { get; set; }
        public string MenuOption { get; set; }
        public OrderStatus Status { get; set; }

        public override object Clone()
        {
            Order clone = (Order)MemberwiseClone();
            return clone;
        }
    }
}
