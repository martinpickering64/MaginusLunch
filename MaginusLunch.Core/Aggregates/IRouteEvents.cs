namespace MaginusLunch.Core.Aggregates
{
    /// <summary>
    /// Event Router used to register Aggregates and 
    /// to dispatch Events to the Aggregates that 
    /// have been registered.
    /// </summary>
    public interface IRouteEvents
    {
        /// <summary>
        /// Register an Aggregate instance that will receive Events.
        /// </summary>
        /// <param name="aggregate"></param>
        void Register(IAggregate aggregate);

        /// <summary>
        /// Dispatch an Event to the registered Aggregates.
        /// </summary>
        /// <param name="anEvent"></param>
        void Dispatch(object anEvent);
    }
}
