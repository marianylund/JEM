using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCameraBehaviour : MonoBehaviour
{
    private SelfDrivingBeeBehaviour _beeToFollow;
    float offset;
    public static bool ShouldCameraFollowTheBee = true;
    private void OnEnable()
    {
        ShouldCameraFollowTheBee = true;
    }
    private void Start()
    {
        _beeToFollow = GameObject.FindGameObjectWithTag("MainBee")?.GetComponent<SelfDrivingBeeBehaviour>();
        if (_beeToFollow == null)
            Debug.LogError("No bee to follow for camera");

        offset = Camera.main.aspect * Camera.main.orthographicSize * Settings.s.cameraMovementOffset;
        ScenesLoader.onChangingScene += StopFollowingTheBee;
    }

    private void OnDestroy()
    {
        ScenesLoader.onChangingScene -= StopFollowingTheBee;
    }

    private void StopFollowingTheBee(Level lvl)
    {
        ShouldCameraFollowTheBee = false;
    }

    void FixedUpdate()
    {
        if (ShouldCameraFollowTheBee)
            this.transform.position = this.transform.position.With(x: Mathf.Lerp(this.transform.position.x, _beeToFollow.transform.position.x + offset, Time.deltaTime * Settings.s.beeFlyingSpeed));
    }
    public static bool IsWithinView(Vector3 position)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

    public static bool IsWithinViewX(Vector3 position)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        return screenPoint.x > 0 && screenPoint.x < 1;
    }

    public static Rect GetCameraRect()
    {
        Vector2 screenBounds = (Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight)) - Camera.main.transform.position) * 2.0f;
        Vector2 screenOrigo = Camera.main.ScreenToWorldPoint(Vector2.zero);
        return new Rect(screenOrigo, screenBounds);
    }
}
