using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : GenericObjectPool<EnemyBehaviour>, IObjectPool
{
    private float timer_;

    protected override void ChildAwake()
    {
        maxObj = Settings.s.maxEnemiesInView;
        LevelProgression.onEndReached += OnEndReached;
    }

    private void OnEndReached(float m)
    {
        TryToReturnObj();
    }

    private void OnDisable()
    {
        LevelProgression.onEndReached -= OnEndReached;
    }

    private void TryToReturnObj()
    {
        foreach (EnemyBehaviour enemy in allObj)
        {
            if (enemy && enemy.gameObject.activeSelf && !MainCameraBehaviour.IsWithinView(enemy.transform.position))
            {
                ReturnToPool(enemy);
            }
        }
    }

    protected override Vector3 GetObjPositionForSpawn(EnemyBehaviour obj)
    {
        return obj.transform.position.RandomXYWithinBounds(RandomSpawnerOutsideView.GetSpawnRect(Settings.s.enemySpawnBounds));
    }

    public void SpawnObj()
    {
        //Debug.Log("Enemy pool is trying to spawn objects");
        if (objects.Count <= 0)
        {
            TryToReturnObj();
        }

        for (int i = 0; i < Settings.s.maxEnemiesInView / 2.0f; i++)
        {
            if (objects.Count <= 0)
                return;
            float toSpawnRandom = Random.value;
            bool toSpawnOrNotToSpawn = toSpawnRandom <= Settings.s.enemySpawnChance;
            //Debug.Log("Enemies: " + " currentChance = " + toSpawnRandom + " needed <= " + Settings.s.enemySpawnChance + " so to spawn: " + toSpawnOrNotToSpawn);
            if (toSpawnOrNotToSpawn)
                SpawnObject();
        }
    }
}
