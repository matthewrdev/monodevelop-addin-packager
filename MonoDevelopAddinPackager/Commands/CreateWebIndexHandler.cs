using System;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System.Diagnostics;
using MonoDevelop.Core;
using System.IO;

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

		public override bool Execute (SolutionItem solutionItem, out string message)
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
				string mrepOutputFolder = outputFolder;
				if (PropertyService.HasValue (Options.Keys.MREP_OUTPUT_FOLDER_KEY)) {
					mrepOutputFolder = PropertyService.Get (Options.Keys.MREP_OUTPUT_FOLDER_KEY, Options.Defaults.MREP_OUTPUT_FOLDER_DEFAULT);
					base.CommandStatus ("Ouput folder is configured to: " + mrepOutputFolder);
				} else {
					base.CommandStatus ("No output folder has been configured, using the build output folder");
				}

				if (MDToolHelper.CreateWebIndexForAddin (outputFolder, mrepOutputFolder, base.CommandStatus))
				{
					string mrepPath = Path.Combine (mrepOutputFolder, MDToolHelper.MAIN_MREP_FILE);
					OpenFileHelper.OpenAndSelect (mrepPath);
					return true;
				}
			}

			using (var scope = base.TaskScope ("Generate mrep for addin..."))
			{
				string mrepOutputFolder = outputFolder;
				if (PropertyService.HasValue (Options.Keys.MREP_OUTPUT_FOLDER_KEY)) {
					mrepOutputFolder = PropertyService.Get (Options.Keys.MREP_OUTPUT_FOLDER_KEY, Options.Defaults.MREP_OUTPUT_FOLDER_DEFAULT);
					base.CommandStatus ("Ouput folder is configured to: " + mrepOutputFolder);
				} else {
					base.CommandStatus ("No output folder has been configured, using the build output folder");
				}

				if (MDToolHelper.CreateWebIndexForAddin (outputFolder, mrepOutputFolder, base.CommandStatus))
				{
					string mrepPath = Path.Combine (mrepOutputFolder, MDToolHelper.MAIN_MREP_FILE);
					OpenFileHelper.OpenAndSelect (mrepPath);
					return true;
				}
			}

			return true;
		}
	}
}

