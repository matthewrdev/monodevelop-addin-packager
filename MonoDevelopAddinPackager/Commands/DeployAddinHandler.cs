using System;
using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using System.IO;
using System.Diagnostics;

namespace MonoDevelopAddinPackager
{
	public class DeployAddinHandler : HandlerBase
	{
		public DeployAddinHandler ()
		{
		}

		public override string CommandName {
			get {
				return "Deploy Addin";
			}
		}

		public override bool Execute (MonoDevelop.Projects.SolutionItem solutionItem, out string message)
		{			
			message = "";
			string outputFolder = "";

			using (var scope = TaskScope ("Verifying deployment script and output folder settings are configured..")) {
				if (PropertyService.HasValue (Options.Keys.DEPLOY_SCRIPT_KEY) == false) {
					message = "Deployment script is not configured. Please open MonoDevelop Addin Packager settings in the Preferences dialog and configure";
					return false;
				}

				if (PropertyService.HasValue (Options.Keys.DEPLOY_SCRIPT_KEY) == false) {
					message = "Output directory is not configured. Please open MonoDevelop Addin Packager settings in the Preferences dialog and configure";
					return false;
				}
			}

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

			string mrepOutputFolder = PropertyService.Get (Options.Keys.MREP_OUTPUT_FOLDER_KEY, Options.Defaults.MREP_OUTPUT_FOLDER_DEFAULT);
			using (var scope = base.TaskScope ("Generate mrep for addin..."))
			{
				if (MDToolHelper.CreateWebIndexForAddin (outputFolder, mrepOutputFolder, base.CommandStatus))
				{
					string mrepPath = Path.Combine (mrepOutputFolder, MDToolHelper.MAIN_MREP_FILE);
					OpenFileHelper.OpenAndSelect (mrepPath);
				}
			}

			using (var scope = base.TaskScope ("Deploying addin..."))
			{
				var script = PropertyService.Get (Options.Keys.DEPLOY_SCRIPT_KEY, Options.Defaults.DEPLOY_SCRIPT_DEFAULT);
				CommandStatus ("Using deployment script '" + script + "'");

				Process.Start(scru
				var proc = new Process();
				proc.StartInfo.FileName = gitPath;
				proc.StartInfo.WorkingDirectory = new FileInfo(Host.TemplateFile).Directory.FullName;
				proc.StartInfo.Arguments = iArguments; 
				proc.StartInfo.UseShellExecute = false;
				proc.StartInfo.RedirectStandardOutput = true; 
				proc.StartInfo.RedirectStandardError = true;

				proc.Start();		 	
				proc.WaitForExit();			

				oExitCode = proc.ExitCode;
				result = proc.StandardOutput.ReadToEnd().Trim();
			}

			return false;
		}

		public override int MaxStatusSteps {
			get {
				return 7;
			}
		}
	}
}

