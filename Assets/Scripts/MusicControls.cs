using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControls : MonoBehaviour
{
    public static MusicControls i_ { get; private set; } // special thing that lets you refer to music control from all scripts

    [FMODUnity.BankRef]
    public List<string> Banks;
    // bank:/Master
    // bank:/Background

    [FMODUnity.EventRef] //Easier to choose event
    public string BackgroundPastLevel = "event:/BackgroundPast"; //Path to background sound
    public string BackgroundNowLevel = "event:/BackgroundNow"; //Change to sound fit for now level
    public string BackgroundFutureLevel = "event:/BackgroundFuture"; //Change to sound fit for future level
    public string Bee = "event:/Bee"; //Path to sound of one bee TODO: Code: Add bee sound to bee
    public string Beehive = "event:/Bees"; //Path to sound for multiple bees
    public string PlantSeed = "event:/Plant seed"; //Path to sound when the seed is planted 
    public string FlowerGrows = "event:/Flower grows"; //Path to sound when the flower is growing
    public string PickUpFlower = "event:/Pick up flower"; //Path to sound when bee is picking up flower
    public string RemoveClouds = "event:/Remove clouds"; //Path to sound when clouds are removed
    public string Enemy = "event:/Enemy"; //Path to sound for hit enemy
    public string Dragonfly = "event:/Dragonfly"; //Path to sound for dragonfly
    public string Spider = "event:/Spider"; //Path to sound for spider
    public string Rain = "event:/Rain"; //Path to sound for rain
    public string Hurricane = "event:/Hurricane"; //Path to sound for hurricane
    public string ToxicGas = "event:/ToxicGas"; //Path to sound for toxic gas
    public string Fire = "event:/Fire"; //Path to sound for fire 
    public string GetHitByEnemy = "event:/Get hit by enemy"; //Path to sound of bee getting hit by enemy
    public string LowOnEnergy = "event:/Low on energy"; //Path to sound when bee is low on enegy
    public string ABeeDies = "event:/ABeeDies"; //Path to sound when a bee dies
    public string LevelComplete = "event:/LevelComplete"; //Path to sound for level completed
    public string GameOver = "event:/GameOver"; //Path to sound for game over
    public string DramaticEnding = "event:/DramaticEnding"; //Path to the second sound for game over
    public string Hover = "event:/Hover"; //Path to hover sound for button
    private FMOD.Studio.EventInstance musicEv;
    private FMOD.Studio.EventInstance beeSounds;
    private FMOD.Studio.EventInstance beehiveSounds;

    private bool HasLoaded = false;

    private Coroutine _loadingCoroutine;

    void Awake()
    {
        // Making so that there is only one music controls at all times
        if (MusicControls.i_ == null)
        {
            Debug.Log("Setting music control's instance");
            MusicControls.i_ = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            UnityEngine.Object.Destroy(gameObject);
        }

        // Start loading the music controller on the side
        if (_loadingCoroutine == null && !HasLoaded)
            _loadingCoroutine = StartCoroutine(LoadAndSubscribe());
        else
            Debug.LogWarning("Music Controls has already coroutine for subscribing");
    }

    private IEnumerator LoadAndSubscribe()
    {
        //// Start loading all the sounds needed on the sound
        //if (!HasLoaded)
        //{
        //    foreach (var bank in Banks)
        //    {
        //        FMODUnity.RuntimeManager.LoadBank(bank, true);
        //        Debug.Log("Setting Bank to loading: " + bank);
        //    }
        //    HasLoaded = true;
        //}

        // Keep yielding the co-routine until all the Bank loading is done
        while (FMODUnity.RuntimeManager.AnyBankLoading())
        {
            Debug.Log("Waiting for banks to load");
            yield return null;
        }

        // When you are finished, subscribe to events, start music special to the level
        _loadingCoroutine = null;

        if (ScenesLoader.i_ != null)
            OnSceneChanging(ScenesLoader.i_.CurrentLevel);
        else
            Debug.LogWarning("There is not ScenesLoader in the scene for MusicControls.");
    }

    public FMOD.Studio.EventInstance GetNewEnemyMusic(EnemyType enemy)
    {
        switch (enemy)
        {
            case EnemyType.DragonFly:
                return FMODUnity.RuntimeManager.CreateInstance(Dragonfly);
            case EnemyType.Spider:
                return FMODUnity.RuntimeManager.CreateInstance(Spider);
            case EnemyType.CloudRain:
                return FMODUnity.RuntimeManager.CreateInstance(Rain);
            case EnemyType.Hurricane:
                return FMODUnity.RuntimeManager.CreateInstance(Hurricane);
            case EnemyType.ToxicGass:
                return FMODUnity.RuntimeManager.CreateInstance(ToxicGas);
            case EnemyType.Fire:
                return FMODUnity.RuntimeManager.CreateInstance(Fire);
            default:
                Debug.LogError("Enemy type not found: " + enemy);
                return FMODUnity.RuntimeManager.CreateInstance(Enemy);
        }
    }

    public FMOD.Studio.EventInstance GetLowOnEnergySound()
    {
        return FMODUnity.RuntimeManager.CreateInstance(LowOnEnergy);
    }


    private void OnSceneChanging(Level lvl)
    {
        switch (lvl)
        {
            case Level.Loading:
                StartCoroutine(OnLoadingScene());
                break;
            case Level.Past:
                OnPastLevel();
                break;
            case Level.Now:
                OnNowLevel();
                break;
            case Level.Future:
                OnFutureLevel();
                break;
            case Level.Menu:
                OnChangingToMenu();
                break;
            case Level.Ending:
                OnChangingToEnding();
                break;
            default:
                break;
        }
    }

    private IEnumerator OnLoadingScene()
    {
        yield return new WaitUntil(() => { return FMODUnity.RuntimeManager.HasBanksLoaded && FMODUnity.RuntimeManager.IsInitialized && !FMODUnity.RuntimeManager.AnyBankLoading(); });

        FMODUnity.RuntimeManager.WaitForAllLoads();

        SubscribeOnAllLevels();
        yield return new WaitForSeconds(3.0f);
        ScenesLoader.i_.GoToTheMainMenu();
    }

    private void PlayOnLevelFinished(float milestone)
    {
        if(ScenesLoader.i_.CurrentLevel != Level.Future) // Do not play happy sound for the future level
            FMODUnity.RuntimeManager.PlayOneShot(LevelComplete); // Sound when bee has reached the beehive
    }

    public void PlayOnHover()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Hover);
    }

    private void PlayOnLevelFailed(float milestone)
    {
        FMODUnity.RuntimeManager.PlayOneShot(GameOver); // Sound when the last bee died and level failed
    }

    private void PlayOnEndReached(float milestone)
    {
        beehiveSounds = FMODUnity.RuntimeManager.CreateInstance(Beehive);
        beehiveSounds.start(); // Sound when bee has reached the last part of the screen, where the camera stops moving, maybe start playing beehive sound? TODO: Stop the sound?
    }

    private void PlayOnMilestoneReached(float milestone)
    {
        if (milestone == 0.1f) // Finished 10% of the level
        {
            beehiveSounds.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // Stop playing beehive sounds, since the bee has moved away from the beehive
        }
        else if (milestone == 0.9f) // Finished 90% of the level
        {

        }
    }
   

    private void PlayOnEnemyEliminated(Vector3 position, EnemyType type)
    {
        // TODO: Add sound when enemy has been successfully eliminated ? 

    }

    public void PlayOnEnemyHit(Vector3 position, EnemyType type)
    {
        FMODUnity.RuntimeManager.PlayOneShot(Enemy, transform.position);
    }

    private void PlayOnSeedGrownToFlower(Vector3 position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(FlowerGrows, position);
    }

    private void PlayOnBeeDie(Vector3 position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(ABeeDies, position);
    }
    private void PlayOnSeedPlanted(Vector3 position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(PlantSeed, position);
    }

    private void PlayOnGettingEnergy(int energy, Vector3 position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(PickUpFlower, position);
    }

    private void PlayOnTakeDamage(int damage, Vector3 position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(GetHitByEnemy, position);
    }

    private void PlayOnCloudedChange(bool isClouded)
    {
        if (isClouded)
        {

        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot(RemoveClouds);
        }
    }

    private void SubscribeOnAllLevels()
    {
        // Subscribe to all events that are the same for all the level.
        // Meaning since there are flowers growing and enemies being hit in both past, now and future levels

        ScenesLoader.onChangingScene += OnSceneChanging;

       
        EnemyBehaviour.onEnemyEliminated += PlayOnEnemyEliminated;
        //EnemyBehaviour.onEnemyHit += PlayOnEnemyHit; -> Moved to enemy to avoid the delay, if everything works, then can delete this line
        SeedFlowerBehaviour.onSeedGrownToFlower += PlayOnSeedGrownToFlower;

        SelfDrivingBeeBehaviour.onGettingEnergy += PlayOnGettingEnergy;
        LevelProgression.onMilestoneReached += PlayOnMilestoneReached;

        LevelProgression.onEndReached += PlayOnEndReached;
        LevelProgression.onLevelFinished += PlayOnLevelFinished;
        LevelProgression.onLevelFailed += PlayOnLevelFailed;

        BeehiveBehaviour.onBeeDie += PlayOnBeeDie;
        SelfDrivingBeeBehaviour.onTakeDamage += PlayOnTakeDamage;
        FallingSeedBehaviour.onSeedPlanted += PlayOnSeedPlanted;

        SunBehaviour.onCloudedChange += PlayOnCloudedChange;

    }

    private void OnPastLevel()
    {
        // All the things that need to happen when you start Past lvl. 
        // For now just start the correct background sound
        PlayPastLevelMusic();

        beehiveSounds = FMODUnity.RuntimeManager.CreateInstance(Beehive); // make to beehive
        beehiveSounds.start();
    }

    private void PlayPastLevelMusic()
    {
        try
        {
            if (musicEv.isValid())
            {
                musicEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
            musicEv = FMODUnity.RuntimeManager.CreateInstance(BackgroundPastLevel);
            if (musicEv.isValid())
            {
                musicEv.start();
            }
            else
            {
                Debug.Log("Tried to start past level music, but it was not valid");
            }
        }catch(Exception e)
        {
            Debug.LogError("Could not find background past level" + e.Message + "\n" + e.StackTrace + "\n" + e.InnerException);
        }
    }

    private void OnNowLevel()
    {
        // All the things that need to happen when you start Now lvl
        PlayNowLevelMusic();

        beehiveSounds = FMODUnity.RuntimeManager.CreateInstance(Beehive); // make to beehive
        beehiveSounds.start();
    }

    private void PlayNowLevelMusic()
    {
        musicEv = FMODUnity.RuntimeManager.CreateInstance(BackgroundNowLevel);
        musicEv.start();
    }

    private void OnFutureLevel()
    {
        // All the things that need to happen when you start Future lvl
        PlayFutureLevelMusic();

        beehiveSounds = FMODUnity.RuntimeManager.CreateInstance(Beehive); // make to beehive
        beehiveSounds.start();
    }

    private void PlayFutureLevelMusic()
    {
        musicEv = FMODUnity.RuntimeManager.CreateInstance(BackgroundFutureLevel);
        musicEv.start();
    }

    private void OnChangingToMenu()
    {
        // All the things that need to happen when changing to menu scene
        if (musicEv.isValid()) // if some music is set up
        {
            musicEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // stop it
        }

        if (beehiveSounds.isValid())// Here you can add some background music for the menu as well
            beehiveSounds.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        if (Settings.s)
        {
            if (Settings.s.UNLOCKED_LEVELS.Contains(Level.Future))
            {
                PlayFutureLevelMusic();
            }
            else if (Settings.s.UNLOCKED_LEVELS.Contains(Level.Now))
            {
                PlayNowLevelMusic();
            }
            else if (Settings.s.UNLOCKED_LEVELS.Contains(Level.Past))
            {
                PlayPastLevelMusic();
            }
        }
        else
        {
            PlayPastLevelMusic();
        }

        
    }

    private void OnChangingToEnding()
    {
        if (musicEv.isValid()) // if some music is set up
        {
            musicEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); // stop it
        }

        musicEv = FMODUnity.RuntimeManager.CreateInstance(DramaticEnding);
        musicEv.start(); // Dramatic ending sound starts
    }

    public void OnEndingGettingWholesome()
    {
        Debug.Log("Ending getting wholesome, playing that");
        musicEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        PlayOnLevelFinished(1.0f);
        if(!beehiveSounds.isValid())
            beehiveSounds = FMODUnity.RuntimeManager.CreateInstance(Beehive); // make to beehive
        beehiveSounds.start();
    }

    private void OnDestroy()
    {
        // All the things that need to happen when this object is destroyed
        // It is important to unsubcribe to all the events
        if (_loadingCoroutine != null)
            StopCoroutine(_loadingCoroutine);

        EnemyBehaviour.onEnemyEliminated -= PlayOnEnemyEliminated;
        //EnemyBehaviour.onEnemyHit -= PlayOnEnemyHit;
        SeedFlowerBehaviour.onSeedGrownToFlower -= PlayOnSeedGrownToFlower;

        SelfDrivingBeeBehaviour.onGettingEnergy -= PlayOnGettingEnergy;
        LevelProgression.onMilestoneReached -= PlayOnMilestoneReached;

        LevelProgression.onEndReached -= PlayOnEndReached;
        LevelProgression.onLevelFinished -= PlayOnLevelFinished;
        LevelProgression.onLevelFailed -= PlayOnLevelFailed;

        BeehiveBehaviour.onBeeDie -= PlayOnBeeDie;
        SelfDrivingBeeBehaviour.onTakeDamage -= PlayOnTakeDamage;
        FallingSeedBehaviour.onSeedPlanted -= PlayOnSeedPlanted;

        SunBehaviour.onCloudedChange -= PlayOnCloudedChange;

    }
}