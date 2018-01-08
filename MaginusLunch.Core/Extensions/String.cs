namespace MaginusLunch.Core.Extensions
{
    public static class Strings
    {
        /// <summary>
        /// converts an email address into a Maginus Lunch Orders UserId
        /// </summary>
        /// <param name="emailAddress">the email address</param>
        /// <returns>the userId</returns>
        public static string EmailAddressToUserId(this string emailAddress)
        {
            return emailAddress.Substring(0, emailAddress.IndexOf('@'));
        }
    }
}
