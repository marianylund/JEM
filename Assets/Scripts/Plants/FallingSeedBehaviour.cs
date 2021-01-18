using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSeedBehaviour : MonoBehaviour
{
    public bool isDragging { get; private set; } = false;
    public FallingSeedPool pool;
    private float timer_;
    private float lifeLength;

    public delegate void OnFallingSeed(Vector3 position);
    public static OnFallingSeed onSeedPlanted;
    public static OnFallingSeed onSeedGrabbed;
    public static OnFallingSeed onSeedDropped;

    private bool onTutorial = false;

    private void Start()
    {
        pool = GetComponentInParent<FallingSeedPool>();
    }

    private void OnEnable()
    {
        timer_ = 0.0f;
        isDragging = false;
        lifeLength = Random.Range(Settings.s.seedMinLifeLength, Settings.s.seedMaxLifeLength);
    }

    public void OnTutorialStart(Transform hand)
    {
        timer_ = 0.0f;
        isDragging = true;
        onTutorial = true;
        onSeedGrabbed?.Invoke(transform.position);
    }

    public void OnTutorialStop()
    {
        onTutorial = false;
    }

    public void OnMouseDown()
    {
        isDragging = true;
        timer_ = 0.0f;
        onSeedGrabbed?.Invoke(transform.position);
    }

    public void OnMouseUp()
    {
        isDragging = false;
        onSeedDropped?.Invoke(transform.position);
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
            timer_ += Time.deltaTime;
            float step = 0.6f * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down * 100.0f, step);
            if (timer_ >= lifeLength)
                pool.ReturnToPool(this);
        }
    }

    public void Planted()
    {
        isDragging = false;
        onSeedPlanted?.Invoke(transform.position);
        pool.ReturnToPool(this);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GroundToPlantIn") && MainCameraBehaviour.IsWithinView(transform.position))
        {
            Planted();
        }
    }
}

