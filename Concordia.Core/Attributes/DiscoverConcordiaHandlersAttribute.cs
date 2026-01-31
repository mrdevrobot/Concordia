using System;

namespace Concordia.Attributes
{
    /// <summary>
    /// Enables automated discovery of Concordia handlers (IRequestHandler, INotificationHandler) in the current assembly.
    /// The Source Generator will also scan referenced assemblies for other discovery attributes to chain registrations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class DiscoverConcordiaHandlersAttribute : Attribute
    {
    }
}
