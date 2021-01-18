using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SunBehaviour : MonoBehaviour
{
    public delegate void OnSunEvent(bool isClouded);
    public static OnSunEvent onCloudedChange;

    private SpriteRenderer m_SpriteRenderer;
    public bool IsClouded { get; private set; } = false;
    [SerializeField]
    private RawImage _darkerEffect;
    private Color _darkerColor;
    private Coroutine _darkenRoutine;
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Sun");
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _darkerColor = Settings.s.darkenEffectColor;
        _darkerColor.a = 0.0f;
        _darkerEffect.color = _darkerColor;

        if(ScenesLoader.i_.CurrentLevel == Level.Future)
        {
            LevelProgression.onLevelFinished += DarkenOnFutureLevelEnd;
        }
    }

    private void OnDisable()
    {
        if (ScenesLoader.i_.CurrentLevel == Level.Future)
        {
            LevelProgression.onLevelFinished -= DarkenOnFutureLevelEnd;
        }
    }

    private void DarkenOnFutureLevelEnd(float m)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        if (_darkenRoutine != null)
        {
            StopCoroutine(_darkenRoutine);
        }
        _darkenRoutine = StartCoroutine(ChangeToBlack());
    }

    private IEnumerator ChangeToBlack()
    {
        float timer = 0.0f;
        float effectSpeed = Settings.s.darkenEffectSpeed*3.0f;
        Color startColor = _darkerColor;
        Color finishColor = Color.black;
        while (timer < effectSpeed)
        {
            _darkerColor = Color.Lerp(startColor, finishColor, timer / effectSpeed);
            _darkerEffect.color = _darkerColor;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }


    private IEnumerator ChangeDarknessRoutine(float from, float to)
    {
        float effectSpeed = Settings.s.darkenEffectSpeed;
        float startAlpha = _darkerColor.a;
        float timer = Mathf.InverseLerp(from, to, startAlpha) * effectSpeed;
        while (timer < effectSpeed)
        {
            _darkerColor.a = Mathf.Lerp(startAlpha, to, timer / effectSpeed); 
            _darkerEffect.color = _darkerColor;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void ChangeDarkness(float from, float to)
    {
        if(_darkenRoutine != null)
        {
            StopCoroutine(_darkenRoutine);
        }
        _darkenRoutine = StartCoroutine(ChangeDarknessRoutine(from, to));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeCloudedStatus(true);
        ChangeDarkness(0.0f, Settings.s.darkenEffectMaxAlpha);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        ChangeCloudedStatus(false);
        ChangeDarkness(Settings.s.darkenEffectMaxAlpha, 0.0f);
    }

    private void ChangeCloudedStatus(bool changeTo)
    {
        IsClouded = changeTo;
        m_SpriteRenderer.color = IsClouded ? Color.gray : Color.white;
        onCloudedChange?.Invoke(IsClouded);
    }
}
