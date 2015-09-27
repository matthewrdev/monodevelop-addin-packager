using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"MonoDevelopAddinPackager", 
	Namespace = "MonoDevelopAddinPackager",
	Version = "2.2.0"
)]

[assembly:AddinName ("MonoDevelop Addin Packager")]
[assembly:AddinCategory ("IDE extensions")]
[assembly:AddinDescription ("Tooling to assist the development of MonoDevelop addins.\n\nCurrent features:\n\n - 'Package Addin' to pack a MonoDevelop Addin project into an .mpack.\n - 'Create Addin Web Index (.mrep)' to build web index .mrep files for the addin.\n - 'Clean Addin Packages' to delete all generated package files for the active project and configuration.")]
[assembly:AddinUrl("https://github.com/matthew-ch-robbins/monodevelop-addin-packager")]
[assembly:AddinAuthor ("Matthew Robbins")]
