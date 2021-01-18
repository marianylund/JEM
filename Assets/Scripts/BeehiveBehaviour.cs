using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeehiveBehaviour : MonoBehaviour
{
    [SerializeField]
    private Slider _energySlider;
    [SerializeField]
    private Image _fill;

    private Transform _mainBee;
    private List<Transform> _otherBees;
    private SelfDrivingBeeBehaviour _beeBehaviour;

    public delegate void OnBeehiveEvent(Vector3 pos);
    public static OnBeehiveEvent onBeeDie;
    public static OnBeehiveEvent onLastBeeDie;

    private float timer_ = 0.0f;

    private FMOD.Studio.EventInstance lowOnEnergySound;
    private bool hasNotifiedAboutLowEnergy = false;
    private float lowEnergyValue = 0.2f;

    public int NumberOfBees { get => _otherBees.Count + 1; }
    void Start()
    {
        _otherBees = new List<Transform>();
        _beeBehaviour = GetComponent<SelfDrivingBeeBehaviour>();
        lowOnEnergySound = MusicControls.i_.GetLowOnEnergySound();

        FindBeesInChildren();
        CheckSetUp();

        _energySlider.maxValue = Settings.s.beeMaxEnergy;
        SelfDrivingBeeBehaviour.onTakeDamage += _LooseABee;
        SelfDrivingBeeBehaviour.onGettingEnergy += _GetEnergy;
        CheckNumberOfBees();

        lowEnergyValue = Settings.s.beeSliderGradient.colorKeys[0].time * _energySlider.maxValue;
    }

    void Update()
    {
        timer_ += Time.deltaTime;
        if (timer_ > Settings.s.beeLoosingEnergySpeed)
        {
            timer_ = 0.0f;
            UpdateEnergySliderValue(-1.0f);
            //_energySlider.value -= 1;
        }
    }

    private void CheckNumberOfBees()
    {
        if (ScenesLoader.i_.CurrentLevel == Level.Past)
            Settings.s.NUMBER_OF_BEES = Settings.s.numberOfBeesToStartWith;
        if (NumberOfBees > Settings.s.NUMBER_OF_BEES){
            // Remove bees
            int beesToRemove = NumberOfBees - Settings.s.NUMBER_OF_BEES;
            //Debug.Log("Beehive has " + NumberOfBees + ", but need only " + Settings.s.NUMBER_OF_BEES);
            for (int i = 0; i < beesToRemove; i++)
            {
                int index = Random.Range(0, _otherBees.Count);
                //Debug.Log("Removing " + _otherBees[index]);
                GameObject.Destroy(_otherBees[index].gameObject);
                _otherBees.RemoveAt(index);
            }
        }
        else if (NumberOfBees < Settings.s.NUMBER_OF_BEES)
        {
            Debug.LogError("Beehive has less bee children than number of bees in the settings");
        }
        // Else do not do anything
    }

    private void UpdateEnergySliderValue(float addValue)
    {
        _energySlider.value += addValue;
        _fill.color = Settings.s.beeSliderGradient.Evaluate(_energySlider.normalizedValue);

        if (_energySlider.value <= 0)
            _OutOfEnergy();
        else if (!hasNotifiedAboutLowEnergy & _energySlider.value <= lowEnergyValue)
        {
            hasNotifiedAboutLowEnergy = true;
            lowOnEnergySound.start();
        }
        else if (hasNotifiedAboutLowEnergy & _energySlider.value > lowEnergyValue)
        {
            hasNotifiedAboutLowEnergy = false;
            lowOnEnergySound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    private void _GetEnergy(int energy, Vector3 pos = new Vector3())
    {
        UpdateEnergySliderValue(energy);
        //_energySlider.value += energy;
    }

    private void _OutOfEnergy()
    {
        _LooseABee(1, transform.position);
        if (NumberOfBees >= 1)
        {
            _GetEnergy(Settings.s.onLoosingBeeRestoreEnergy);
        }
    }

    private void _LooseABee(int damage, Vector3 pos)
    {
        SimpleBee dyingBee = null;
        if (NumberOfBees == 1)
        {
            // Game Over
            Debug.Log("Game OVER!");
            dyingBee = _mainBee.GetComponent<SimpleBee>();
            Debug.Assert(dyingBee != null, "Could not get a Simple Bee from the main bee " + _mainBee.name);
            MainCameraBehaviour.ShouldCameraFollowTheBee = false;
            onLastBeeDie?.Invoke(dyingBee.transform.position);
            dyingBee.RIP();
        }
        else
        {
            for (int i = 0; i < damage; i++)
            {
                int index = Random.Range(0, _otherBees.Count);
                dyingBee = _otherBees[index].GetComponent<SimpleBee>();
                Debug.Assert(dyingBee != null, "Could not get a Simple Bee from " + _otherBees[index].name);
                _otherBees.RemoveAt(index);
                onBeeDie?.Invoke(dyingBee.transform.position);
                dyingBee.RIP();
            }
        }

    }

    private void CheckSetUp()
    {
        Debug.Assert(_otherBees.Count != 0, "No other bees found!");
        Debug.Assert(lowOnEnergySound.isValid(), "Low on energy sound is not set up correctly in Beehive");
        Debug.Assert(_mainBee != null, "No main bee found. Remember that object needs to be called MainBee");
        Debug.Assert(_beeBehaviour != null, "No bee behaviour found on the same gameObject");
        Debug.Assert(_energySlider != null, "No energy slider is set up for the beehive");
        Debug.Assert(_fill != null, "No energy slider fill is set up in the BeehiveBehaviour");

    }

    private void FindBeesInChildren()
    {
        foreach (Transform child in transform)
        {
            if (child.name.ToLower().Contains("mainbee"))
            {
                _mainBee = child;
            }
            else
            {
                _otherBees.Add(child);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (Transform child in transform)
        {
            Destroy(child);
        }
        _otherBees?.Clear();
        SelfDrivingBeeBehaviour.onTakeDamage -= _LooseABee;
        SelfDrivingBeeBehaviour.onGettingEnergy -= _GetEnergy;
        lowOnEnergySound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

}
