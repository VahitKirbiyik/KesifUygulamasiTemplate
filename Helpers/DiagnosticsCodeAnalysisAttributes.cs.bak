// Disabled: framework provides these attributes on net8.0 and later.
// Keeping the file for reference but excluding from compilation to avoid duplicate/invalid attribute definitions.
#if !NET8_0
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, Inherited = false)]
    public sealed class ExcludeFromCodeCoverageAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class MemberNotNullAttribute : Attribute
    {
        public MemberNotNullAttribute(string member) { }
        public MemberNotNullAttribute(params string[] members) { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class MemberNotNullWhenAttribute : Attribute
    {
        public MemberNotNullWhenAttribute(bool returnValue, string member) { }
        public MemberNotNullWhenAttribute(bool returnValue, params string[] members) { }
    }
}
#endif
