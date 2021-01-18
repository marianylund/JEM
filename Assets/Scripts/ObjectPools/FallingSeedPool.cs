using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSeedPool : GenericObjectPool<FallingSeedBehaviour>, IObjectPool
{
    protected override void ChildAwake()
    {
        maxObj = Settings.s.maxFallingSeedsInView;
    }
    protected override Vector3 GetObjPositionForSpawn(FallingSeedBehaviour obj)
    {
        return obj.transform.position.RandomXYWithinBounds(RandomSpawnerOutsideView.GetSpawnRect(Settings.s.seedSpawnBounds));
    }
    public void SpawnObj()
    {
        for(int i = 0; i <= Settings.s.maxFallingSeedsInView / 2.0f; i++)
        {
            if (objects.Count <= 0)
                AddObjects(2);
            if (Random.value <= Settings.s.fallingSeedSpawnChance)
                SpawnObject();
        }
    }

}
