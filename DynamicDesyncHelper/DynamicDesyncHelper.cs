using ResoniteModLoader;
using HarmonyLib;
using FrooxEngine;

namespace DynamicDesyncHelper;

public partial class DynamicDesyncHelper : ResoniteMod
{
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> enable =
        new("enable", "Enable DynamicDesyncHelper", () => true);

    public override string Name => "DynamicDesyncHelper";
    public override string Author => "Raidriar796";
    public override string Version => "1.0.0";
    public override string Link => "https://github.com/Raidriar796/DynamicDesyncHelper";
    private static ModConfiguration? Config;

    public override void OnEngineInit()
    {
        Harmony harmony = new("net.raidriar796.DynamicDesyncHelper");
        Config = GetConfiguration();
        Config?.Save(true);
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(User), "InternalRunUpdate")]
    private static class UpdateLoopHook
    {
        private static int LWSBaseSize = 64;
        private static int LWSStepSize = 32;

        private static void Postfix()
        {
            RealtimeNetworkingSettings realtimeNetworkingSettings = Settings.GetActiveSetting<RealtimeNetworkingSettings>()!;
            if (realtimeNetworkingSettings == null && !Engine.Current.IsInitialized) return;
            if (Engine.Current.WorldManager.FocusedWorld == null) return;

            if (Engine.Current.WorldManager.FocusedWorld.LocalUser.QueuedMessages >= 1)
            {
                realtimeNetworkingSettings!.LNL_WindowSize.Value = LWSBaseSize + LWSStepSize;
            }
            else
            {
                realtimeNetworkingSettings!.LNL_WindowSize.Value = LWSBaseSize;
            }
        }
    }
}
