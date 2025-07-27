using System;

namespace Benito.ScriptingFoundations.NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FoldoutAttribute : MetaAttribute, IGroupAttribute
    {
        public string Name { get; private set; }

        public FoldoutAttribute(string name)
        {
            Name = name;
        }
    }
}
