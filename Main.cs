using BepInEx;
using HarmonyLib;
using UnboundLib.Utils;
namespace Root_Maps {
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class Main : BaseUnityPlugin
    {
        private const string ModId = "root.custom.maps";
        private const string ModName = "Root_Maps";
        public const string Version = "0.3.3";
        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            LevelManager.RegisterMaps(Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("root_maps", typeof(Main).Assembly), "<b><#54018f>Root Maps");
        }
    }
}
