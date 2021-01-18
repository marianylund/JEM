using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    DragonFly = 0,
    Spider = 1,
    Hurricane = 2,
    CloudRain = 3,
    Fire = 4,
    ToxicGass = 5,
}

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _dragonfly;
    [SerializeField]
    private GameObject _spider;
    [SerializeField]
    private GameObject _hurricane;
    [SerializeField]
    private GameObject _cloudRain;
    [SerializeField]
    private ParticleSystem _fire;
    [SerializeField]
    private ParticleSystem _smallFire;
    [SerializeField]
    private ParticleSystem _toxicGass;


    public delegate void OnEnemyEvent(Vector3 position, EnemyType type);
    public static OnEnemyEvent onEnemyEliminated;
    public static OnEnemyEvent onEnemyHit;
    public static OnEnemyEvent onEnemyInView;

    private SpriteRenderer spriteRenderer;
    private ParticleSystem _particleSystem;

    public EnemyType CurrentEnemy { get; private set; }

    private EnemyPool pool;
    public int clicked { get; private set; } = 0;

    private Transform bee;
    private bool _hasHitBee = false;

    private GameObject[] obj;
    private bool _hasNotifiedInView = false;
    private FMOD.Studio.EventInstance enemySound;

    private void Start()
    {
        pool = GetComponentInParent<EnemyPool>();
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        bee = GameObject.FindGameObjectWithTag("MainBee")?.transform;
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        CheckSetUp();

        obj = new GameObject[] { _dragonfly, _spider, _hurricane, _cloudRain, _fire.transform.parent.gameObject, _toxicGass.transform.parent.gameObject };


        gameObject.tag = "Enemy";
        onEnemyEliminated += OnEnemyEliminated;
        SelfDrivingBeeBehaviour.onTakeDamage += _OnDamagingBee;
        ChangeEnemyOnLevel();
    }

    private void OnEnemyEliminated(Vector3 pos, EnemyType t)
    {
        clicked = 0;
    }

    private void ChangeEnemyOnLevel()
    {
        int ind = 0;
        switch (ScenesLoader.i_.CurrentLevel)
        {
            case Level.Past:
                ind = Random.Range((int)EnemyType.DragonFly, (int)EnemyType.Hurricane);
                SetObjActive(ind);
                break;
            case Level.Now:
                ind = Random.Range((int)EnemyType.DragonFly, (int)EnemyType.ToxicGass);
                SetObjActive(ind);
                break;
            case Level.Future:
                ind = Random.Range((int)EnemyType.Hurricane, (int)EnemyType.ToxicGass + 1);
                SetObjActive(ind);
                break;
        }

        enemySound = MusicControls.i_.GetNewEnemyMusic(CurrentEnemy);
    }

    private void StopPlayingSound(bool fadeOut = true)
    {
        //Debug.Log("Enemy of type: " + CurrentEnemy + " is stopping its sound. FadeOut: " + fadeOut);
        enemySound.stop(fadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void StartPlayingSound()
    {
        //Debug.Log("Enemy of type: " + CurrentEnemy + " is starting its sound");
        enemySound.start();
    }

    // TODO: fix randomness so it shows everything equally: https://docs.unity3d.com/2019.3/Documentation/Manual/RandomNumbers.html
    private void SetObjActive(int index)
    {
        CurrentEnemy = (EnemyType)index;
        for (int i = 0; i < obj.Length; i++)
        {
            if (i == index)
            {
                obj[i].SetActive(true);
            }
            else
                obj[i].SetActive(false);
        }

        if(CurrentEnemy == EnemyType.Fire)
        {
            _fire.Play();
        }else if(CurrentEnemy == EnemyType.ToxicGass)
        {
            _toxicGass.Play();
        }
    }

    private void CheckSetUp()
    {
        Debug.Assert(_particleSystem != null, "Particle System is null!");
        Debug.Assert(_dragonfly != null, "Dragonfly is not set up!");
        Debug.Assert(_spider != null, "Spider is not set up!");
        Debug.Assert(_hurricane != null, "Hurricane is not set up!");
        Debug.Assert(spriteRenderer != null, "spriteRenderer is not set up!");
        Debug.Assert(pool != null, "Pool is not set up!");
        Debug.Assert(bee != null, "Could not find the MainBee!");
    }

    private void OnDestroy()
    {
        SelfDrivingBeeBehaviour.onTakeDamage -= _OnDamagingBee;
        onEnemyEliminated -= OnEnemyEliminated;
        StopPlayingSound(fadeOut: true);
    }

    private void _OnDamagingBee(int energy, Vector3 position)
    {
        _hasHitBee = true;
    }

    private void OnEnable()
    {
        //spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        //Color newColor = spriteRenderer.color;
        //newColor.a = 1.0f;
        //spriteRenderer.color = newColor;
        //clicked = 0;
        //_hasHitBee = false;
    }

    private void Update()
    {
        if (!_hasNotifiedInView && MainCameraBehaviour.IsWithinView(transform.position))
        {
            _hasNotifiedInView = true;
            onEnemyInView?.Invoke(transform.position, CurrentEnemy);
            StartPlayingSound();
        }else if (_hasNotifiedInView && !MainCameraBehaviour.IsWithinView(transform.position))
        {
            _hasNotifiedInView = false;
            StopPlayingSound(fadeOut: true);
        }
    }

    private void FixedUpdate()
    {
        Fly();
    }

    public void OnMouseDown()
    {
        EnemyGetHit();
    }

    public void EnemyGetHit()
    {
        clicked += 1;
        MusicControls.i_.PlayOnEnemyHit(transform.position, CurrentEnemy);
        onEnemyHit?.Invoke(transform.position, CurrentEnemy);
        if (_particleSystem.isPlaying){
            _particleSystem.Clear();
            _particleSystem.Stop();
        }
        _particleSystem.Play();
        //ChangeOpacity();

        if (CurrentEnemy == EnemyType.Fire)
        {
            _fire.Stop();
            _smallFire.Play();
        }
        if (clicked > Settings.s.clicksToEliminateEnemy - 1)
        {
            Eliminated();
        }
    }

    private void ChangeOpacity()
    {
        //Color newColor = spriteRenderer.color;
        //float rest = Settings.s.clicksToEliminateEnemy - clicked;
        //float rest_per = rest / Settings.s.clicksToEliminateEnemy;
        //newColor.a = rest_per + 0.1f;
        //spriteRenderer.color = newColor;
    }

    private void Eliminated()
    {
        if (CurrentEnemy == EnemyType.Fire)
        {
            _fire.Stop();
            _smallFire.Stop();
        }
        else if (CurrentEnemy == EnemyType.ToxicGass)
        {
            _toxicGass.Stop();
        }
        StopPlayingSound(fadeOut: false);
        onEnemyEliminated?.Invoke(transform.position, CurrentEnemy);
        _hasNotifiedInView = false;
        pool.ReturnToPool(this);
        ChangeEnemyOnLevel();
    }

    private void Fly()
    {
        // Move our position a step closer to the target.
        float step = Settings.s.enemyMovementSpeed * Time.deltaTime; // calculate distance to move
        if(_hasHitBee) // If you already flew past the bee, just fly to the left
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left * 100.0f, step);
        else
            transform.position = Vector3.MoveTowards(transform.position, bee.position.With(z:transform.position.z), step);
    }

}
