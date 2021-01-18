using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudBehaviour : MonoBehaviour
{
    private bool isDragging;
    public delegate void OnCloudEvent(Vector3 position);
    public static OnCloudEvent onCloudGrabbed;
    public static OnCloudEvent onCloudDropped;
    private SunBehaviour sun;

    private bool onTutorial = false;

    [SerializeField]
    private Image _greyCloud;
    [SerializeField]
    private GameObject _rain;

    public bool ShouldRain { get; private set; } = false;
    public bool IsRaining { get; private set; } = false;
    private float _rainTimer = 0.0f;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Clouds");
        sun = GameObject.FindObjectOfType<SunBehaviour>();
        if(sun == null)
        {
            Debug.LogError("No sun to move towards");
            gameObject.SetActive(false);
        }

        ShouldRain = false; // Raining only in now and future levels
        if (ShouldRain)
            Debug.Assert(_greyCloud != null, "No grey cloud is set up!");

    }

    private void UpdateRain()
    {
        if(!IsRaining)
        {
            if (_rainTimer >= Settings.s.rainTimer)
                StartRaining();
            else
                ContinueToDarkenTheCloud();
        }
        else {
            if (_rainTimer <= 0.0f)
                StopRaining();
            else
                ContinueToRain();
        }
    }

    private void StartRaining()
    {
        IsRaining = true;
        _rain.SetActive(true);
    }

    private void StopRaining()
    {
        IsRaining = false;
        _rain.SetActive(false);
    }

    private void ContinueToRain()
    {
        _rainTimer -= Time.deltaTime;
        _greyCloud.fillAmount = _rainTimer / Settings.s.rainTimer;
    }

    private void ContinueToDarkenTheCloud()
    {
        _rainTimer += Time.deltaTime;
        _greyCloud.fillAmount = _rainTimer / Settings.s.rainTimer;
    }

    public void OnTutorialStart(Transform hand)
    {
        onTutorial = true;
        isDragging = true;
        onCloudGrabbed?.Invoke(transform.position);
    }
    public void OnMouseDown()
    {
        isDragging = true;
        onCloudGrabbed?.Invoke(transform.position);
    }

    public void OnTutorialStop()
    {
        onTutorial = false;
        isDragging = false;
    }

    public void OnMouseUp()
    {
        isDragging = false;
        onCloudDropped?.Invoke(transform.position);
    }

    void Update()
    {
        if (isDragging)
        {
            if (!onTutorial)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                transform.Translate(mousePosition);
            }
        }
        else
        {
            float step = Settings.s.cloudMovementSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, sun.transform.position.With(z:transform.position.z), step);
        }

        if (ShouldRain)
            UpdateRain();
    }

}
