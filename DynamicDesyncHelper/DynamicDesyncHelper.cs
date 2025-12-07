using ResoniteModLoader;
using HarmonyLib;
using FrooxEngine;

namespace DynamicDesyncHelper;

public partial class DynamicDesyncHelper : ResoniteMod
{
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
}
