using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"MonoDevelopAddinPackager", 
	Namespace = "MonoDevelopAddinPackager",
	Version = "2.0.0"
)]

[assembly:AddinName ("MonoDevelop Addin Packager")]
[assembly:AddinCategory ("IDE extensions")]
[assembly:AddinDescription ("Utility to package and deploy MonoDevelop addins. Adds 'Package Addin' and 'Create Addin Web Index options to the Project menu.")]
[assembly:AddinUrl("https://github.com/matthew-ch-robbins/monodevelop-addin-packager")]
[assembly:AddinAuthor ("Matthew Robbins")]
