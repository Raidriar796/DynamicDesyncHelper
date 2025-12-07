using ResoniteModLoader;
using HarmonyLib;
using FrooxEngine;
using Elements.Core;
using System.Diagnostics;

namespace DynamicDesyncHelper;

public partial class DynamicDesyncHelper : ResoniteMod
{
    public override string Name => "DynamicDesyncHelper";
    public override string Author => "Raidriar796";
    public override string Version => "0.1.0";
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
        private static bool isIncreasing = true;
        private static int lastQueuedCount = 0;
        private static double elapsedTime = 0;
        private static readonly Stopwatch stopwatch = Stopwatch.StartNew();

        private static void Postfix()
        {
            if (!Config!.GetValue(enable)) return; 
            if (!Engine.Current.IsInitialized) return;
            if (Engine.Current.WorldManager.FocusedWorld == null) return;
            if (Engine.Current.WorldManager.FocusedWorld.IsAuthority) return;

            RealtimeNetworkingSettings realtimeNetworkingSettings = Settings.GetActiveSetting<RealtimeNetworkingSettings>()!;
            if (realtimeNetworkingSettings == null) return;

            if (Engine.Current.WorldManager.FocusedWorld.LocalUser.QueuedMessages >= 1000)
            {
                if (lastQueuedCount < Engine.Current.WorldManager.FocusedWorld.LocalUser.QueuedMessages)
                {
                    if (!stopwatch.IsRunning) stopwatch.Restart();

                    if (!isIncreasing)
                    {
                        stopwatch.Restart();
                        isIncreasing = true;
                    }

                    elapsedTime = stopwatch.ElapsedMilliseconds / 1000;

                    if (elapsedTime > Config!.GetValue(stepIncreaseInterval))
                    {
                        stopwatch.Restart();
                        realtimeNetworkingSettings.LNL_WindowSize.Value = MathX.Min(realtimeNetworkingSettings.LNL_WindowSize.Value + Config.GetValue(LWSStepSize), 512);
                        lastQueuedCount = Engine.Current.WorldManager.FocusedWorld.LocalUser.QueuedMessages;
                    }
                }
                else 
                {
                    if (!stopwatch.IsRunning) stopwatch.Restart();

                    if (isIncreasing)
                    {
                        stopwatch.Restart();
                        isIncreasing = false;
                    }

                    elapsedTime = stopwatch.ElapsedMilliseconds / 1000;

                    if (elapsedTime > Config!.GetValue(stepDecreaseInterval))
                    {
                        stopwatch.Restart();
                        realtimeNetworkingSettings.LNL_WindowSize.Value = MathX.Max(realtimeNetworkingSettings.LNL_WindowSize.Value - Config.GetValue(LWSStepSize), Config.GetValue(LWSBaseSize));
                        lastQueuedCount = Engine.Current.WorldManager.FocusedWorld.LocalUser.QueuedMessages;
                    }
                }
            }
            else
            {
                if (!stopwatch.IsRunning) stopwatch.Restart();

                elapsedTime = stopwatch.ElapsedMilliseconds / 1000;

                if (elapsedTime > Config!.GetValue(stepDecreaseInterval))
                {
                    stopwatch.Restart();
                    realtimeNetworkingSettings.LNL_WindowSize.Value = MathX.Max(realtimeNetworkingSettings.LNL_WindowSize.Value - Config.GetValue(LWSStepSize), Config.GetValue(LWSBaseSize));
                    lastQueuedCount = Engine.Current.WorldManager.FocusedWorld.LocalUser.QueuedMessages;
                }
            }
        }
    }
}
