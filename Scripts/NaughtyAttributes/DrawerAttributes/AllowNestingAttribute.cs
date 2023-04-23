using System;

namespace Benito.ScriptingFoundations.NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AllowNestingAttribute : DrawerAttribute
    {
    }
}
