using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Level
{
    Past,
    Now,
    Future,
    Menu,
    Ending,
    Loading
}

public abstract class LevelProgression : MonoBehaviour
{
    public bool FinishedLevel { get; internal set; } = false;
    public float DistanceTravelled { get; internal set; } = 0.0f;
    public float TimePlayed { get; internal set; } = 0.0f;
    public float TargetDistance { get; internal set; }
    /// <summary>
    /// Milestones between 0.0 and 1.0 of the reached way
    /// 1.0 is the end of the level
    /// </summary>
    public List<float> Milestones { get; internal set; }
    public List<float> ReachedMilestones { get; internal set; }
    private float _lastCameraPos = 0.0f;
    public float Progress { get => DistanceTravelled / TargetDistance; }
    public delegate void OnLevelProgression(float milestone);
    public static OnLevelProgression onMilestoneReached;
    public static OnLevelProgression onEndReached;
    public static OnLevelProgression onLevelFinished;
    public static OnLevelProgression onLevelFailed;

    void Start()
    {
        ReachedMilestones = new List<float>();
        Milestones = new List<float>();
        Debug.Log("I am using settings: " + Settings.s.name);
        SetUpTargetDistanceAndMilestones();
        Milestones.Sort();
        BeehiveBehaviour.onLastBeeDie += _LevelFailed;
    }

    private void OnDestroy()
    {
        BeehiveBehaviour.onLastBeeDie -= _LevelFailed;
        _Destroy();
    }

    internal abstract void _Destroy();


    private void _LevelFailed(Vector3 pos)
    {
        ScenesLoader.i_.FailLevel();
        onLevelFailed?.Invoke(-1.0f);
    }

    internal abstract void SetUpTargetDistanceAndMilestones();

    private void Update()
    {
        if (FinishedLevel)
            return;

        DistanceTravelled += GetPosDelta();
        TimePlayed += Time.deltaTime;

        if (Progress >= 1.0f)
        {
            onEndReached?.Invoke(1.0f);
            FinishedLevel = true;
            return;
        }

        if(Milestones.Count != 0 && Progress >= Milestones[0])
        {
            onMilestoneReached?.Invoke(Milestones[0]);
            ReachedMilestones.Add(Milestones[0]);
            Milestones.RemoveAt(0);
        }
    }

    private float GetPosDelta()
    {
        float delta = System.Math.Abs(_lastCameraPos - Camera.main.transform.position.x);
        _lastCameraPos = Camera.main.transform.position.x;
        return delta;
    }
}
