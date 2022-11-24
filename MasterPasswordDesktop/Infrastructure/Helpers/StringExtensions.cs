using System;

namespace MasterPasswordDesktop.Infrastructure.Helpers
{
    public static class StringExtensions
    {
        public static T ToEnumOfType<T>(this string str) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), str);
        }
    }

}
