using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameSettings : ScriptableObject
{
    [Header("PAST")]
    public float targetDistancePast = 120.0f;
    [Tooltip("Has to be a value between 0.0 and 1.0, how much of the way bee needs to pass before clouds are introduced. Default 0.3, so 30% of the way")]
    public float pastIntroduceCloud = 0.3f;
    public float pastIntroduceEnemies = 0.5f;
    public float pastFallingSeedSpawnChance = 0.8f;
    public int pastMaxFallingSeedsInView = 6;
    public int pastMaxEnemiesInView = 2;
    public float pastEnemySpawnChance = 0.5f;

    [Header("PRESENT")]
    public float targetDistanceNow = 150.0f;
    public float nowIntroduceCloud = 0.1f;
    public float nowIntroduceEnemies = 0.15f;
    public float nowFallingSeedSpawnChance = 0.8f;
    public int nowMaxFallingSeedsInView = 3;
    public int nowMaxEnemiesInView = 3;
    public float nowEnemySpawnChance = 0.5f;

    [Header("FUTURE")]
    public float targetDistanceFuture = 200.0f;
    public float futureIntroduceCloud = 0.0f;
    public float futureIntroduceEnemies = 0.0f;
    public float futureFallingSeedSpawnChance = 0.6f;
    public int futureMaxFallingSeedsInView = 2;
    public int futureMaxEnemiesInView = 4;
    public float futureEnemySpawnChance = 0.7f;

    [Header("Bee")]
    public int numberOfBeesToStartWith = 15;
    public float beeFlyingSpeed = 2f;
    [Tooltip("The bee will accelarate for the first beeWarmUpTimer seconds until it reaches the beeFlyingSpeed")]
    public float beeWarmUpTimer = 3.0f;
    public float beeLoosingEnergySpeed = 1.0f;
    public int beeMaxEnergy = 10;
    public int onLoosingBeeRestoreEnergy = 5;
    public Gradient beeSliderGradient;

    [Header("Flower")]
    public int flowerRestoresEnergy = 3;

    [Header("FallingSeed")]
    public float seedFallingSpeed = 0.8f;
    public float fallingSeedSpawnChance = 0.8f;
    public int maxFallingSeedsInView = 6;
    public float seedMaxLifeLength = 20.0f;
    public float seedMinLifeLength = 10.0f;

    [Header("Clouds")]
    public float cloudMovementSpeed = 1.0f;
    public float darkenEffectMaxAlpha = 0.4f;
    public Color darkenEffectColor = new Color(0.245283f, 0.2094162f, 0.2094162f, 0.4f);
    public float darkenEffectSpeed = 1.0f;

    [Header("Enemy")]
    public float enemyMovementSpeed = 1.2f;
    public int clicksToEliminateEnemy = 2;
    public int maxEnemiesInView = 4;
    public float enemySpawnChance = 0.5f;
    [Tooltip("How fast the cloud gains the rain and empties it. For example rainTimer = 2.0f, so the cloud will darken for 2 seconds and the rain for 2 second")]
    public float rainTimer = 2.0f;

    [Header("Camera")]
    public float cameraMovementOffset = 0.8f;
    public Rect spawnBounds = new Rect(0.0f, 0.06f, 1.0f, 0.6f);
    public Rect enemySpawnBounds = new Rect(0.0f, 0.06f, 1.0f, 0.6f);
    public Rect seedSpawnBounds = new Rect(-.084f, 0.52f, 1.0f, 0.32f);

    [Header("Debug")]
    public bool debugMode = true;
    public Color enemySpawnBoundDebugColor = Color.red;
    public Color seedSpawnBoundDebugColor = Color.green;

    [Header("Do not change, please")]
    public int NUMBER_OF_BEES = 5;
    public HashSet<Level> UNLOCKED_LEVELS = new HashSet<Level>() { Level.Past };

}
