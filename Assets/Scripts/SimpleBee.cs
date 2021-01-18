using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBee : MonoBehaviour
{
    private SpriteRenderer _dead;
    private SpriteRenderer _alive;
    private Coroutine _dying;
    public bool IsAlive { get => _alive.enabled; }
    public bool IsMainBee { get; private set; } = false;
    void Start()
    {
        IsMainBee = gameObject.name.ToLower().Contains("mainbee");
        float scale = IsMainBee ? 1.0f : Random.Range(0.65f, 0.8f);
        transform.localScale = new Vector3(scale, scale, scale);
        foreach (SpriteRenderer child in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (child.name.ToLower().Contains("alive"))
            {
                _alive = child;
                _alive.enabled = true;
                _alive.sortingOrder = IsMainBee ? 1 : - 1;
            }else if (child.name.ToLower().Contains("dead"))
            {
                _dead = child;
                _dead.enabled = false;
                _dead.sortingOrder = IsMainBee ? 1 : -1;
            }
        }
        CheckSetUp();
    }

    private void CheckSetUp()
    {
        Debug.Assert(_dead != null, "No dead sprite bee found");
        Debug.Assert(_alive != null, "No _alive sprite bee found");
    }

    public void RIP()
    {
        Debug.Assert(IsAlive, "The bee is not alive! Ghosts can't die" + gameObject.name);
        _dying = StartCoroutine("FallDown");
    }

    IEnumerator FallDown()
    {
        _alive.enabled = false;
        _dead.enabled = true;
        transform.parent = null; // Unconnect from the beehive
        float timeToFall = Settings.s.beeFlyingSpeed;
        while (MainCameraBehaviour.IsWithinView(transform.position))
        {
            float step = timeToFall * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down * 10.0f + Vector3.right * 5.0f, step);

            // Settings.s.beeFlyingSpeed = Mathf.Lerp(0.05f, originalBeeSpeed, TimePlayed / timeToSpeedUp);
            yield return new WaitForEndOfFrame();
        }
        _dying = null;
    }

    private void OnDestroy()
    {
        if (_dying != null)
            StopCoroutine(_dying);
    }

}
