using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNow : LevelProgression
{
    private float originalBeeSpeed;
    private float originalEnemySpawnChance;
    private float originalCloudMovementSpeed;
    private Transform endBeeHive;

    internal override void SetUpTargetDistanceAndMilestones()
    {
        TargetDistance = Settings.s.targetDistanceNow;
        originalBeeSpeed = Settings.s.beeFlyingSpeed;
        originalCloudMovementSpeed = Settings.s.cloudMovementSpeed;

        originalEnemySpawnChance = Settings.s.nowEnemySpawnChance;
        Settings.s.maxEnemiesInView = Settings.s.nowMaxEnemiesInView;
        Settings.s.maxFallingSeedsInView = Settings.s.nowMaxFallingSeedsInView;
        Settings.s.fallingSeedSpawnChance = Settings.s.nowFallingSeedSpawnChance;


        Settings.s.enemySpawnChance = 0.0f;
        Settings.s.cloudMovementSpeed = 0.0f;

        Settings.s.beeFlyingSpeed = 0.05f;
        Milestones.Add(0.1f); // Turn off bee hive sound
        Milestones.Add(Settings.s.nowIntroduceCloud); // Introduce clouds
        Milestones.Add(Settings.s.nowIntroduceEnemies); // Introduce enemies
        Milestones.Add(0.9f); // Turn on bee hive sound

        endBeeHive = GameObject.FindGameObjectWithTag("Finish")?.transform;
        if (endBeeHive == null)
        {
            Debug.LogError("No endBeeHive in the scene with tag Finish");
            gameObject.SetActive(false);
        }
        endBeeHive.position = endBeeHive.position + Vector3.right * TargetDistance;

        onEndReached += ReachedTheEnd;
        LevelProgression.onMilestoneReached += ProgressLevel;
        StartCoroutine(SpeedUpTheBee());

        RandomSpawnerOutsideView spawner = GameObject.FindObjectOfType<RandomSpawnerOutsideView>();
        Debug.Assert(spawner != null, "Did not find any random spawner in the scene!");
        spawner.StartSpawner();
    }

    private void ProgressLevel(float milestone)
    {
        if (milestone == Settings.s.nowIntroduceCloud) 
        {
            IntroduceClouds();
        }
        if (milestone == Settings.s.nowIntroduceEnemies) 
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

    private void BeeReachedHome(int beeCount, Vector3 pos)
    {
        Debug.Log("LevelNow got that bee reached home");
        string msg = beeCount.ToString() + " bees reached home safe and sound!";
        try
        { 
            onLevelFinished?.Invoke(1.0f);
            Settings.s.NUMBER_OF_BEES = beeCount;
            if (!Settings.s.UNLOCKED_LEVELS.Contains(Level.Future))
            {
                Settings.s.UNLOCKED_LEVELS.Add(Level.Future);
                msg += "\nNew Future Level unlocked. Congratulations!";
            }
            if(beeCount == 1)
            {
                msg += "\n(You can replay the 1st level (Past) to gather more bees)";
            }
            ScenesLoader.i_.FinishLevel(msg);
            Debug.Log("Finished trying");
        }catch (System.Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace + "\n" + e.InnerException);
            Debug.Log("Caught exception");
            ScenesLoader.i_.FinishLevel(msg);
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
        //MainCameraBehaviour.ShouldCameraFollowTheBee = false;
        while (Settings.s.beeFlyingSpeed < originalBeeSpeed)
        {
            Settings.s.beeFlyingSpeed = Mathf.Lerp(0.05f, originalBeeSpeed, TimePlayed / timeToSpeedUp);
            yield return new WaitForEndOfFrame();
        }
        Settings.s.beeFlyingSpeed = originalBeeSpeed;
        SelfDrivingBeeBehaviour.onReachingHome += BeeReachedHome;
        //MainCameraBehaviour.ShouldCameraFollowTheBee = true;
        // Dampen Camera following

    }
}
