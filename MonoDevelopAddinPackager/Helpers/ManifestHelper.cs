using System;
using System.Linq;
using MonoDevelop.Projects;
using Gtk;
using MonoDevelop.Ide;
using System.Collections.Generic;
using System.IO;

namespace MonoDevelopAddinPackager
{
	public class ManifestHelper
	{
		public ManifestHelper ()
		{
		}

		public const string MANIFEST_IDENTIFIER = ".addin";
		public const string MANIFEST_EXTENSION_SUFFIX = ".addin.xml";

		public const string DEFAULT_MANIFEST_FILE_CONTENTS = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<ExtensionModel>\n\n\t<!-- TODO: Define your addin components here. See http://www.monodevelop.com/archived/developers/articles/obsolete-articles/writing-an-add-in/ to get started. -->\n\n</ExtensionModel>";

		public static string ManifestFileForProject(Project project)
		{
			var manifest = ManifestItemForProject (project);

			if (manifest == null) {
				return null;
			}

			return manifest.FilePath.ToString ();
		}

		public static ProjectFile ManifestItemForProject(Project project)
		{
			var files = project.Items.OfType<ProjectFile> ();
			var items = files.Where(p => p.BuildAction == "EmbeddedResource" && p.Name.Contains(MANIFEST_IDENTIFIER)).ToList ();

			return items.FirstOrDefault();
		}

		public static void GenerateEmptyManifestForProject(Project project)
		{
			var fi = new FileInfo (project.FileName);
			string outputPath = Path.Combine (fi.Directory.FullName, "Manifest" + MANIFEST_EXTENSION_SUFFIX);

			File.WriteAllText (outputPath, DEFAULT_MANIFEST_FILE_CONTENTS);
			project.AddFile (outputPath, "EmbeddedResource");
		}

		public static bool CheckIfNeedsManifestEntry (SolutionItem item)
		{
			var project = item as Project;
			if (project != null && SolutionItemHelper.IsAddinProject (project)) {
				var manifest = ManifestHelper.ManifestItemForProject (project);
				return manifest == null;
			}
			return false;
		}

		public static void PromptForManifestGeneration(SolutionItem[] items)
		{
			var projects = items.OfType<Project> ().ToList ();
			if (projects.Any () == false) {
				return;
			}

			string message = "";
			if (projects.Count > 1) {
				message = "The projects '" + string.Join(", ", projects.Select(p => p.Name)) + "' are MonoDevelop addins but do not have manifests.\n\nGenerate them now?";
			} else {
				message = "The project'" + projects[0].Name + "' is a MonoDevelop addin but it does not have a manifest.\n\nGenerate one now?";
			}

			var d = new MessageDialog (IdeApp.Workbench.RootWindow, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, message);
			if (d.Run () == (int)ResponseType.Yes) {
				foreach (var p in projects) {
					GenerateEmptyManifestForProject (p);
				}
			}
			d.Hide ();
			d.Dispose ();
		}

		public static void PromptForManifestGeneration(List<SolutionItem> items)
		{
			PromptForManifestGeneration (items.ToArray ());
		}
	}
}

