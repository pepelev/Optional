// ReSharper disable CheckNamespace

namespace JetBrains.Annotations;
// ReSharper restore CheckNamespace

#if !(NET40_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER)
/// <summary>
///     https://www.jetbrains.com/help/resharper/Reference__Code_Annotation_Attributes.html#PureAttribute
/// </summary>
[AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Constructor |
    AttributeTargets.Method |
    AttributeTargets.Property |
    AttributeTargets.Event |
    AttributeTargets.Parameter |
    AttributeTargets.Delegate
)]
internal sealed class PureAttribute : Attribute
{
}
#endif
/// <summary>
///     https://www.jetbrains.com/help/resharper/Reference__Code_Annotation_Attributes.html#InstantHandleAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class InstantHandleAttribute : Attribute
{
}

/// <summary>
///     https://www.jetbrains.com/help/resharper/Reference__Code_Annotation_Attributes.html#LinqTunnelAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class LinqTunnelAttribute : Attribute
{
}

/// <summary>
///     https://www.jetbrains.com/help/resharper/Reference__Code_Annotation_Attributes.html#NoEnumerationAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class NoEnumerationAttribute : Attribute
{
}