using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class DistanceSlider : MonoBehaviour
{
    private Slider _slider;
    private LevelProgression _levelProgression;
    void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.minValue = 0.0f;
        _slider.maxValue = 1.0f;
        _levelProgression = GameObject.FindObjectOfType<LevelProgression>();
        if(_levelProgression == null)
        {
            Debug.LogError("No level progression script found in the scene for the distance slider");
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        _slider.value = _levelProgression.Progress;
    }
}
