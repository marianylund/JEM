using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    [SerializeField]
    GameObject MainMenu;
    [SerializeField]
    GameObject FailedMenu;

    [SerializeField]
    TMP_Text FinishedText;

    [SerializeField]
    Button PlayButton_Past;
    [SerializeField]
    Button PlayButton_Now;
    [SerializeField]
    Button PlayButton_Future;
    [SerializeField]
    Button PlayButton_Now_Locked;
    [SerializeField]
    Button PlayButton_Future_Locked;

    [SerializeField]
    Button RetryButton_Failed;

    public const string SLOGAN = "It's a tough time to bee alive - only you can help the bees survive";

    void Start()
    {
        if (MainMenu == null || FailedMenu == null)
            Debug.LogError("Menus are not set up in MenuBehaviour");
        string status = ScenesLoader.i_._MenuStatus;
        switch (status)
        {
            case "MainMenu":
                ShowMainMenu();
                break;
            case "FailedMenu":
                ShowFailedMenu();
                break;
            case "FinishedMenu":
                ShowFinishedMenu();
                break;
            default:
                ShowMainMenu();
                Debug.LogWarning("Cannot find this type of menu: " + status);
                break;
        }

        PlayButton_Past.onClick.AddListener(() => { ScenesLoader.i_.PlayPastLevel(); });
        PlayButton_Now.onClick.AddListener(() => { ScenesLoader.i_.PlayNowLevel(); });
        PlayButton_Future.onClick.AddListener(() => { ScenesLoader.i_.PlayFutureLevel(); });
        RetryButton_Failed.onClick.AddListener(() => { ScenesLoader.i_.ReplayLevel(); });
        //MainMenuButton.onClick.AddListener(() => { ShowMainMenu(); });
       // RetryButton_Finished.onClick.AddListener(() => { ScenesLoader.i_.ReplayLevel(); });

        ShowLevels();
    }

    private void ShowLevels()
    {
        if(Settings.s.UNLOCKED_LEVELS.Count == 1)
        {
            PlayButton_Now_Locked.gameObject.SetActive(true);
            PlayButton_Now.gameObject.SetActive(false);
            PlayButton_Future_Locked.gameObject.SetActive(true);
            PlayButton_Future.gameObject.SetActive(false);
        }
        else
        {
            foreach (Level lvl in Settings.s.UNLOCKED_LEVELS)
            {
                switch (lvl)
                {
                    case Level.Past:
                        break;
                    case Level.Now:
                        PlayButton_Now_Locked.gameObject.SetActive(false);
                        PlayButton_Now.gameObject.SetActive(true);
                        break;
                    case Level.Future:
                        PlayButton_Future_Locked.gameObject.SetActive(false);
                        PlayButton_Future.gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void ShowMainMenu()
    {
        FinishedText.text = ScenesLoader.i_._MSG;
        MainMenu.SetActive(true);
        FailedMenu.SetActive(false);
    }

    public void ShowFailedMenu()
    {
        MainMenu.SetActive(false);
        FailedMenu.SetActive(true);
    }

    public void ShowFinishedMenu()
    {
        string msg = ScenesLoader.i_._MSG;
        FinishedText.text = msg;
        MainMenu.SetActive(true);
        FailedMenu.SetActive(false);
    }
}