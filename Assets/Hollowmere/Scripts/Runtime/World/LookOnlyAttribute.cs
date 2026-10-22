using System;

namespace Hollowmere
{
    /// <summary>
    /// Marks a component as purely visual (light flicker, colour grading): it never touches gameplay state, so the
    /// gameplay layout snapshot (LayoutFingerprint) leaves it out.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LookOnlyAttribute : Attribute { }
}
