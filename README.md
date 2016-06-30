# MonoDevelop Addin Packager

**Support for the Addin Packager has been deprecated as of Xamarin Studio 6**

To generate an mpack for your addins, use the script below:

````
find . -name "obj" | xargs rm -Rf
find . -name "bin" | xargs rm -Rf

BUILD_DATE=$(date +"%y-%m-%d-%H-%M")

mkdir ./builds
mkdir ./builds/mpack
mkdir ./builds/mpack/$BUILD_DATE

# Build your solution
xbuild /p:Configuration=Release ./MySolution.sln

# Package the dll using MD tool
/Applications/Xamarin\ Studio.app/Contents/MacOS/mdtool setup pack ./MySolution/bin/Release/MyAddin.dll -d:./builds/mpack/$BUILD_DATE

# View output
open ./builds/mpack/$BUILD_DATE
````

Tooling to assist the development of MonoDevelop addins.

Current features:
 - 'Package Addin' to pack a MonoDevelop Addin project into an .mpack.
 - 'Create Addin Web Index (.mrep)' to build web index .mrep files for the addin.
 - 'Clean Addin Packages' to delete all generated package files for the active project and configuration.

Keep up to date with the latest features by following me on Twitter: [@matthewrdev](https://twitter.com/matthewrdev)


Feedback is welcomed.
