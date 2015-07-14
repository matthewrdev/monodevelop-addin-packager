using System;
using Mono.TextEditor;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.Commands;
using MonoDevelop.Projects;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace MonoDevelopAddinPackager
{
	public class PackageProjectHandler : CommandHandler
	{

		// <ProjectTypeGuids>{86F6BF2A-E449-4B3E-813B-9ACC37E5545F};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>	

		public readonly string[] MONO_DEVELOP_ADDIN_PROJECT_GUIDS = new string[] {"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC", "86F6BF2A-E449-4B3E-813B-9ACC37E5545F"};
		public const string PROJECT_GUID_PROP_KEY = "ProjectTypeGuids";

		public const string MONODEVELOP_OSX_INSTALL_PATH = @"/Applications/Xamarin Studio.app/Contents/MacOS/";
		public const string MONODEVELOP_WINDOWS_INSTALL_PATH = @"C:\Program Files (x86)\Xamarin Studio\bin\";

		public const string MDTOOL_PACK_COMMAND = "setup pack {0} -d:{1}";
		
		protected override void Run ()
		{
			SolutionItem item = null;
			CanExecuteInContext (out item);

			using (var monitor = IdeApp.Workbench.ProgressMonitors.GetBuildProgressMonitor ()) {
				var buildResult = item.Build (monitor, IdeApp.Workspace.ActiveConfiguration);
				if (buildResult.Failed == false) {
					var project = item as Project;
					var outputPath = project.GetOutputFileName (IdeApp.Workspace.ActiveConfiguration);

					var name = DesktopService.PlatformName;



					string mdtoolPath = "";
					if (DesktopService.PlatformName == "OSX") {
						mdtoolPath = Path.Combine (MONODEVELOP_OSX_INSTALL_PATH, "mdtool");
					} else {
						mdtoolPath = Path.Combine (MONODEVELOP_OSX_INSTALL_PATH, "mdtool.exe");
					}

					ProcessStartInfo startInfo = new ProcessStartInfo();
					startInfo.Arguments = String.Format (MDTOOL_PACK_COMMAND, outputPath.ToString(), outputPath.ParentDirectory.ToString ());
					startInfo.UseShellExecute = false;
					startInfo.RedirectStandardOutput = true;
					startInfo.FileName = mdtoolPath;

					// TODO: Redirect out 

					Process process = new Process ();
					process.StartInfo = startInfo;
					process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => IdeApp.Workbench.StatusBar.ShowMessage (e.Data);
					process.Start ();

					if (process.WaitForExit (60000)) {
						DesktopService.OpenFolder (outputPath.ParentDirectory.ToString ());
					} else {
					}
				}
			}
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
			var binPath = Path.GetDirectoryName(Assembly.GetEntryAssembly ().Location);

			var files = Directory.GetFiles (binPath);

			SolutionItem item = null;
			info.Enabled = CanExecuteInContext (out item);
		}   
	}
}


