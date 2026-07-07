using UnityEngine;

public static class Variables
{
    //global variables
    public const int gameMaxValue = 99999, gameMinValue = -99999, gameNullValue = 900000, gameInfinityValue = -1;
    public const int gameDoesNotExistIndcator = 80000000;
    public const float naturalScalingIncrease = 0.001f; // 1/1000

    //relic variables
    public const int kineticBatterySpaces = 2, kineticBatteryVigorDuration = 3;
    public const int vampiricBootDamage = 7, vampiricBootBurstDuration = 3;
    public const int adaptiveShieldBlock = 4;
    public const int frozenLensSpeedLoss = 5;
    public const int shatteredSwordStrengthLoss = 3;
    public const int toxicTentaclePoison = 1;
    public const int waxHandRetainedBlock = 5;
    public const int mortarDamageIncrease = 1;
    public const int twoTailedNewtMaxTimes = 13;
    public const int petrifiedScrollCost = 6, petrifiedScrollMaxTimes = 1;
    public const int phantomLockpicksMaxTimes = 10;
    public const int everlastingFlameCost = 8, everlastingFlameMaxTimes = 1;
    public const int halfFilledPotionHealingPercent = 40;

    //rewards
    public const int ScrappedCardXP = 1;
    

    //colors
    public const string relicIncreaseableNumberColor = "<color=#009f9f>";

    //sprites
    public const string attackSprite = "<sprite name=Attack>";
    public const string blockSprite = "<sprite name=Block>";
    public const string skillSprite = "<sprite name=Skill>";
    public const string moveSprite = "<sprite name=Move>";
    public const string lockpickSprite = "<sprite name=Lockpick>";

    public const string rangeSprite = "<sprite name=Range>";
    public const string targetSprite = "<sprite name=Target>";

    

    //spawners
    public const int spawnerActivationDelay = 10;
    public const float spawnerSpawnChance = 0.002f; //1/500
    public const float enemySpawnYElevation = 12;

    //reward probabilities
    public const float commonCardProbability = 0.64f;
    public const float uncommonCardProbability = 0.32f;
    public const float rareCardProbability = 0.04f; // doesnt do anything
    public const float commonRelicProbability = 0.64f;
    public const float uncommonRelicProbability = 0.32f;
    public const float rareRelicProbability = 0.04f; // doesnt do anything
}
