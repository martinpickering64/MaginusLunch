namespace MaginusLunch.Orders.API.Authorization
{
    public class UserRetrievalAuthorizationResource
    {
        public UserRetrievalAuthorizationResource(string userId)
        {
            UserId = userId;
        }
        public string UserId { get; }
    }
}
