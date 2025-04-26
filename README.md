# BepInEx.AutoPlugin

[![CI](https://github.com/BepInEx/BepInEx.AutoPlugin/workflows/CI/badge.svg)](https://github.com/BepInEx/BepInEx.AutoPlugin/actions)
[![NuGet](https://img.shields.io/endpoint?color=blue&logo=NuGet&label=NuGet&url=https://shields.kzu.io/v/BepInEx.AutoPlugin?feed=nuget.bepinex.dev/v3/index.json)](https://nuget.bepinex.dev/packages/BepInEx.AutoPlugin)

BepInEx.AutoPlugin is an incremental C# source generator that takes the following properties from your project:

```xml
<PropertyGroup>
  <AssemblyName>com.example.ExamplePlugin</AssemblyName>
  <AssemblyTitle>ExamplePlugin</AssemblyTitle>
  <Version>0.1.0</Version>
</PropertyGroup>
```

And generates a partial class for your partial plugin class decorated with the `BepInAutoPluginAttribute`, decorating the generated class with the `BepInPluginAttribute` using the above properties:

```cs
[BepInEx.BepInPlugin(ExamplePlugin.Id, "ExamplePlugin", "0.1.0")]
partial class ExamplePlugin : BaseUnityPlugin
{
    public const string Id = "com.example.ExamplePlugin";
    public static string Name => "ExamplePlugin";
    public static string Version => "0.1.0";
}
```

A `PatcherAutoPluginAttribute` also exists for BepInEx 6 preloader patchers.

## Usage

Mark your plugin class partial, and decorate it with the `BepInAutoPluginAttribute`:

```cs
[BepInAutoPlugin]
public partial class ExamplePlugin : BaseUnityPlugin
{
}
```

And you're done! You don't need to manually include the `BepInPluginAttribute`, as the generated partial class includes that.

You can also access all the properties in your code, as they are public members in your class:

```cs
[BepInAutoPlugin]
public partial class ExamplePlugin : BaseUnityPlugin
{
    void Awake()
    {
        Logger.LogInfo($"Plugin {Name} version {Version} is loaded!");
    }
}
```

### Overriding Properties

AutoPlugin allows overriding any of the properties with the optional attribute arguments:

```cs
[BepInAutoPlugin(id: "com.example.MyOverrideId", name: "My Override Name", version: "1.2.3")]
public partial class ExamplePlugin : BaseUnityPlugin
{
}
```

### MSBuild Configuration

AutoPlugin allows stripping version build metadata to turn `1.0.0-beta+1234567890` into `1.0.0-beta` for the `Version` property by setting the following in your csproj:

```xml
<PropertyGroup>
  <BepInAutoPluginStripBuildMetadata>true</BepInAutoPluginStripBuildMetadata>
</PropertyGroup>
```

Note that this will do nothing on BepInEx 5 as it only accepts a version number without a pre-release version or build metadata, so it strips both.
