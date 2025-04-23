# BepInEx.AutoPlugin

[![CI](https://github.com/BepInEx/BepInEx.AutoPlugin/workflows/CI/badge.svg)](https://github.com/BepInEx/BepInEx.AutoPlugin/actions)
[![NuGet](https://img.shields.io/endpoint?color=blue&logo=NuGet&label=NuGet&url=https://shields.kzu.io/v/BepInEx.AutoPlugin?feed=nuget.bepinex.dev/v3/index.json)](https://nuget.bepinex.dev/packages/BepInEx.AutoPlugin)

Incremental C# Source generator that turns

```cs
[BepInAutoPlugin("com.example.ExamplePlugin")]
public partial class ExamplePlugin : BaseUnityPlugin
{
}
```

into

```cs
[BepInEx.BepInPlugin(ExamplePlugin.Id, "ExamplePlugin", "0.1.0")]
public class ExamplePlugin : BaseUnityPlugin
{
    public const string Id = "com.example.ExamplePlugin";
    public static string Name => "ExamplePlugin";
    public static string Version => "0.1.0";
}
```

## Configuration

AutoPlugin allows stripping version build metadata to turn `1.0.0-beta+1234567890` into `1.0.0-beta` for the `Version` property by setting the following in your csproj:

```xml
<PropertyGroup>
  <BepInAutoPluginStripBuildMetadata>true</BepInAutoPluginStripBuildMetadata>
</PropertyGroup>
```

Note that this will do nothing on BepInEx 5 as it only accepts a version number without a pre-release version or build metadata, so it strips both.
