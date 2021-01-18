using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{

    [SerializeField]
    private Button yesButton;
    [SerializeField]
    private Button cancelButton;

    private void OnEnable()
    {
        Time.timeScale = 0.0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1.0f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
        yesButton.onClick.RemoveListener(GoToMenu);
        cancelButton.onClick.RemoveListener(Cancel);
    }

    void Start()
    {
        yesButton.onClick.AddListener(GoToMenu);
        cancelButton.onClick.AddListener(Cancel);
    }

   
    private void GoToMenu()
    {
        Time.timeScale = 1.0f;
        ScenesLoader.i_.GoToTheMainMenu();
    }

    private void Cancel()
    {
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
    }
}
