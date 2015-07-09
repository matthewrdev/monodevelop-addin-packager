using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"MonoDevelopAddinPackager", 
	Namespace = "MonoDevelopAddinPackager",
	Version = "1.0.0"
)]

[assembly:AddinName ("MonoDevelop Addin Packager")]
[assembly:AddinCategory ("IDE extensions")]
[assembly:AddinDescription ("Helper utility that allows monodevelop addin projects to be packaged into an mpack.")]
[assembly:AddinUrl("https://github.com/matthew-ch-robbins/monodevelop-addin-packager")]
[assembly:AddinAuthor ("Matthew Robbins")]
