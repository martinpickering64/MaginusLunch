using System;
using System.Collections;

namespace MaginusLunch.Logging
{
    public static class Guard
    {
        public static void AgainstNull(string argumentName, object value)
        {
            if (value == null)
                throw new ArgumentNullException(argumentName);
        }

        public static void AgainstNullAndEmpty(string argumentName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(argumentName);
        }

        public static void AgainstNullAndEmpty(string argumentName, ICollection value)
        {
            if (value == null)
                throw new ArgumentNullException(argumentName);
            if (value.Count == 0)
                throw new ArgumentOutOfRangeException(argumentName);
        }
    }
}
