using MelonLoader;
using Il2CppAssets.Scripts.Simulation.Bloons;

using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper;
using Il2CppAssets.Scripts.Models.Bloons;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using UnityEngine.InputSystem.Utilities;
using BTD_Mod_Helper.Api.ModOptions;
using System.Collections.Generic;

using System.Collections.Immutable;
using BloonariusMasteryMode.Patches;

[assembly: MelonInfo(typeof(BloonariusMasteryMode.BloonariusMasteryModeMod), BloonariusMasteryMode.ModHelperData.Name, BloonariusMasteryMode.ModHelperData.Version, BloonariusMasteryMode.ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace BloonariusMasteryMode;

public class BloonariusMasteryModeMod : BloonsTD6Mod
{
    private static readonly ModSettingBool LimitMinions = new(false) {
        displayName = "Limit minion spawning"
    };
    public static bool IsMasteryModeEnabled = false;

    public static readonly ModSettingInt RoundsEarly = new(10){
        displayName = "Rounds early that cash per pop is reduced.",
        slider = true,
        min = 0,
        max = 20
    };

    private static readonly Dictionary<string, string> promotionMap = new()
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
    
    public override void OnUpdate(){
        #if DEBUG
        CashCalculator.OnUpdate();
        #endif
        DifficultySelectScreenPatch.OnUpdate();
    }

    public static int currentRound = 1;
    public override void OnBloonCreated(Bloon bloon)
    {
        if ((bloon.bloonModel.id == ModContent.BloonID<Bloonarius>()) || (bloon.bloonModel.id == ModContent.BloonID<BloonariusFortified>())){
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

            #if DEBUG
            LoggerInstance.Msg($"Bloonarius Freeplay Rules: {speedMultiplier}x speed, {healthMultiplier}x health!");
            #endif

            float defaultSpeed;
            int defaultHealth;

            if (bloon.bloonModel.id == ModContent.BloonID<Bloonarius>()){
                defaultSpeed = Bloonarius.SPEED;
                defaultHealth = Bloonarius.HEALTH;
            } else if (bloon.bloonModel.id == ModContent.BloonID<BloonariusFortified>()){
                defaultSpeed = BloonariusFortified.SPEED;
                defaultHealth = BloonariusFortified.HEALTH;
            } else {
                LoggerInstance.Error($"Bloonarius has invalid id - {bloon.bloonModel.id}");
                return;
            }

            bloon.bloonModel.Speed = defaultSpeed * speedMultiplier;
            bloon.bloonModel.maxHealth = (int)(defaultHealth * healthMultiplier);
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
            #if DEBUG
            // Default Speed: 1.25x. BAD Speed: 4.5x
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Bloonarius Default Speed: {bloonModel.Speed} -> {SPEED}");
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Bloonarius Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
            #endif
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

    public class BloonariusFortified : ModBloon
    {
        public override string BaseBloon => BloonType.BloonariusElite3;

        public override string Icon => VanillaSprites.BloonariusPortraitElite;

        public const int HEALTH = 2000000;

        public const float SPEED = 3.0f;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            #if DEBUG
            // Default Speed: 1.25x. BAD Speed: 4.5x
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Bloonarius Default Speed: {bloonModel.Speed} -> {SPEED}");
            Melon<BloonariusMasteryModeMod>.Logger.Msg($"Elite Bloonarius Default Leak Damage: {bloonModel.leakDamage} -> {HEALTH}");
            #endif
            bloonModel.leakDamage = (float) HEALTH;
            bloonModel.Speed = SPEED; // New Speed: 3.0x
            bloonModel.GetBehavior<DistributeCashModel>().cash = 1000000.0f;
            bloonModel.isBoss = false;
            bloonModel.dontShowInSandbox = true;
            bloonModel.dontShowInSandboxOnRelease = true;

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
        string temp = promotionMap.GetValueOrDefault(bloon, bloon);

        return temp switch
        {
            "Bloonarius3" => ModContent.BloonID<Bloonarius>(),
            "BloonariusElite3" => ModContent.BloonID<BloonariusFortified>(),
            _ => temp,
        };
    }
}