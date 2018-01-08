namespace MaginusLunch.Orders.API.Authorization
{
    /// <summary>
    /// Constants for Authorization Policy Names.
    /// </summary>
    public static class AuthorizationPolicies
    {
        /// <summary>
        /// This Policy ensures that the Requester may place orders.
        /// </summary>
        public const string CanAddOrder = "CanAddOrder";

        /// <summary>
        /// This Policy ensures that the Requester is allowed to send order commands.
        /// </summary>
        public const string WillAcceptOrderCommand = "WillAcceptOrderCommand";

        /// <summary>
        /// This Policy ensures that the Requester may access/retrieve Lunch Orders.
        /// </summary>
        public const string CanAccessOrders = "CanAccessOrders";
    }
}
