using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace MonoDevelopAddinPackager
{
	public class PackageProjectHandler : CommandHandler
	{
		// <ProjectTypeGuids>{86F6BF2A-E449-4B3E-813B-9ACC37E5545F};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>	
		public readonly string[] MONO_DEVELOP_ADDIN_PROJECT_GUIDS = new [] {"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC", "86F6BF2A-E449-4B3E-813B-9ACC37E5545F"};
		public const string PROJECT_GUID_PROP_KEY = "ProjectTypeGuids";

		public const string MDTOOL_PACK_COMMAND = "setup pack {0} -d:{1}";
		
		protected override void Run ()
		{
			SolutionItem item = GetTargettedSolutionItem ();

			if (item == null) {
				Console.WriteLine ("Can't process project because no solution items are active.");
				return;
			}

			try {
				using (var monitor = IdeApp.Workbench.ProgressMonitors.GetBuildProgressMonitor ()) {
					var buildResult = item.Build (monitor, IdeApp.Workspace.ActiveConfiguration);
					if (buildResult.Failed == false) {
						var project = item as Project;
						var outputPath = project.GetOutputFileName (IdeApp.Workspace.ActiveConfiguration);

						string mdtoolPath = "";
						if (DesktopService.PlatformName == "OSX") {
							mdtoolPath = ResolveOsxMDToolPath ();
						} else {
							mdtoolPath = ResolveWindowsMDToolPath ();
						}

						ProcessStartInfo startInfo = new ProcessStartInfo();
						startInfo.Arguments = String.Format (MDTOOL_PACK_COMMAND, outputPath, outputPath.ParentDirectory);
						startInfo.UseShellExecute = false;
						startInfo.RedirectStandardOutput = true;
						startInfo.FileName = mdtoolPath;

						Process process = new Process ();
						process.StartInfo = startInfo;
						process.OutputDataReceived += (sender, e) => IdeApp.Workbench.StatusBar.ShowMessage (e.Data);
						process.Start ();

						if (process.WaitForExit (60000)) {
							DesktopService.OpenFolder (outputPath.ParentDirectory.ToString ());
						} else {
							Console.WriteLine(".mpack generation took too long");
						}
					}
				}
			}
			catch (Exception ex) {
				Console.WriteLine ("A critical error occurred while generating the mpack for the project." + ex.ToString());
			}
		}

		string ResolveOsxMDToolPath ()
		{
			var binPath = Path.GetDirectoryName(Assembly.GetEntryAssembly ().Location);
			var di = new DirectoryInfo (binPath);

			// All exes are in: "[/InstallPath]/Resources/lib/monodevelop/bin/"
			// We need to access mdtool in "/[InstallPath]/MacOS/" so step up 4 directories .
			var contentsPath = Path.Combine (di.Parent.Parent.Parent.Parent.FullName, "MacOS");

			return Path.Combine (contentsPath, "mdtool");
		}

		string ResolveWindowsMDToolPath ()
		{
			var binPath = Path.GetDirectoryName(Assembly.GetEntryAssembly ().Location);
			return Path.Combine (binPath, "mdtool.exe");
		}

		protected bool CanExecuteInContext()
		{
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

			if (activeSolution.StartupItem == null) {
				return false;
			}

			var activeItemId = activeSolution.StartupItem.ItemId;

			var targettedItem = GetTargettedSolutionItem ();
			if (targettedItem == null) {
				return false;
			}

			if (targettedItem.ExtendedProperties.Contains (PROJECT_GUID_PROP_KEY) == false) {
				return false;
			}

			var guids = (string)targettedItem.ExtendedProperties [PROJECT_GUID_PROP_KEY];
			return guids.Contains (MONO_DEVELOP_ADDIN_PROJECT_GUIDS [0]) && guids.Contains (MONO_DEVELOP_ADDIN_PROJECT_GUIDS [1]);
		}

		protected SolutionItem GetTargettedSolutionItem()
		{
			var workspace = IdeApp.Workspace;

			if (String.IsNullOrEmpty (workspace.ActiveConfigurationId)) {
				return null;
			}

			Solution activeSolution = null;
			var items = workspace.Items.ToArray();
			foreach (var i in items) {
				if (i.GetType () == typeof(Solution)) {
					activeSolution = i as Solution;
					break;
				}
			}

			if (activeSolution.StartupItem == null) {
				return null;
			}

			return activeSolution.Items.FirstOrDefault(si => si.ItemId == activeSolution.StartupItem.ItemId);
		}

		protected override void Update (CommandInfo info)
		{
			info.Enabled = CanExecuteInContext ();
		}   
	}
}
