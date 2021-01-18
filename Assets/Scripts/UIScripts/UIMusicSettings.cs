using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMusicSettings : MonoBehaviour
{
    [SerializeField]
    private Slider _musicSlider;
    [SerializeField]
    private Slider _effectSlider;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(_musicSlider != null, "Music slider is not set up");
        Debug.Assert(_effectSlider != null, "Effects slider is not set up");

        // Set slider value to be the volume

        _musicSlider.onValueChanged.AddListener((float v) => UpdateMusicVolume(v));
        _effectSlider.onValueChanged.AddListener((float v) => UpdateEffectVolume(v));
    }

    private void OnDestroy()
    {
        _musicSlider.onValueChanged.RemoveListener((float v) => UpdateMusicVolume(v));
        _effectSlider.onValueChanged.RemoveListener((float v) => UpdateEffectVolume(v));


    }

    private void UpdateMusicVolume(float value)
    {
        
    }

    private void UpdateEffectVolume(float value)
    {

    }

}
