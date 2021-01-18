using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedFlowerBehaviour : MonoBehaviour
{
    public delegate void OnSeedFlowerEvent(Vector3 position);
    public static OnSeedFlowerEvent onSeedGrownToFlower;

    public int amountOfEnergyFlowerGives { get; private set; }
    public bool hasGrown { get; private set; } = false;
    public bool CanPickUp { get; private set; } = false;

    private SunBehaviour sun;
    private FlowerSeedPool pool;
    private Animator _animator;
    private ParticleSystem _particleSystem;

    [SerializeField]
    private GameObject[] obj;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        amountOfEnergyFlowerGives = Settings.s.flowerRestoresEnergy;
        pool = GetComponentInParent<FlowerSeedPool>();
        sun = GameObject.FindGameObjectWithTag("Sun")?.GetComponent<SunBehaviour>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _CheckTheSetUp();

        ChangeToRandomFlower();
        _CheckIfTheSunIsOut();
    }

    private void _CheckTheSetUp()
    {

        Debug.Assert(_animator != null, "Animator Controller is null!");
        Debug.Assert(_particleSystem != null, "PArticle System is null!");
        Debug.Assert(pool != null, "pool is null!");
        Debug.Assert(sun != null, "sun is null!");
        Debug.Assert(obj.Length == 3, "There only " + obj.Length + " children in flowers ");
    }

    private void ChangeToRandomFlower()
    {
        int ind = Random.Range(0, 3);
        SetObjActive(ind);
    }

    private void SetObjActive(int index)
    {
        for (int i = 0; i < obj.Length; i++)
        {
            if (i == index)
            {
                obj[i].SetActive(true);
                _animator = obj[i].GetComponent<Animator>();
            }
            else
                obj[i].SetActive(false);
        }
    }

    private void _CheckIfTheSunIsOut()
    {
        if (!(sun?.IsClouded ?? true)) // if there is a sun and it is not clouded
        {
            _Bloom(true, " sun is out and not clouded: " + sun.IsClouded);
        }

        if (!hasGrown)
            SunBehaviour.onCloudedChange += _OnCloudedChange;
    }

    private void _Bloom(bool set, string msg)
    {
        // Debug.Log("Setting bloom to: " + set + " " + msg);
        _animator?.SetBool("ShouldBloom", set);
        hasGrown = _animator?.GetBool("ShouldBloom") ?? false;
        CanPickUp = hasGrown;
        if (hasGrown)
            onSeedGrownToFlower?.Invoke(transform.position);
    }

    private void OnEnable()
    {
        _CheckIfTheSunIsOut();
    }

    private void _OnCloudedChange(bool gotClouded)
    {
        if (!gotClouded && !hasGrown) gotSun();
    }

    public void PickUp()
    {
        Debug.Assert(hasGrown, "Flower has not grown but got picked up!");
        Debug.Assert(CanPickUp, "Flower should not be picked up!");
        // TODO: Some animation for picking up the flower?
        _particleSystem.Play();
        CanPickUp = false;
    }

    public void ReturnToPool()
    {
        _Disable();
    }


    private void gotSun()
    {
        Debug.Assert(!hasGrown, "Flower has grown but still reacts to sun!");

        _Bloom(true, " got sun");
        onSeedGrownToFlower?.Invoke(transform.position);
        SunBehaviour.onCloudedChange -= _OnCloudedChange;
    }

    private void _Disable()
    {
        _Bloom(false, "Turning off on _Disable");
        SunBehaviour.onCloudedChange -= _OnCloudedChange;
    }

    private void OnDestroy()
    {
        _Disable();
    }

    private void OnDisable()
    {
        ChangeToRandomFlower();
        _Disable();
    }
}
