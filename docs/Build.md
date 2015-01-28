# Checked-in Toolkit

On the build machine, you shouldn't install NuProj. Instead, you should restore
the [NuGet package that provides the build server support](http://www.nuget.org/packages/NuProj).

In order for MSBuild to find the `NuProj.targets` that NuProj files depend on
you need to override the `NuProjTargetsPath` property:

```xml
<PropertyGroup>
    <MyNuProjPath>$(MyCheckinRoot)packages\NuProj.0.9.3\tools\</MyNuProjPath>
    <NuProjTargetsPath>$(MyNuProjPath)NuProj.targets</NuProjTargetsPath>
    <!--
    <NuProjTasksPath>$(MyNuProjPath)NuProj.Tasks.dll</NuProjTasksPath>
    <NuGetToolPath>$(MyNuProjPath)</NuGetToolPath>
    <NuGetToolExe>NuGet.exe</NuGetToolExe>
    -->
    <!-- This is required in order to get access to indirect dependencies.
         If you're already having a custom targets file that you inject
         into Microsoft.Common.targets, omit this line and instead import
         Microsoft.Common.NuProj.targets into your targets file. -->
    <CustomAfterMicrosoftCommonTargets>$(MyNuProjPath)Microsoft.Common.NuProj.targets</CustomAfterMicrosoftCommonTargets>
</PropertyGroup>
```

Optionally, you can also chose a different layout and override the other
properties as well.
