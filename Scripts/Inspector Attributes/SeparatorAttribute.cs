using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.InspectorAttributes
{
	public class SeparatorAttribute : PropertyAttribute
	{
		public readonly string title;
		public SeparatorAttribute()
		{
			this.title = "";
		}
		public SeparatorAttribute(string _title)
		{
			this.title = _title;
		}
	}
}

