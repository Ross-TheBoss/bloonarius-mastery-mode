using MelonLoader;
//using Harmony;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Simulation.Bloons;

using Il2CppSystem.Collections.Generic;

using Il2CppAssets.Scripts.Utils;
using System;

using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper;
using System.Linq;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppAssets.Scripts.Models.Bloons;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using UnityEngine.InputSystem.Utilities;
using BTD_Mod_Helper.Api.ModOptions;
using System.Collections.Generic;

using BTD_Mod_Helper.UI.Modded;
using Il2CppAssets.Scripts.Simulation.Track.RoundManagers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

[assembly: MelonInfo(typeof(BloonariusMasteryMode.BloonariusMasteryModeMod), BloonariusMasteryMode.ModHelperData.Name, BloonariusMasteryMode.ModHelperData.Version, BloonariusMasteryMode.ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace BloonariusMasteryMode
{
    public class BloonariusMasteryModeMod : BloonsTD6Mod
    {
        // public Image? image = null;

        // public override void OnMainMenu()
        // {
        //     base.OnMainMenu();
        //     image = MainMenuUI.GetPlayButton().image;
        //     image.SetSprite(VanillaSprites.WoodenRoundButton);
        // }

        // public override void OnUpdate()
        // {
        //     base.OnUpdate();
        //     Console.WriteLine("Nabbing stuff!");
        //     image?.SaveToPNG("WoodenRoundButton.png");
        // }

        public override void OnMatchStart()
        {
            if (!IsMasteryModeEnabled()){
                return;
            }

            // Upgrade all freeplay bloons
            foreach (FreeplayBloonGroupModel freeplayGroup in InGame.instance.bridge.Model.freeplayGroups){
                string line;
                if (freeplayGroup.bounds.LengthSafe() > 0){
                    FreeplayBloonGroupModel.Bounds bounds = freeplayGroup.bounds.First();
                    if (bounds.lowerBounds >= 141){
                        freeplayGroup.group.bloon = PromoteBloon(freeplayGroup.group.bloon);

                        if (freeplayGroup.group.bloon == "Bloonarius3"){
                            freeplayGroup.group.bloon = ModContent.BloonID<Bloonarius>();
                        } else if (freeplayGroup.group.bloon == "BloonariusElite3"){
                            freeplayGroup.group.bloon = ModContent.BloonID<EliteBloonarius>();
                        }

                        line = freeplayGroup.name;
                        line += " - Promoted!";
                    } else {
                        line = freeplayGroup.name;
                    }
                } else {
                    line = freeplayGroup.name;
                }

                LoggerInstance.Msg(line);
            }
        }

        private static readonly ModSettingBool LimitMinions = new(true) {
            displayName = "Limit minion spawning"
        };

        public static readonly System.Collections.Generic.Dictionary<string, string> promotionMap = new()
        {
            { "Red", "Blue" },
            { "RedCamo", "BlueCamo" },
            { "RedRegrow", "BlueRegrow" },
            { "RedRegrowCamo", "BlueRegrowCamo" },

            { "Blue", "Green" },
            { "BlueCamo", "GreenCamo" },
            { "BlueRegrow", "GreenRegrow" },
            { "BlueRegrowCamo", "GreenRegrowCamo" },

            { "Green", "Yellow" },
            { "GreenCamo", "YellowCamo" },
            { "GreenRegrow", "YellowRegrow" },
            { "GreenRegrowCamo", "YellowRegrowCamo" },

            { "Yellow", "Pink" },
            { "YellowCamo", "PinkCamo" },
            { "YellowRegrow", "PinkRegrow" },
            { "YellowRegrowCamo", "PinkRegrowCamo" },

            { "Pink", "Black" },
            { "PinkCamo", "BlackCamo" },
            { "PinkRegrow", "BlackRegrow" },
            { "PinkRegrowCamo", "BlackRegrowCamo" },

            { "Black", "Zebra" },
            { "BlackCamo", "ZebraCamo" },
            { "BlackRegrow", "ZebraRegrow" },
            { "BlackRegrowCamo", "ZebraRegrowCamo" },

            { "White", "Purple" },
            { "WhiteCamo", "PurpleCamo" },
            { "WhiteRegrow", "PurpleRegrow" },
            { "WhiteRegrowCamo", "PurpleRegrowCamo" },

            { "Purple", "LeadFortified" },
            { "PurpleCamo", "LeadFortifiedCamo" },
            { "PurpleRegrow", "LeadRegrowFortified" },
            { "PurpleRegrowCamo", "LeadRegrowFortifiedCamo" },

            { "Lead", "Rainbow" },
            { "LeadCamo", "RainbowCamo" },
            { "LeadRegrow", "RainbowRegrow" },
            { "LeadRegrowCamo", "RainbowRegrowCamo" },
            { "LeadFortified", "RainbowRegrowCamo" },
            { "LeadRegrowFortified", "RainbowRegrowCamo" },
            { "LeadFortifiedCamo", "RainbowRegrowCamo" },
            { "LeadRegrowFortifiedCamo", "RainbowRegrowCamo" },

            { "Zebra", "Rainbow" },
            { "ZebraCamo", "RainbowCamo" },
            { "ZebraRegrow", "RainbowRegrow" },
            { "ZebraRegrowCamo", "RainbowRegrowCamo" },

            { "Rainbow", "Ceramic" },
            { "RainbowCamo", "CeramicCamo" },
            { "RainbowRegrow", "CeramicRegrow" },
            { "RainbowRegrowCamo", "CeramicRegrowCamo" },

            { "Ceramic", "Moab" },
            { "CeramicCamo", "Moab" },
            { "CeramicRegrow", "Moab" },
            { "CeramicRegrowCamo", "Moab" },
            { "CeramicFortified", "MoabFortified" },
            { "CeramicFortifiedCamo", "MoabFortified" },
            { "CeramicRegrowFortified", "MoabFortified" },
            { "CeramicRegrowFortifiedCamo", "MoabFortified" },

            { "Moab", "Bfb" },
            { "MoabFortified", "BfbFortified" },

            { "Bfb", "DdtCamo" },
            { "BfbFortified", "DdtFortifiedCamo" },

            { "DdtCamo", "Zomg" },
            { "DdtFortifiedCamo", "ZomgFortified" },

            { "Zomg", "Bad" },
            { "ZomgFortified", "BadFortified" },

            { "Bad", "Bloonarius3" },
            { "BadFortified", "BloonariusElite3" }
        };

        public static int currentRound = 1;
        public static double totalRoundCash = 0;

        [HarmonyPatch(typeof(Simulation), "RoundStart")]
        public class RoundStartHook {
            [HarmonyPrefix]
            public static void Prefix(int spawnedRound){
                if (IsMasteryModeEnabled()){
                    Melon<BloonariusMasteryModeMod>.Logger.Msg($"Round {spawnedRound+1} (Mastery Mode) started!");
                }
                currentRound = spawnedRound;
            }
        }

        private static bool IsMasteryModeEnabled(){
            string masteryModeRoundsetId = ModContent.RoundSetId<AllCustomRounds>();
            // LoggerInstance.Msg($"{RoundSetChanger.RoundSetOverride} (RoundSetOverride) = {masteryModeRoundsetId}");       
            return RoundSetChanger.RoundSetOverride == masteryModeRoundsetId;
        }

        public override void OnBloonCreated(Bloon bloon)
        {
            if ((bloon.bloonModel.id == ModContent.BloonID<Bloonarius>()) || (bloon.bloonModel.id == ModContent.BloonID<EliteBloonarius>())){
                float speedMultiplier;
                if (currentRound <= 100){
                    speedMultiplier = 1.0f;
                } else if (currentRound <= 150){
                    speedMultiplier = 1.0f + (0.02f * (float)(currentRound - 101));
                } else if (currentRound <= 200){
                    speedMultiplier = 2.4f + (0.02f * (float)(currentRound - 151));
                } else if (currentRound <= 250){
                    speedMultiplier = 3.9f + (0.02f * (float)(currentRound - 201));
                } else {
                    speedMultiplier = 5.4f + (0.02f * (float)(currentRound - 252));
                }

                float healthMultiplier;
                if (currentRound <= 100){
                    healthMultiplier = 1.0f;
                } else if (currentRound <= 120){
                    healthMultiplier = 1.0f + (0.02f * (float)(currentRound - 100));
                } else if (currentRound <= 144){
                    healthMultiplier = 1.4f + (0.05f * (float)(currentRound - 120));
                } else if (currentRound <= 170){
                    healthMultiplier = 2.6f + (0.15f * (float)(currentRound - 144));
                } else if (currentRound <= 270){
                    healthMultiplier = 6.5f + (0.35f * (float)(currentRound - 170));
                } else if (currentRound <= 320){
                    healthMultiplier = 41.5f + (1.0f * (float)(currentRound - 270));
                } else if (currentRound <= 420){
                    healthMultiplier = 91.5f + (1.5f * (float)(currentRound - 320));
                } else if (currentRound <= 520){
                    healthMultiplier = 241.5f + (2.5f * (float)(currentRound - 420));
                } else {
                    healthMultiplier = 491.5f + (5.0f * (float)(currentRound - 520));
                }

                LoggerInstance.Msg($"Bloonarius Freeplay Rules: {speedMultiplier}x speed, {healthMultiplier}x health!");

                float defaultSpeed;
                int defaultHealth;

                if (bloon.bloonModel.id == ModContent.BloonID<Bloonarius>()){
                    defaultSpeed = Bloonarius.SPEED;
                    defaultHealth = Bloonarius.HEALTH;
                } else if (bloon.bloonModel.id == ModContent.BloonID<EliteBloonarius>()){
                    defaultSpeed = EliteBloonarius.SPEED;
                    defaultHealth = EliteBloonarius.HEALTH;
                } else {
                    LoggerInstance.Error($"Bloonarius has invalid id - {bloon.bloonModel.id}");
                    return;
                }

                bloon.bloonModel.Speed = defaultSpeed * speedMultiplier;
                bloon.bloonModel.maxHealth = (int)(defaultHealth * healthMultiplier);
            }
        }

        [HarmonyPatch(typeof(Simulation), "AddCash")]
        public class AddCash_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref double c, ref Simulation.CashSource source)
            {
                if ((source == Simulation.CashSource.Normal) && IsMasteryModeEnabled())
                {
                    if (currentRound >= 41 && currentRound <= 50){
                        c *= 0.5 / 1.0;
                    } else if (currentRound >= 51 && currentRound <= 60){
                        c *= 0.2 / 0.5; 
                    } else if (currentRound >= 61 && currentRound <= 75){
                        c *= 0.2 / 0.2;
                    } else if (currentRound >= 75 && currentRound <= 85){
                        c *= 0.1 / 0.2;
                    } else if (currentRound >= 86 && currentRound <= 90){
                        c *= 0.1 / 0.1;
                    } else if (currentRound >= 91 && currentRound <= 100){
                        c *= 0.05 / 0.1;
                    }

                    totalRoundCash += c;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Simulation), "RoundEnd")]
        public class RoundEndHook {
            [HarmonyPrefix]
            public static void Prefix(int round, int highestCompletedRound){
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Round {round+1} = ${totalRoundCash}");
                totalRoundCash = 0.0;
            }
        }

        [HarmonyPatch(typeof(FreeplayRoundManager), nameof(FreeplayRoundManager.GetRoundEmissions))]
        public class FreeplayRoundManager_GetRoundEmissionsHook {
            [HarmonyPostfix]
            public static void Postfix(FreeplayRoundManager __instance, int roundArrayIndex, ref Il2CppReferenceArray<BloonEmissionModel> __result){
                // Prevent the RBE limit from stopping 2 EliteBloonarius spawning on round 200.
                if (!IsMasteryModeEnabled()){
                    return;
                }

                if ((roundArrayIndex+1) == 200){
                    int i = 0;
                    int targetIndex = 0;
                    while (true){
                        try
                        {
                            var bounds = __instance.freeplayGroups[i].bounds.First();
                            if ((bounds.lowerBounds == 200) && (bounds.upperBounds == 200)){
                                targetIndex = i;
                                break;
                            }
                        } catch (IndexOutOfRangeException) {
                            break;
                        }

                        i++;
                    }

                    __result = __instance.freeplayGroups[targetIndex].bloonEmissions;
                }
            }
        }

        public class Bloonarius : ModBloon 
        {
            public override string BaseBloon => BloonType.Bloonarius3;

            public override string Icon => VanillaSprites.BloonariusPortrait;

            public const int HEALTH = 350000;

            public const float SPEED = 3.0f;

            public override void ModifyBaseBloonModel(BloonModel bloonModel)
            {
                // Default Speed: 1.25x. BAD Speed: 4.5x
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Bloonarius Default Speed: {bloonModel.Speed} -> {SPEED}");
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Bloonarius Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
                bloonModel.leakDamage = (float) HEALTH;
                bloonModel.Speed = SPEED; // New Speed: 3.0x
                bloonModel.GetBehavior<DistributeCashModel>().cash = 100000.0f;
                bloonModel.isBoss = false;

                if ((bool) LimitMinions.GetValue()){
                    // Don't Spawn Pink Bloons
                    foreach (SpawnBloonsActionModel behaviour in bloonModel.GetBehaviors<SpawnBloonsActionModel>()){
                        if (behaviour.bloonType == "Pink"){
                            behaviour.spawnCount = 0;
                        }
                    }
                }
            }
        }

        public class EliteBloonarius : ModBloon 
        {
            public override string BaseBloon => BloonType.BloonariusElite3;

            public override string Icon => VanillaSprites.BloonariusPortraitElite;

            public const int HEALTH = 2000000;

            public const float SPEED = 3.0f;

            public override void ModifyBaseBloonModel(BloonModel bloonModel)
            {
                // Default Speed: 1.25x. BAD Speed: 4.5x
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Bloonarius Default Speed: {bloonModel.Speed} -> {SPEED}");
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Bloonarius Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
                bloonModel.leakDamage = (float) HEALTH;
                bloonModel.Speed = SPEED; // New Speed: 3.0x
                bloonModel.GetBehavior<DistributeCashModel>().cash = 1000000.0f;
                bloonModel.isBoss = false;

                if ((bool) LimitMinions.GetValue()){
                    // Don't Spawn Ceramic Bloons
                    foreach (SpawnBloonsActionModel behaviour in bloonModel.GetBehaviors<SpawnBloonsActionModel>()){
                        if (behaviour.bloonType == "Ceramic"){
                            behaviour.spawnCount = 0;
                        }
                    }
                }
            }
        }

        public static string PromoteBloon(string bloon)
        {
            //if (bloon.Contains("Pink") || bloon.Contains("Lead")) return bloon;
            string temp = bloon;
            promotionMap.TryGetValue(bloon, out temp);
            return temp;
        }

        public class AllCustomRounds : ModRoundSet
        {
            public override string BaseRoundSet => RoundSetType.Default;
            public override int DefinedRounds => BaseRounds.Count;
            public override string DisplayName => "Mastery Mode";
            public override bool CustomHints => true;
            public override SpriteReference IconReference => ModContent.GetSpriteReference<BloonariusMasteryModeMod>("MasteryModeButton")!;

            private readonly System.Collections.Generic.Dictionary<int, string> hints = new()
            {
                {1, "Mastery mode... red bloons become blue bloons."},
                {2, "Blue bloons become green bloons."},
                {5, "Green bloons become yellow bloons."},
                {11, "I think you get the point."},
                {37, "First 2 MOAB-Class Bloons next round."},
                {39, "MOABs become BFBs..."},
                {45, "Fortified MOABs coming up next."},
                {54, "BTD6 is awesome. Life is awesome too. Don't forget to have a break sometimes and do something else. Then play more BTD6!"},
                {59, "What is a DDT Bloon you may ask? Like a MOAB crossed with a Pink, Camo, Black and Lead Bloon. In all the bad ways."},
                {62, "Next level will be hard. Really hard."},
                {79, "No ZOMGs so far... not too BAD was it?"},
                {84, "It's about to get worse though..."},
                {96, "Fortified BADs - as bad as it gets... right?"},
                {99, "The final round. Throw everything you've got at the Tier 3 Bloonarius. You won't make a dent."},
                {100, "Congratulations on beating round 100! Enjoy your reward!"},
                {139, "Only the BTD6 elite can beat the next round."},
                {199, "This mod knows no limits."}
            };

            public override void ModifyRoundModels(RoundModel roundModel, int round)
            {
                for (int k = 0; k < roundModel.groups.Length; k++)
                {
                    BloonGroupModel bloonGroup = roundModel.groups[k];
                    bloonGroup.bloon = PromoteBloon(bloonGroup.bloon);
                }

                try {
                    roundModel.ReplaceBloonInGroups<Bloonarius>("Bloonarius3");
                    roundModel.ReplaceBloonInGroups<EliteBloonarius>("BloonariusElite3");
                } catch (Exception e){
                    Melon<BloonariusMasteryModeMod>.Logger.Error($"{e.GetType()}: {e.Message}");
                }
            }

            public override string GetHint(int round){
                Melon<BloonariusMasteryModeMod>.Logger.Msg($"Getting round {round} hint.");
                return hints.GetValueOrDefault(round);
            }
        }
    }
}