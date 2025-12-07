using ResoniteModLoader;

namespace DynamicDesyncHelper;

public partial class DynamicDesyncHelper : ResoniteMod
{
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> enable =
        new("enable", "Enable DynamicDesyncHelper", () => true);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<int> LWSBaseSize =
        new("LWSBaseSize", "The baseline LNL Window Size", () => 64, false, v => v <= 512 && v >= 8);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<int> LWSStepSize =
        new("LWSStepSize", "How much the LNL Window Size is changed per interval", () => 32, false, v => v <= 512 && v >= 8);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<int> stepIncreaseInterval =
        new("stepIncreaseInterval", "Seconds between increasing the LNL Window Size", () => 10, false, v => v >= 1);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<int> stepDecreaseInterval =
        new("stepDecreaseInterval", "Seconds between decreasing the LNL Window Size", () => 60, false, v => v >= 1);
}
