using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingSceneBehaviour : MonoBehaviour
{

    public delegate void OnEndSceneBehaviour();
    public static OnEndSceneBehaviour onEndingStart;

    [SerializeField]
    [Tooltip("How fast it disolves in and out. Default: 5.0")]
    private float disolveSpeed = 5.0f;
    [SerializeField]
    [Tooltip("Default 1.0, can use to make the testing faster, does not affect disolve speed")]
    private float overAllSpeed = 1.0f;
    [SerializeField]
    [Tooltip("For how long each page should appear for, it is in seconds. List should be the same size as the pages")]
    private List<float> speedOfPages;
    [SerializeField]
    [Tooltip("Place them in the correct order, how you want them to appear")]
    private List<GameObject> pages;
    // Start is called before the first frame update
    [SerializeField]
    private int indexOfWholesomePage = 3;
    [SerializeField]
    private RawImage disolveImage;
    private Color disolveColor = Color.black;
    private Coroutine _endCoroutine;
    void Start()
    {
        Debug.Assert(pages.Count == speedOfPages.Count, "The sizes of pages and speeds are different.");
        Debug.Assert(disolveImage != null, "Disolve image could not be found");
        CalculateTheTotalTime();
        onEndingStart?.Invoke();
        _endCoroutine = StartCoroutine(StartEndingScene());
    }

    private void OnDestroy()
    {
        StopCoroutine(_endCoroutine);
    }

    private void CalculateTheTotalTime()
    {
        float totalTime = 0.0f;
        for (int i = 0; i < pages.Count; i++)
        {
            totalTime += (disolveSpeed * 2.0f) + (speedOfPages[i] * overAllSpeed);
        }
        Debug.Log("In total it will take: " + totalTime + " seconds to finish the ending scene");
    }

    private void EnablePage(int ind)
    {
       for(int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == ind);
        }
    }

    private IEnumerator StartEndingScene()
    {

        float timer = 0.0f;
        for (int pageInd = 0; pageInd < pages.Count; pageInd++)
        {
            if (pageInd == indexOfWholesomePage)
                MusicControls.i_.OnEndingGettingWholesome();

            pages[pageInd].transform.position = Vector3.zero;
            disolveImage.transform.parent = pages[pageInd].transform;
            EnablePage(pageInd);

            // Disolve the page in
            timer = 0.0f;
            disolveColor.a = 1.0f;
            disolveImage.color = disolveColor;
            while (timer < disolveSpeed)
            {
                disolveColor.a = Mathf.Lerp(1.0f, 0.0f, timer / disolveSpeed);
                disolveImage.color = disolveColor;
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            // Let the page showing
            yield return new WaitForSeconds(speedOfPages[pageInd] * overAllSpeed);

            // Disolve the page out
            timer = 0.0f;
            disolveColor.a = 0.0f;
            disolveImage.color = disolveColor;
            while (timer < disolveSpeed)
            {
                disolveColor.a = Mathf.Lerp(0.0f, 1.0f, timer / disolveSpeed);
                disolveImage.color = disolveColor;
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        ScenesLoader.i_.GoToTheMainMenu();
    }
}
