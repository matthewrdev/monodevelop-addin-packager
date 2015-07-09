using System;
using Mono.TextEditor;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.Commands;
using MonoDevelop.Projects;
using System.Linq;

namespace MonoDevelopAddinPackager
{
	public class PackageProjectHandler : CommandHandler
	{

		// <ProjectTypeGuids>{86F6BF2A-E449-4B3E-813B-9ACC37E5545F};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>	

		public readonly string[] MONO_DEVELOP_ADDIN_PROJECT_GUIDS = new string[] {"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC", "86F6BF2A-E449-4B3E-813B-9ACC37E5545F"};
		public const string PROJECT_GUID_PROP_KEY = "ProjectTypeGuids";
		
		protected override void Run ()
		{
			SolutionItem item = null;
			CanExecuteInContext (out item);

			var buildResult = item.Build (IdeApp.Workbench.ProgressMonitors.GetBuildProgressMonitor (), IdeApp.Workspace.ActiveConfiguration);
			if (buildResult.Failed == false) {
				var project = item as Project;
				var outputPath = project.GetOutputFileName (IdeApp.Workspace.ActiveConfiguration);

				// Throw this path at out packager.
				
			}

//			if (CanExecuteInContext (out item)) {
//				var buildResult = item.Build (IdeApp.Workbench.ProgressMonitors.GetBuildProgressMonitor (), IdeApp.Workspace.ActiveConfiguration);
//				if (buildResult.Failed == false) {
//					// Get configuratyion output and execute the mdtool command.
//					buildResult.
//
//				}
//			}
		}

		protected bool CanExecuteInContext(out SolutionItem targettedItem)
		{
			targettedItem = null;
			var workspace = IdeApp.Workspace;

			if (String.IsNullOrEmpty (workspace.ActiveConfigurationId)) {
				return false;
			}

			ConfigurationSelector activeConfig = workspace.ActiveConfiguration;
			Console.WriteLine (activeConfig.GetType ());

			Solution activeSolution = null;
			var items = workspace.Items.ToArray();
			foreach (var i in items) {
				if (i.GetType () == typeof(Solution)) {
					activeSolution = i as Solution;
					break;
				}
			}

			if (activeSolution == null) {
				return false;
			}

			var activeItemId = activeSolution.StartupItem.ItemId;

			targettedItem = activeSolution.Items.FirstOrDefault(si => si.ItemId == activeSolution.StartupItem.ItemId);
			if (targettedItem == null) {
				return false;
			}

			if (targettedItem is Project == false) {
			}

			if (targettedItem.ExtendedProperties.Contains ("ProjectTypeGuids") == false) {
				return false;
			}

			var guids = (string)targettedItem.ExtendedProperties ["ProjectTypeGuids"];
			return guids.Contains (MONO_DEVELOP_ADDIN_PROJECT_GUIDS [0]) && guids.Contains (MONO_DEVELOP_ADDIN_PROJECT_GUIDS [1]);
		}

		protected override void Update (CommandInfo info)
		{
			SolutionItem item = null;
			//info.Enabled = CanExecuteInContext (out item);
			info.Enabled = true;
		}   
	}
}


