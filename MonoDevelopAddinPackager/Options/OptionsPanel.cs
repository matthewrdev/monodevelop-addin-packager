using System;
using System.Linq;
using Gtk;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using System.IO;
using System.Diagnostics;

namespace MonoDevelopAddinPackager
{
	public class MonoDevelopAddinPackagerOptionsPanel : OptionsPanel
	{
		MonoDevelopAddinPackagerPanelWidget widget;

		public override Widget CreatePanelWidget ()
		{
			return widget = new  MonoDevelopAddinPackagerPanelWidget ();
		}

		public override void ApplyChanges ()
		{
			widget.Store ();
		}
	}

	public class MonoDevelopAddinPackagerPanelWidget : Gtk.Bin 
	{	
		string mrepPath;
		string deployScript;

		Label mrepOutputLabel;
		Entry mrepEntry;

		Label deploymentScriptLabel;
		Entry deploymentScriptEntry;

		Button submitBugButton;

		VBox container;

		public MonoDevelopAddinPackagerPanelWidget ()
		{
			global::Stetic.BinContainer.Attach (this);

			Requisition size;

			mrepPath = PropertyService.Get (Options.Keys.MREP_OUTPUT_FOLDER_KEY, Options.Defaults.MREP_OUTPUT_FOLDER_DEFAULT);
			deployScript = PropertyService.Get (Options.Keys.DEPLOY_SCRIPT_KEY, Options.Defaults.DEPLOY_SCRIPT_DEFAULT);

			mrepOutputLabel = new Label ("Output Folder (mrep)");
			size = mrepOutputLabel.SizeRequest ();
			mrepOutputLabel.SetSizeRequest (size.Width, size.Height);
			mrepOutputLabel.TooltipText = "The output folder for the web archive generation (mrep files). When empty, the output folder will default to current configurations output folder.";
			mrepEntry = new Entry ();
			mrepEntry.Text = mrepPath;
			mrepEntry.Changed += (object sender, EventArgs e) => {
				mrepPath = mrepEntry.Text;
				PropertyService.Set (Options.Keys.MREP_OUTPUT_FOLDER_KEY, mrepPath);
			};

			deploymentScriptLabel = new Label ("Deployment Command");
			deploymentScriptLabel.TooltipText = "A command that will be invoked via bash that will deploy the addin. The command will execute with the output folder as its working directory.";
			deploymentScriptEntry = new Entry ();
			deploymentScriptEntry.Text = deployScript;
			deploymentScriptEntry.Changed += (object sender, EventArgs e) => {
				deployScript = deploymentScriptEntry.Text;
				PropertyService.Set (Options.Keys.DEPLOY_SCRIPT_KEY, deployScript);
			};

			submitBugButton = new Button ();
			submitBugButton.Label = "Submit a bug";
			submitBugButton.Clicked += (object sender, EventArgs e) => {
				Process.Start(@"https://github.com/matthewrdev/monodevelop-addin-packager/issues/new");
			};

			container = new Gtk.VBox ();
			container.Add (mrepOutputLabel);
			container.Add (mrepEntry);
			container.Add (deploymentScriptLabel);
			container.Add (deploymentScriptEntry);
			container.Add (new Label ("")); // For padding.
			container.Add (submitBugButton);

			Add (container);
			ShowAll ();
		}

		public void Store ()
		{
			PropertyService.Set (Options.Keys.MREP_OUTPUT_FOLDER_KEY, mrepPath);
			PropertyService.Set (Options.Keys.DEPLOY_SCRIPT_KEY, deployScript);
		}
	}
}

