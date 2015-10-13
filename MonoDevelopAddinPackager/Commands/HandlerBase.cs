using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Projects;
using System.Linq;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Dialogs;

namespace MonoDevelopAddinPackager
{
	public abstract class HandlerBase : CommandHandler
	{
		protected class CommandTaskScope : IDisposable
		{
			ProgressDialog dialog;
			internal CommandTaskScope(ProgressDialog dialog, string taskMessage)
			{
				this.dialog = dialog;
				dialog.BeginTask(taskMessage);
			}

			public void Dispose ()
			{
				dialog.EndTask ();
			}
		}

		public HandlerBase ()
		{
		}

		protected ProgressDialog _dialog;

		int _progress = 0;

		public abstract int MaxStatusSteps { get; }
		public abstract string CommandName { get; }
		public abstract bool Execute(SolutionItem solutionItem, out string message);

		protected void CommandStatus(string message)
		{
			_dialog.WriteText (message + "\n");
		}

		protected CommandTaskScope TaskScope(string message)
		{
			AdvanceProgress ();
			return new CommandTaskScope (_dialog, message);
		}

		protected void AdvanceProgress()
		{
			_progress++;

			_dialog.Progress = (double)_progress / (double)MaxStatusSteps;
		}

		protected override void Run ()
		{
			_progress = 0;

			SolutionItem item = GetTargettedSolutionItem ();

			if (item == null) {
				Console.WriteLine ("Can't process project because no solution items are active.");
				return;
			}


			_dialog = new ProgressDialog (IdeApp.Workbench.RootWindow, true, true);
			_dialog.Title = CommandName;
			_dialog.Show ();

			bool success;
			string message = "";
			try {
				success = Execute (item, out message);
			}
			catch (Exception ex) {
				message = "Command failed.";
				CommandStatus ("Failed to execute command: " + ex.Message);
				CommandStatus ("Please file a bug report at https://github.com/matthew-ch-robbins/monodevelop-addin-packager");
				success = false;
			}


			_dialog.Progress = 1.0;
			_dialog.Message = message;

			if (success) {
				_dialog.Hide ();
			}
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

			return SolutionItemHelper.IsAddinProject (targettedItem);
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
			Console.WriteLine (MDToolHelper.ResolveMDToolPath ());
			info.Enabled = CanExecuteInContext ();
		}  
	}
}

