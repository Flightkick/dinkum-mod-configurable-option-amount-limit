using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ConfigurableOptionAmountLimit {
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        private const string OPTION_AMOUNT_LIMIT_KEY = "OptionAmountLimit";
        private const int OPTION_AMOUNT_MIN = 1;
        private const int OPTION_AMOUNT_MAX = 100_000;
        public static Plugin Instance { get; private set; }
        private readonly ConfigEntry<bool> _pluginEnabledConfig;
        private readonly ConfigEntry<int> _maxItemBuyLimitConfig;

        private bool PluginEnabled => _pluginEnabledConfig.Value;
        public int MaxItemBuyLimit => _maxItemBuyLimitConfig.Value;

        private readonly Harmony _harmony = new(PluginInfo.PLUGIN_GUID);

        public Plugin() {
            _pluginEnabledConfig = Config.Bind("Options", "Enabled", true, "You can disable the mod quickly by editing this value to false.");
            _maxItemBuyLimitConfig = Config.Bind("Options", OPTION_AMOUNT_LIMIT_KEY, 999, $"The maximum limit of items that can be bought at once. (Min: {OPTION_AMOUNT_MIN}, Max: {OPTION_AMOUNT_MAX})");
        }

        public void Awake() {
            Instance = this;
            
            string namespaceStr = GetType().Namespace;
            string configFile = $"{namespaceStr}.cfg";

            if (!PluginEnabled) {
                Logger.LogInfo($"Mod: {PluginInfo.PLUGIN_NAME} is disabled!");
                Logger.LogInfo($"Enable it via the {configFile} file.");
                return;
            }

            switch (MaxItemBuyLimit) {
                case < OPTION_AMOUNT_MIN:
                    throw new UserConfigurationException($"The '{OPTION_AMOUNT_LIMIT_KEY}' config option in {configFile} should be at least {OPTION_AMOUNT_MIN}.");
                case > OPTION_AMOUNT_MAX:
                    throw new UserConfigurationException($"The '{OPTION_AMOUNT_LIMIT_KEY}' config option in {configFile} should be at most {OPTION_AMOUNT_MAX}.");
                default:
                    Patch();
                    break;
            }
        }

        private void Patch() {
            _harmony.PatchAll();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} has loaded!");
            Logger.LogInfo($"Version: {PluginInfo.PLUGIN_VERSION}");
        }
    }
}