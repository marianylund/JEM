using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFuture : LevelProgression
{
    [SerializeField]
    private GameObject fireForTheEnd;
    private float originalBeeSpeed;
    private float originalEnemySpawnChance;
    private float originalCloudMovementSpeed;
    private Transform endBeeHive;

    public bool GotNotifiedABoutBeeReachingHome = false;
    internal override void SetUpTargetDistanceAndMilestones()
    {
        fireForTheEnd.SetActive(false);

        TargetDistance = Settings.s.targetDistanceFuture;
        originalBeeSpeed = Settings.s.beeFlyingSpeed;
        originalEnemySpawnChance = Settings.s.futureEnemySpawnChance;
        originalCloudMovementSpeed = Settings.s.cloudMovementSpeed;

        Settings.s.maxEnemiesInView = Settings.s.futureMaxEnemiesInView;
        Settings.s.maxFallingSeedsInView = Settings.s.futureMaxFallingSeedsInView;
        Settings.s.fallingSeedSpawnChance = Settings.s.futureFallingSeedSpawnChance;

        Settings.s.enemySpawnChance = 0.0f;
        Settings.s.cloudMovementSpeed = 0.0f;

        Settings.s.beeFlyingSpeed = 0.05f;
        Milestones.Add(0.1f); // Turn off bee hive sound
        Milestones.Add(Settings.s.futureIntroduceCloud); // Introduce clouds
        Milestones.Add(Settings.s.futureIntroduceEnemies); // Introduce enemies
        Milestones.Add(0.9f); // Turn on bee hive sound

        endBeeHive = GameObject.FindGameObjectWithTag("Finish")?.transform;
        if (endBeeHive == null)
        {
            Debug.LogError("No endBeeHive in the scene with tag Finish");
            gameObject.SetActive(false);
        }
        
        //endBeeHive.position = endBeeHive.position + Vector3.right * (TargetDistance - MainCameraBehaviour.GetCameraRect().width / 2.0f);

        StartCoroutine(SpeedUpTheBee());

        onEndReached += ReachedTheEnd;
        onMilestoneReached += ProgressLevel;

        RandomSpawnerOutsideView spawner = GameObject.FindObjectOfType<RandomSpawnerOutsideView>();
        Debug.Assert(spawner != null, "Did not find any random spawner in the scene!");
        spawner.StartSpawner();
    }

    protected void ProgressLevel(float milestone)
    {
        if (milestone == Settings.s.futureIntroduceCloud) 
        {
            IntroduceClouds();
        }
        if (milestone == Settings.s.futureIntroduceEnemies) 
        {
            IntroduceEnemies();
        }
    }

    internal override void _Destroy()
    {
        SelfDrivingBeeBehaviour.onReachingHome -= BeeReachedHome;
        onMilestoneReached -= ProgressLevel;
        onEndReached -= ReachedTheEnd;
    }

    private void IntroduceClouds()
    {
        Settings.s.cloudMovementSpeed = originalCloudMovementSpeed;
    }

    private void IntroduceEnemies()
    {
        Settings.s.enemySpawnChance = originalEnemySpawnChance;
    }

    private void ReachedTheEnd(float milestone)
    {
        MainCameraBehaviour.ShouldCameraFollowTheBee = false;
    }

    public void BeeReachedHome(int beeCount, Vector3 pos)
    {
        GotNotifiedABoutBeeReachingHome = true;
        Debug.Log("LevelFuture got that bee reached home");
        try
        {
            Settings.s.NUMBER_OF_BEES = beeCount;
            onLevelFinished?.Invoke(1.0f);
            if (fireForTheEnd)
            {
                fireForTheEnd.transform.position = endBeeHive.transform.position.With(y: fireForTheEnd.transform.position.y);
                fireForTheEnd.SetActive(true);
            }
            else
            {
                Debug.LogError("Fire has disappeared");
            }
            ScenesLoader.i_.LoadEnding();
            Debug.Log("Finished trying");
        }
        catch (Exception e )
        {
            Debug.LogError("Something went wrong when trying to load the ending. " + e.Message + "\n" + e.StackTrace + "\n" + e.InnerException);
            Debug.Log("Caught exception");
            ScenesLoader.i_.LoadEnding();
        }
    }

    void OnDisable()
    {
        Settings.s.beeFlyingSpeed = originalBeeSpeed;
        Settings.s.enemySpawnChance = originalEnemySpawnChance;
        Settings.s.cloudMovementSpeed = originalCloudMovementSpeed;
    }

    IEnumerator SpeedUpTheBee()
    {
        float timeToSpeedUp = Settings.s.beeWarmUpTimer;
        while (Settings.s.beeFlyingSpeed < originalBeeSpeed)
        {
            Settings.s.beeFlyingSpeed = Mathf.Lerp(0.05f, originalBeeSpeed, TimePlayed / timeToSpeedUp);
            yield return new WaitForEndOfFrame();
        }
        Settings.s.beeFlyingSpeed = originalBeeSpeed;
        SelfDrivingBeeBehaviour.onReachingHome += BeeReachedHome;
    }
}
