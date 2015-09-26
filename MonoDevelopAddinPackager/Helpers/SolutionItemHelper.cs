using System;
using MonoDevelop.Projects;

namespace MonoDevelopAddinPackager
{
	public class SolutionItemHelper
	{
		public SolutionItemHelper ()
		{
		}


		public static bool  IsAddinProject (SolutionItem targettedItem)
		{
			if (targettedItem.ExtendedProperties.Contains (MDToolHelper.PROJECT_GUID_PROP_KEY) == false)
			{
				return false;
			}

			var guids = (string)targettedItem.ExtendedProperties [MDToolHelper.PROJECT_GUID_PROP_KEY];
			return guids.Contains (MDToolHelper.MONO_DEVELOP_ADDIN_PROJECT_GUIDS [0]) 
				&& guids.Contains (MDToolHelper.MONO_DEVELOP_ADDIN_PROJECT_GUIDS [1]);
		}
	}
}

