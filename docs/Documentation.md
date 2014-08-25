# Welcome to NuProj

NuProj provides an MSBuild-based approach to create NuGet packages (.nupkg). The
build projects are called .nuproj files and are regular MSBuild projects.

You can create new a .nuproj file by using the template that we install into
Visual Studio. Invoke the menu command **File | New | File...** or simply press
<kbd>Ctrl + N</kbd> and select **NuGet Package MSBuild Project**.

In general, the definition of a NuProj file is very similar to the NuGet .nuspec
file – except that it is done via MSBuild. In fact, NuProj generates the .nuspec
file and simply calles NuGet to build the package. Thus, for details you can
still refer to the [official NuGet documentation](http://docs.nuget.org/docs/reference/nuspec-reference).

For more details, visit:

* [Authoring](Authoring.md)
* [Build Integration](Build.md)