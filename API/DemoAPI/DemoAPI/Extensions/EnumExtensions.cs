using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace TrexDino.Extensions
{
    public static class EnumExtensions
    {
        private static readonly ConcurrentDictionary<System.Enum, MemberInfo> EnumMemberInfoCache = new ConcurrentDictionary<System.Enum, MemberInfo>();

        public static T ToEnumIgnoreCase<T>(this string enumString, T defaultValue = default(T)) where T : struct
        {
            return System.Enum.TryParse(enumString, true, out T value)
                ? value
                : defaultValue;
        }
    }
}