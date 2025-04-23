using BepInEx;
using BepInEx.Preloader.Core.Patching;

[BepInAutoPlugin]
public partial class PluginInGlobalNamespace { }

namespace Plugin
{
    [BepInAutoPlugin(id: "my id", name: "my name", version: "my version")]
    public partial class MyPluginWithOverrides { }

    // [PatcherAutoPlugin]
    public partial class MyPatcherPlugin { }

    namespace Nested
    {
        [BepInAutoPlugin]
        public partial class NestedPlugin { }
    }
}
