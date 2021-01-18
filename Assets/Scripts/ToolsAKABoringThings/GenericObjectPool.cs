using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Makes it easier to reuse objects, so the program does not need to create a new one when the same one was destroyed. Will work well for for example enemies.
/// All credit to Jason Weimann, just followed his tutorial about object pooling, see it here: https://www.youtube.com/watch?v=uxm4a0QnQ9E&t=1512s.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class GenericObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField]
    protected T prefab;
    protected int maxObj = 10;

    public static GenericObjectPool<T> Instance { get; private set; }
    protected Queue<T> objects = new Queue<T>();
    protected T[] allObj;

    private void Awake()
    {
        if (GenericObjectPool<T>.Instance == null)
        {
            GenericObjectPool<T>.Instance = this;
        }
        else
        {
            GameObject.Destroy(gameObject);
        }

        Debug.Assert(prefab != null, "Object pool is missing prefab. " + this.GetType().Name);

        ChildAwake();
        AddObjects(maxObj);
        ScenesLoader.onChangingScene += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        _Destroy();
    }

    private void OnDisable()
    {
        _Destroy();
    }

    private void _Destroy()
    {
        if (gameObject != null)
        {
            foreach (T obj in allObj)
            {
                GameObject.Destroy(obj);
            }
            objects = new Queue<T>();
            allObj = new T[0];
            GenericObjectPool<T>.Instance = null;
            ScenesLoader.onChangingScene -= OnSceneUnloaded;
        }
    }

    private void OnSceneUnloaded(Level lvl)
    {
        _Destroy();
        GameObject.Destroy(gameObject);
    }

    protected virtual void ChildAwake() { }

    public T Get()
    {
        if (objects.Count == 0)
            return null;
        //    AddObjects(1);
        return objects.Dequeue();
    }

    public virtual void ReturnToPool(T objectToReturn)
    {
        objectToReturn.gameObject.SetActive(false);
        objects.Enqueue(objectToReturn);
    }

    protected virtual T AddObjects(int count)
    {
        T[] createdChildren = AddChildrenToQueue();
        // Add the rest:
        for (int i = 0; i < count - createdChildren.Length; i++)
        {
            var newObject = GameObject.Instantiate(prefab, gameObject.transform);
            newObject.gameObject.SetActive(false);
            objects.Enqueue(newObject);
        }

        allObj = new T[createdChildren.Length + objects.Count];
        createdChildren.CopyTo(allObj, 0);
        objects.ToArray().CopyTo(allObj, createdChildren.Length);
        return Get();
    }

    protected virtual T[] AddChildrenToQueue()
    {
        T[] createdChildren = gameObject.GetComponentsInChildren<T>();
        foreach (T obj in createdChildren)
        {
            if(!obj.gameObject.activeSelf)
                objects.Enqueue(obj);
        }
        return createdChildren;
    }

    protected virtual Vector3 GetObjPositionForSpawn(T obj)
    {
        return obj.transform.position.RandomXYWithinBounds(RandomSpawnerOutsideView.GetSpawnRect());
    }

    protected virtual void SpawnObject()
    {
        T obj = Get();
        if (obj != null)
        {
            obj.transform.position = GetObjPositionForSpawn(obj);
            obj.gameObject.SetActive(true);
        }
    }

}
