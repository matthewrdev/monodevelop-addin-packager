using System;

namespace MonoDevelopAddinPackager
{
	public static class Options
	{
		public static class Keys
		{
			public const string MREP_OUTPUT_FOLDER_KEY = "MonoDevelopAddinPackager.mrep_output_folder";
			public const string DEPLOY_SCRIPT_KEY = "MonoDevelopAddinPackager.deploy_script";
		}

		public static class Defaults
		{
			public const string MREP_OUTPUT_FOLDER_DEFAULT = "";
			public const string DEPLOY_SCRIPT_DEFAULT = "";
		}
	}
}

