using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System.Diagnostics;

namespace MonoDevelopAddinPackager
{
	public class PackageProjectHandler : HandlerBase
	{
		public override int MaxStatusSteps
		{
			get
			{
				return 3;
			}
		}

		public override string CommandName
		{
			get
			{
				return "Package Addin (.mpack)";
			}
		}

		public override bool Execute (SolutionItem solutionItem, out string message)
		{
			using (var scope = TaskScope ("Generating mpac for addin..."))
			{
				message = "";
				string outputDirectory = "";
				if (MDToolHelper.PackageAddin (solutionItem, base.CommandStatus, out outputDirectory))
				{
					Process.Start (outputDirectory);
				}

				else
				{
					message = "Failed to package addin.";
					return false;
				}
			}

			message = "Addin was packaged succesfully!";

			return true;
		} 
	}
}
