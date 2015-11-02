using System;
using Mono.TextEditor;
using MonoDevelop.Components;
using MonoDevelop.Ide;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide.TypeSystem;

namespace MonoMobileSharpTools
{
//	public class ManifestTooltipProvider : TooltipProvider, IDisposable
//	{
//		public ManifestTooltipProvider ()
//		{
//		}
//
//		public override TooltipItem GetItem (Mono.TextEditor.TextEditor editor, int offset)
//		{
//			var doc = IdeApp.Workbench.ActiveDocument;
//
//			return null;
//		}
//		
//		static TooltipInformationWindow lastWindow = null;
//		TooltipItem lastResult;
//
//		static void DestroyLastTooltipWindow ()
//		{
//			if (lastWindow != null) {
//				lastWindow.Destroy ();
//				lastWindow = null;
//			}
//		}
//
//		public void Dispose ()
//		{
//			DestroyLastTooltipWindow ();
//			lastDefinition = null;
//			lastResult = null;
//		}
//
//		protected override Gtk.Window CreateTooltipWindow (Mono.TextEditor.TextEditor editor, int offset, Gdk.ModifierType modifierState, TooltipItem item)
//		{
//			var doc = IdeApp.Workbench.ActiveDocument;
//			if (doc == null)
//				return null;
//
//			var titem = (ResourceToolTipData)item.Item;
//
//			var	tooltipInformation = CreateTooltip (titem, offset, null, modifierState);
//
//			if (tooltipInformation == null || string.IsNullOrEmpty (tooltipInformation.SignatureMarkup))
//				return null;
//
//			var result = new TooltipInformationWindow ();
//			result.ShowArrow = true;
//			result.AddOverload (tooltipInformation);
//			result.RepositionWindow ();
//			return result;
//		}
//
//		public override Gtk.Window ShowTooltipWindow (TextEditor editor, int offset, Gdk.ModifierType modifierState, int mouseX, int mouseY, TooltipItem item)
//		{
//			var titem = (ResourceToolTipData)item.Item;
//			if (lastDefinition != null 
//				&& lastWindow != null 
//				&& lastWindow.IsRealized 
//				&& titem.Definition != null 
//				&& lastDefinition == titem.Definition)
//				return lastWindow;
//
//			DestroyLastTooltipWindow ();
//
//			var tipWindow = CreateTooltipWindow (editor, offset, modifierState, item) as TooltipInformationWindow;
//			if (tipWindow == null)
//				return null;
//
//			var p1 = editor.LocationToPoint (titem.StartLocation);
//			var p2 = editor.LocationToPoint (titem.EndLocation);
//			var positionWidget = editor.TextArea;
//			var caret = new Gdk.Rectangle ((int)p1.X - positionWidget.Allocation.X, (int)p2.Y - positionWidget.Allocation.Y, (int)(p2.X - p1.X), (int)editor.LineHeight);
//
//			tipWindow.ShowPopup (positionWidget, caret, PopupPosition.Top);
//			tipWindow.EnterNotifyEvent += delegate {
//				editor.HideTooltip (false);
//			};
//			lastWindow = tipWindow;
//			lastDefinition = titem.Definition;
//			return tipWindow;
//		}
//
//		TooltipInformation CreateTooltip (ResourceToolTipData tooltipData, int offset, Ambience ambience, Gdk.ModifierType modifierState)
//		{
//			if (!String.IsNullOrEmpty (tooltipData.OverloadMessage)) {
//				return new TooltipInformation () { SignatureMarkup = tooltipData.OverloadMessage };
//			}
//
//			ResourceTooltipMarkupBuilder markupBuilder = new ResourceTooltipMarkupBuilder (tooltipData.Session, tooltipData.Project);
//			return markupBuilder.GetResourceTooltip (tooltipData.Project, tooltipData.Definition);
//		}
//
//		protected override void GetRequiredPosition (Mono.TextEditor.TextEditor editor, Gtk.Window tipWindow, out int requiredWidth, out double xalign)
//		{
//			var win = (TooltipInformationWindow)tipWindow;
//			requiredWidth = win.Allocation.Width;
//			xalign = 0.5;
//		}
//	}
}
