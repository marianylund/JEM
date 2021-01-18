using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnerOutsideView : MonoBehaviour
{
    [SerializeField]
    private bool _debugSpawner = false;
    private IObjectPool[] _objectPools;
    private float _spaceMoved = 0.0f;
    private float _lastCameraPos = 0.0f;
    private Rect _cameraRect;

    private void Start()
    {
        _objectPools = gameObject.GetComponentsInChildren<IObjectPool>();
        if (_objectPools.Length == 0)
            Debug.LogError("There are no IObjectPools found on " + gameObject.name);
        _cameraRect = MainCameraBehaviour.GetCameraRect();
    }

    public void StartSpawner()
    {
        foreach (IObjectPool pool in _objectPools)
            pool.SpawnObj();
    }

    private void FixedUpdate()
    {
        _spaceMoved += GetPosDelta();
        if (_spaceMoved >= _cameraRect.width/2.0f)
        {
            foreach(IObjectPool pool in _objectPools)
            {
                pool.SpawnObj();
            }
            _cameraRect = MainCameraBehaviour.GetCameraRect();
            _spaceMoved = 0.0f;
        }
    }


    private float GetPosDelta()
    {
        float delta = System.Math.Abs(_lastCameraPos - Camera.main.transform.position.x);
        _lastCameraPos = Camera.main.transform.position.x;
        return delta;
    }

    public static Rect GetSpawnRect(float xScaler = 1.0f, float yScaler = 1.0f, float addXOffset = 0.0f, float addYOffset = 0.0f)
    {
        Rect main = MainCameraBehaviour.GetCameraRect();
        Vector2 spawnBounds = new Vector2(main.width / 2.0f * xScaler, main.height * yScaler);
        float xOffset = main.width / 2.0f * (1.0f - xScaler) / 2.0f + main.width / 2.0f * addXOffset;
        float yOffset = main.height * (1.0f - yScaler) / 2.0f + main.height * addYOffset;
        Vector2 spawnOrigo = new Vector2(main.x + main.width * 1.5f + xOffset, main.y + yOffset);
        return new Rect(spawnOrigo, spawnBounds);
    }

    public static Rect GetSpawnRect(Rect scaler)
    {
        return GetSpawnRect(scaler.width, scaler.height, scaler.x, scaler.y);
    }

    // Debug functions:
    void OnDrawGizmos()
    {
        if (!_debugSpawner)
            return;

        Rect main = MainCameraBehaviour.GetCameraRect();

        // Green
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        DrawRect(main);

        // Waiting areas
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
        DrawRect(new Rect(main.x - main.width / 2.0f, main.y, main.width / 2.0f, main.height));
        Gizmos.color = new Color(0.0f, 0.0f, 1.0f);
        DrawRect(new Rect(main.x + main.width, main.y, main.width / 2.0f, main.height));

        // Despawn area
        Gizmos.color = new Color(1.0f, 1.0f, 0.0f);
        DrawRect(new Rect(main.x - main.width, main.y, main.width / 2.0f, main.height));
        // Spawn area full
        Gizmos.color = new Color(0.0f, 0.4f, 0.4f);
        float val = 1.0f;
        float x_pos = main.x + main.width * 1.5f;
        DrawRect(new Rect(x_pos + main.width / 2.0f * (1.0f - val), main.y + (main.height * (1.0f - val) / 2.0f), main.width / 2.0f * val, main.height * val));
        // Spawn area given by settings
        Gizmos.color = new Color(0.0f, 1.0f, 1.0f);
        DrawRect(GetSpawnRect(Settings.s.spawnBounds));

        Gizmos.color = Settings.s.enemySpawnBoundDebugColor;
        DrawRect(GetSpawnRect(Settings.s.enemySpawnBounds));

        Gizmos.color = Settings.s.seedSpawnBoundDebugColor;
        DrawRect(GetSpawnRect(Settings.s.seedSpawnBounds));
    }

    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }

}
