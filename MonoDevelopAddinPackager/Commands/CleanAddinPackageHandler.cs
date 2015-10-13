using System;
using MonoDevelop.Projects;
using MonoDevelop.Ide;
using System.IO;
using System.Diagnostics;

namespace MonoDevelopAddinPackager
{
	public class CleanAddinPackageHandler : HandlerBase
	{
		public CleanAddinPackageHandler ()
		{
		}
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
				return "Clean Addin Packages";
			}
		}

		public override bool Execute (MonoDevelop.Projects.SolutionItem solutionItem, out string message)
		{
			message = "";

			var project = solutionItem as Project;
			var outputPath = project.GetOutputFileName (IdeApp.Workspace.ActiveConfiguration);

			string mpackOutputFolder = outputPath.ParentDirectory;

			using (var scope = base.TaskScope("Deleting mpack files..."))
			{
				foreach (var f in Directory.GetFiles (mpackOutputFolder, "*" + MDToolHelper.MPACK_FILE_EXTENSION))
				{ 
					File.Delete (f);
				}
			}

			using (var scope = base.TaskScope ("Deleting mrep files..."))
			{
				foreach (var f in Directory.GetFiles (mpackOutputFolder, "*" + MDToolHelper.MREP_FILE_EXTENSION))
				{ 
					File.Delete (f);
				}
			}

			CommandStatus ("Done!");

			return true;
		}
	}
}

