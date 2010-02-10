using System;
using Fasterflect.Emitter;

namespace Fasterflect
{
    internal static class Constants
    {
        public const string IndexerSetterName = "set_Item";
        public const string IndexerGetterName = "get_Item";
        public const string ArraySetterName = "[]=";
        public const string ArrayGetterName = "=[]";
        public static readonly Type ObjectType = typeof(object);
        public static readonly Type IntType = typeof(int);
        public static readonly Type StructType = typeof(ValueTypeHolder);
        public static readonly Type VoidType = typeof(void);
        public static readonly Type[] ArrayOfObjectType = new[] { typeof(object) };
        public static readonly object[] EmptyObjectArray = new object[0];
        public static readonly string[] EmptyStringArray = new string[0];
    }
}
