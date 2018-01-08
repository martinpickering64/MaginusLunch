namespace MaginusLunch.Core.AspNet.Authorization
{
    /// <summary>
    /// Constants for Authorization Policy Names.
    /// </summary>
    public static class AuthorizationPolicies
    {
        /// <summary>
        /// This Policy ensures that the Requester is considered to be a Maginus Employee.
        /// </summary>
        public const string IsMaginusEmployee = "IsMaginusEmployee";
    }
}
