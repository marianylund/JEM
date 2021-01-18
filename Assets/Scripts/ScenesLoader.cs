using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesLoader : MonoBehaviour
{
    public static ScenesLoader i_;
    public string _MenuStatus { get; private set; } = "MainMenu";
    public string _MSG { get; private set; } = "";
    private List<string> scenesInBuild;
    private string previousScene = "MainMenu";
    public Level CurrentLevel = Level.Menu;

    public const string pastSceneName = "PastLevelScene";
    public const string nowSceneName = "NowLevelScene";
    public const string futureSceneName = "FutureLevelScene";
    public const string startSceneName = "MenuScene";
    public const string endingSceneName = "EndingScene";
    public const string loadingSceneName = "LoadingScene";

    public delegate void OnScenesLoader(Level lvl);
    public static OnScenesLoader onChangingScene;

    private void Awake()
    {
        if (ScenesLoader.i_ == null)
        {
            ScenesLoader.i_ = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }

    public static Level GetLevelFromScene(string scene)
    {
        switch (scene)
        {
            case pastSceneName:
                return Level.Past;
            case nowSceneName:
                return Level.Now;
            case futureSceneName:
                return Level.Future;
            case startSceneName:
                return Level.Menu;
            case endingSceneName:
                return Level.Ending;
            case loadingSceneName:
                return Level.Loading;
            default:
                return Level.Menu;
        }
    }

    private void Start()
    {
        scenesInBuild = new List<string>();
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            int lastSlash = scenePath.LastIndexOf("/");
            scenesInBuild.Add(scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1));
        }
        previousScene = SceneManager.GetActiveScene().name;
    }

    public void PlayPastLevel()
    {
        _LoadScene(pastSceneName);
    }

    public void PlayNowLevel()
    {
        _LoadScene(nowSceneName);
    }

    public void PlayFutureLevel()
    {
        _LoadScene(futureSceneName);
    }

    public void GoToTheMainMenu(string msg = MenuBehaviour.SLOGAN, float waitTime = 0.2f)
    {
        _LoadScene(startSceneName, waitTime);
        _MSG = msg;
        _MenuStatus = "MainMenu";
    }

    public void LoadEnding()
    {
        Debug.Log("Scenes loader got that we need to load the ending");
        _LoadScene(endingSceneName, 6.0f);
    }

    public void ReplayLevel()
    {
        Debug.Log("Trying to replay level: " + previousScene);
        _LoadScene(previousScene);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void FinishLevel(string msg)
    {
        Debug.Log("Scenes loader got that level is finished");
        _LoadScene(startSceneName, 2.0f);
        _MSG = msg;
        _MenuStatus = "FinishedMenu";
    }

    public void FailLevel()
    {
        Debug.Log("Scenes loader got that we need to fail the level");
        _LoadScene(startSceneName, 2.0f);
        _MenuStatus = "FailedMenu";
    }

    private bool SceneExist(string sceneName)
    {
        return scenesInBuild.Contains(sceneName);
    }

    private void _LoadScene(string sceneName, float waitTime = 0.0f)
    {
        if (SceneExist(sceneName))
        {
            CurrentLevel = GetLevelFromScene(sceneName);
            onChangingScene?.Invoke(CurrentLevel);
            if (waitTime > 0.0f)
                StartCoroutine(LoadSceneOffset(waitTime, sceneName));
            else
                _ChangeScene(sceneName);
        }
        else
            Debug.LogError("No scene found: " + sceneName);
    }

    private void _ChangeScene(string sceneName)
    {
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneOffset(float waitTime, string sceneName)
    {
        yield return new WaitForSeconds(waitTime);
        _ChangeScene(sceneName);
    }
}
