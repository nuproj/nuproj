# Authoring

## Metadata

Most of the metadata is represented via simple MSBuild properties:

```xml
<PropertyGroup>
  <Id>HelloWorld</Id>
  <Version>1.0.0</Version>
  <Title>HelloWorld</Title>
  <Authors>The World</Authors>
  <Owners>$(Authors)</Owners>
  <Description>Hello World</Description>
  <!-- Optional information:
  <ReleaseNotes></ReleaseNotes>
  <Summary>Hello World</Summary>
  <ProjectUrl>http://example.com</ProjectUrl>
  <LicenseUrl>http://example.com/license</LicenseUrl>
  <Copyright>Copyright (c)</Copyright>
  <RequireLicenseAcceptance>False</RequireLicenseAcceptance>
  <DevelopmentDependency>False</DevelopmentDependency>
  <Tags>HelloWorld</Tags>
  -->
</PropertyGroup>
```

This allows defining the metadata in terms of other MSBuild properties (such as
`Owners` is simply defined via the `Authors` property). Also the, properties
can be defined via command line arguments passed to MSBuild.

## Dependencies

Package dependencies are expressed via MSBuild item groups:

```xml
<ItemGroup>
  <Dependency Include="RouteMagic">
    <Version>1.1.6</Version>
  </Dependency>
</ItemGroup>
```

The package ID is passed via the standard `Include` attribute. The version is
specified as a custom metadata element within the item. Also, you can indicate
that the dependency is only there when targeting a certain framework:

```xml
<ItemGroup>
  <Dependency Include="RouteMagic">
    <Version>1.1.6</Version>
    <TargetFramework>net40</TargetFramework>
  </Dependency>
</ItemGroup>
```

Packages marked as development dependencies will be ignored.

```xml
<ItemGroup>
  <Dependency Include="Fody">
    <Version>1.25.0</Version>
    <DevelopmentDependency>true</DevelopmentDependency>
  </Dependency>
</ItemGroup>
```

## Framework Assemblies

NuGet supports adding references to framework assemblies as well. You can
specify those via the `FrameworkReference` item:

```xml
<ItemGroup>
  <FrameworkReference Include="System.dll" />
  <FrameworkReference Include="System.Core.dll" />
</ItemGroup>
```

As with dependencies, those can be specific to a certain target framework:

```xml
<ItemGroup>
  <FrameworkReference Include="System.dll" />
  <FrameworkReference Include="System.Core.dll">
    <TargetFramework>net40</TargetFramework>
  </FrameworkReference>
</ItemGroup>
```

## Packaging Files

Packaging library files is done as follows:

```xml
<ItemGroup>
  <Library Include="$(BasePath)HelloWorld.dll" />
    <TargetFramework>net40</TargetFramework>
  </Library>
</ItemGroup>
```

The `TargetFramework` metadata is optional. If it's missing, the library will
apply to all platforms. Please note that per convention the path to the file
name is prefixed with the `$(BasePath)` property. This allows the build process
to control where the binaries should be picked up from.

For packaging content files you can use the `Content` item:

```xml
<ItemGroup>
  <!-- Package license.txt into content folder. -->
  <Content Include="$(BasePath)license.txt" />
  <!-- Package all .css files into content\css -->
  <Content Include="$(BasePath)css\**\*.css">
    <TargetPath>css</TargetPath>
  </Content>
  <!-- The following will also rename readme.txt to readme_HelloWorld.txt -->
  <Content Include="$(BasePath)readme.txt">
    <TargetPath>notes\readme_HelloWorld.txt</TargetPath>
  </Content>
</ItemGroup>
```

The `Content` element supports an optional `TargetPath` metadata element. If not
specified, the file will packaged directly into the content folder.

For ultimate control, you can also use the File item which allows you to package
arbitrary files:

```xml
<ItemGroup>
  <File Include="$(BasePath)build.proj">
    <TargetPath>tools\build</TargetPath>
  </File>
</ItemGroup>
```

## Controlling Library References

For a given a target platform, NuGet will add references to all the libraries in
the corresponding lib folder. You can override this behavior by using the
`Reference` item:

```xml
<ItemGroup>
  <Reference Include="HelloWorld.dll" />
</ItemGroup>
```

Please note that the reference item doesn't support the `TargetFramework`
metadata. Instead, NuGet will only add the references if the file is also
packaged in the corresponding lib folder. If it's not the reference is simply
ignored. In other words, the target framework is already controlled by the fact
that the file might or might not be in the corresponding lib folder.

## Merging Nuspec Template

In addition to using MSBuild properties and items, you can use existing nuspec
file as `NuSpecTemplate` property. The file will be used to populate default 
values. Specified MSBuild properties will overwrite the properties. MSBuild
items will be added to files, framework assemblies, references or dependencies.

```xml
<PropertyGroup>
  <Id>HelloWorld</Id>
  <Version>1.0.0</Version>
  <Title>HelloWorld</Title>
  <Authors>The World</Authors>
  <Owners>$(Authors)</Owners>
  <Description>Hello World</Description>
  <NuSpecTemplate>HelloWorldTemplate.nupsec</NuSpecTemplate>
</PropertyGroup>
```
