using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.MonoDroid;
using MonoDevelop.Projects;
using MonoDevelop.Core;
using Mono.Addins;
using MonoDevelop.Core.Setup;
using MonoDevelop.Ide.Gui.Dialogs;
using Gtk;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MonoDevelopAddinPackager
{
	public class StartupHandler : CommandHandler
	{
		protected override void Run ()
		{
			IdeApp.Exiting += this.OnApplicationExiting;

			Setup ();

			base.Run ();
		}

		void OnApplicationExiting (object sender, ExitEventArgs args)
		{
			IdeApp.Exiting -= this.OnApplicationExiting;

			Shutdown ();
		}

		public void Setup ()
		{
			IdeApp.Workspace.SolutionLoaded += this.OnSolutionLoaded;
			IdeApp.Workspace.ItemAddedToSolution += this.OnItemAddedToSolution;
		}

		public void Shutdown ()
		{
			IdeApp.Workspace.SolutionLoaded -= this.OnSolutionLoaded;
			IdeApp.Workspace.ItemAddedToSolution -= this.OnItemAddedToSolution;
		}

		async void OnSolutionLoaded (object sender, SolutionEventArgs e)
		{
			var solution = e.Solution;
			var items = solution.Items;

			List<SolutionItem> needManifests = new List<SolutionItem> ();

			foreach (var item in items) {
				if (ManifestHelper.CheckIfNeedsManifestEntry (item)) {
					needManifests.Add (item);
				}
			}

			if (needManifests.Any ()) {
				await Task.Delay(2000);
				ManifestHelper.PromptForManifestGeneration (needManifests.ToList());
			}
		}

		void OnItemAddedToSolution (object sender, SolutionItemChangeEventArgs e)
		{
			var item = e.SolutionItem;


			if (ManifestHelper.CheckIfNeedsManifestEntry (item)) {
				ManifestHelper.PromptForManifestGeneration (new [] { item });
			}
		}
	}
}

