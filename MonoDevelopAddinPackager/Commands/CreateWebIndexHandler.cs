using System;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace MonoDevelopAddinPackager
{
	public class CreateWebIndexHandler : HandlerBase
	{
		public CreateWebIndexHandler ()
		{
		}

		public override int MaxStatusSteps
		{
			get
			{
				return 5;
			}
		}

		public override string CommandName
		{
			get
			{
				return "Package Addin (.mrep)";
			}
		}

		public override bool Execute (MonoDevelop.Projects.SolutionItem solutionItem, out string message)
		{
			message = "";

			string outputFolder = "";

			using (var scope = base.TaskScope ("Preparing project for packaging..."))
			{
				var project = solutionItem as Project;
				var outputPath = project.GetOutputFileName (IdeApp.Workspace.ActiveConfiguration);

				outputFolder = outputPath.ParentDirectory;
			}

			bool isPackaged;
			using (var scope = base.TaskScope ("Checking for exisiting package..."))
			{
				isPackaged = MDToolHelper.ProjectHasBeenPackaged (solutionItem);
			}

			if (!isPackaged) {
				using (var scope = base.TaskScope ("Generating mpack for addin..."))
				{
					if (!MDToolHelper.PackageAddin (solutionItem, base.CommandStatus, out outputFolder))
					{
						message = "Cannot generate mrep web index files for the current project, mpack generation failed.";
						return false;
					}
				}
			}

			using (var scope = base.TaskScope ("Generate mrep for addin..."))
			{
				if (MDToolHelper.CreateWebIndexForAddin (outputFolder, base.CommandStatus))
				{
					DesktopService.OpenFolder (outputFolder);
				}
			}

			return true;
		}
	}
}

