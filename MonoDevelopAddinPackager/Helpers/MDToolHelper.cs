using System;
using MonoDevelop.Projects;
using MonoDevelop.Ide;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace MonoDevelopAddinPackager
{
	public class MDToolHelper
	{

		public static readonly string[] MONO_DEVELOP_ADDIN_PROJECT_GUIDS = new [] {"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC", "86F6BF2A-E449-4B3E-813B-9ACC37E5545F"};
		public static readonly string PROJECT_GUID_PROP_KEY = "ProjectTypeGuids";

		public static readonly string MDTOOL_PACK_COMMAND = "setup pack {0} -d:{1}";
		public static readonly string MDTOOL_MREP_BUILD__COMMAND = "setup rep-build {0}";

		public const string MREP_FILE_EXTENSION = ".mrep";
		public const string MPACK_FILE_EXTENSION = ".mpack";

		public const string ROOT_MREP_FILE = "root.mrep";
		public const string MAIN_MREP_FILE = "main.mrep";
		public const string MREP_INDEX_FILE = "index.html";

		public MDToolHelper ()
		{
		}

		public static bool ProjectHasBeenPackaged(SolutionItem item)
		{
			return CheckProjectPackageOfExtension (item, MPACK_FILE_EXTENSION);
		}

		public static bool ProjectHasBeenWebIndexed(SolutionItem item)
		{
			return CheckProjectPackageOfExtension (item, MREP_FILE_EXTENSION);
		}

		public static bool CheckProjectPackageOfExtension (SolutionItem item, string packageExtension)
		{
			var project = item as Project;
			var outputPath = project.GetOutputFileName (IdeApp.Workspace.ActiveConfiguration);
			var outputDirectory = outputPath.ParentDirectory.FullPath.ToString ();
			var files = Directory.GetFiles (outputDirectory);
			return files.Any (f => f.ToLower ().Contains (packageExtension));
		}

		public static bool PackageAddin (SolutionItem item, Action<string> statusCallback, out string outputDirectory)
		{
			outputDirectory = "";
			try
			{
				using (var monitor = IdeApp.Workbench.ProgressMonitors.GetBuildProgressMonitor ())
				{
					statusCallback("Building project...");
					var buildResult = item.Build (monitor, IdeApp.Workspace.ActiveConfiguration);
					if (buildResult.Failed == false)
					{
						statusCallback("Success!");
						var project = item as Project;
						var outputPath = project.GetOutputFileName (IdeApp.Workspace.ActiveConfiguration);

						outputDirectory = outputPath.ParentDirectory;

						var command = String.Format (MDTOOL_PACK_COMMAND, outputPath, outputPath.ParentDirectory);

						return ExecuteMDToolCommand (command, statusCallback);
					}
					else{
						statusCallback("Building project failed...");
						statusCallback(string.Join("\n\t", buildResult.Errors.Select(e => e.ErrorText)));
					}
				}

				return false;
			}
			catch (Exception ex)
			{
				statusCallback("A critical error occurred while generating the mpack for the project. " + ex.ToString ());
				statusCallback ("Please file a bug report at https://github.com/matthew-ch-robbins/monodevelop-addin-packager");
			}

			return false;
		}

		public static bool CreateWebIndexForAddin(string sourceDirectory, string outputDirectory, Action<string> statusCallback)
		{
			string commandTargetDirectory = BuildMrepCommandTargetDirectory (sourceDirectory, outputDirectory, statusCallback);

			string command = String.Format (MDTOOL_MREP_BUILD__COMMAND, commandTargetDirectory);

			if (ExecuteMDToolCommand (command, statusCallback)) {

				var di = new DirectoryInfo (commandTargetDirectory);

				var mreps = Directory.GetFiles (commandTargetDirectory, "*" + MREP_FILE_EXTENSION);
				var mpacks = Directory.GetFiles (commandTargetDirectory, "*" + MPACK_FILE_EXTENSION);

				var files = new List<string> ();
				files.AddRange (mreps);
				files.AddRange (mpacks);
				files.Add (Path.Combine(commandTargetDirectory, MREP_INDEX_FILE));
					
				statusCallback ("Moving files from '" + commandTargetDirectory + "' to '" + outputDirectory);
				foreach (var file in files) {
					var fi = new FileInfo (file);
					string newPath = Path.Combine (outputDirectory, fi.Name);
					statusCallback ("Moving '" + file + "' to '" + newPath + "'");
					File.Copy (file, newPath);
				}
			}

			return false;
		}

		public static string BuildMrepCommandTargetDirectory(string sourceDirectory, string outputDirectory, Action<string> statusCallback)
		{
			string commandTargetDirectory = sourceDirectory;
			if (sourceDirectory != outputDirectory) {
				commandTargetDirectory = Path.Combine (sourceDirectory, ".tmp.mrep");
				statusCallback ("Generating temporary working directory for packager '" + commandTargetDirectory + "'");
				if (Directory.Exists (commandTargetDirectory)) {
					Directory.Delete (commandTargetDirectory, true);
				}

				Directory.CreateDirectory (commandTargetDirectory);

				var mpacks = Directory.GetFiles (sourceDirectory, "*.mpack");
				foreach (var mpack in mpacks) {
					var fi = new FileInfo (mpack);
					string newPath = Path.Combine (commandTargetDirectory, fi.Name);
					File.Copy (mpack, newPath);
				}
			}

			return commandTargetDirectory;
		}

		public static bool ExecuteMDToolCommand (string command, Action<string> statusCallback)
		{
			statusCallback ("Resolving mdtool path...");
			string mdtoolPath = ResolveMDToolPath ();

			if (!File.Exists(mdtoolPath)) {
				statusCallback ("Failed to resolve mdtool executable.");
				statusCallback ("Please file a bug report at https://github.com/matthew-ch-robbins/monodevelop-addin-packager");
				return false;
			}

			statusCallback ("Found mdtool at " + mdtoolPath);


			statusCallback ("Executing " + command);
			ProcessStartInfo startInfo = new ProcessStartInfo ();
			startInfo.Arguments = command;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = true;
			startInfo.FileName = mdtoolPath;
			Process process = new Process ();
			process.StartInfo = startInfo;
			process.OutputDataReceived += (sender, e) => IdeApp.Workbench.StatusBar.ShowMessage (e.Data);
			process.Start ();
			if (!process.WaitForExit (60000))
			{
				statusCallback (".mpack generation timed out. File may or may not have been created.");
				return false;
			}
			statusCallback ("Command completely succesfully!");

			return true;
		}

		public static string ResolveMDToolPath()
		{
			return DesktopService.PlatformName == "OSX" ?  ResolveOsxMDToolPath () : ResolveWindowsMDToolPath ();
		}

		public static string ResolveOsxMDToolPath ()
		{
			var binPath = Path.GetDirectoryName(Assembly.GetEntryAssembly ().Location);
			var di = new DirectoryInfo (binPath);

			// All exes are in: "[/InstallPath]/Resources/lib/monodevelop/bin/"
			// We need to access mdtool in "/[InstallPath]/MacOS/" so step up 4 directories .
			var contentsPath = Path.Combine (di.Parent.Parent.Parent.Parent.FullName, "MacOS");

			return Path.Combine (contentsPath, "mdtool");
		}

		public static string ResolveWindowsMDToolPath ()
		{
			var binPath = Path.GetDirectoryName(Assembly.GetEntryAssembly ().Location);
			return Path.Combine (binPath, "mdtool.exe");
		}

	}
}

