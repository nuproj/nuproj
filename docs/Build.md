# NuProj build authoring

To use the latest version of NuProj, and to enable support to build on cloud build agents
where the NuProj.msi is not installed, you should use NuGet to acquire the NuProj build authoring.

## NuProj core requirements

Acquire NuProj via NuGet.

### Set up your project.json file

Create a `project.json` file inside your NuProj project directory:

```json
{
  "dependencies": {
    "NuProj": "0.10.48-beta-gea4a31bbc5"
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

### Set up your nuget.config file

You will need to specify https://www.myget.org/F/aarnott/api/v3/index.json as
an additional NuGet package source in order to get the latest NuProj package.
Add a `nuget.config` file to the root of your repo: 

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="api.nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="myget.org/F/aarnott" value="https://www.myget.org/F/aarnott/api/v3/index.json" />
  </packageSources>
</configuration>
```

### Hook your .NuProj up to use the new build authoring

In order for MSBuild to find the `NuProj.targets` that NuProj files depend on
you need to change the `NuProjPath` property in your .nuproj file:

```xml
<PropertyGroup>
  <NuGetPackageRoot Condition="'$(NuGetPackageRoot)' == ''">$(UserProfile)\.nuget\packages\</NuGetPackageRoot>
  <NuProjPath>$(NuGetPackageRoot)NuProj\0.10.48-beta-gea4a31bbc5\tools\</NuProjPath>
</PropertyGroup>
<Import Project="$(NuProjPath)\NuProj.props" Condition="Exists('$(NuProjPath)\NuProj.props')" />
 ```

## Arrange for a package restore before build

Prior to starting a build (either locally or in a cloud build) you must ensure NuGet
has restored packages for your NuProj. Sometimes restoring packages for the solution
will be sufficient. When it is not, restore packages for your NuProj's project.json directly:

```
nuget restore solution\yourNuProjDir\project.json
```

## Additional requirements on projects referenced by NuProj

You need to install the [NuProj.Common package](http://www.nuget.org/packages/NuProj.Common)
that provides additional targets to all non-NuProj projects that are directly or indirectly
referenced by your NuProj project.
