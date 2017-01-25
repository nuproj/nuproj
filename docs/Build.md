# NuProj build authoring

To use the latest version of NuProj, and to enable support to build on cloud build agents
where the NuProj.msi is not installed, you should use NuGet to acquire the NuProj build authoring.

## NuProj core requirements

Acquire NuProj via NuGet.

### Set up your solution-level packages.config file

Create a `.nuget\packages.config` file under your solution folder

```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="NuProj" version="0.11.14-beta" developmentDependency="true" />
</packages>
```

### Hook your .NuProj up to use the new build authoring

In order for MSBuild to find the `NuProj.targets` that NuProj files depend on
you need to change the `NuProjPath` property in your .nuproj file:

```xml
<PropertyGroup>
  <NuProjPath>..\packages\NuProj.0.11.14-beta\tools\</NuProjPath>
</PropertyGroup>
<Import Project="$(NuProjPath)\NuProj.props" Condition="Exists('$(NuProjPath)\NuProj.props')" />
 ```

## Arrange for a package restore before build or open the project

Prior to opening the project or starting a build (either locally or in a cloud build) you must
ensure NuGet has restored packages for your NuProj.

```
nuget restore your.sln
```

## Additional requirements on projects referenced by NuProj

You need to install the [NuProj.Common package](http://www.nuget.org/packages/NuProj.Common)
that provides additional targets to all non-NuProj projects that are directly or indirectly
referenced by your NuProj project.

## Using project.json within your NuProj

As an optional step, you can use project.json within your NuProj project
to add NuGet packages that customize your NuProj's build.

To do this, create a project.json file inside your NuProj project directory.
In this example, I use the [ReadOnlySourceTree](https://www.nuget.org/packages/ReadOnlySourceTree) package
to cause NuProj to build to bin and obj folders that are beside the src folder.

```json
{
  "dependencies": {
    "ReadOnlySourceTree": "0.1.37-beta"
  },
  "frameworks": {
    "net451": { }
  },
  "runtimes": {
    "win": { }
  }
}
```

Note that the `frameworks` and `runtimes` fields in your `project.json`
file are required by NuGet, but completely irrelevant to the actual runtimes
and frameworks your package supports.

You should also add a project item for your project.json file to your NuProj project file:

```xml
<ItemGroup>
  <None Include="project.json" />
</ItemGroup>
```

