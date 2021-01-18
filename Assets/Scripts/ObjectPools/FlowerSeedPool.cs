using UnityEngine;

public class FlowerSeedPool : GenericObjectPool<SeedFlowerBehaviour>
{
    protected override void ChildAwake()
    {
        maxObj = Settings.s.maxFallingSeedsInView * 4;
    }

    private void Start()
    {
        if (allObj.Length == 0)
            Debug.LogError("No spawned seed flowers found!");

        FallingSeedBehaviour.onSeedPlanted += PlantSeed;
    }

    private void OnDestroy()
    {
        FallingSeedBehaviour.onSeedPlanted -= PlantSeed;
    }

    private void PlantSeed(Vector3 pos)
    {
        TryToReturnFlowers();

        SeedFlowerBehaviour obj = Get();
        if(obj == null)
        {
            obj = AddObjects(2);
            Debug.LogWarning("Not enough flower seeds in the pool, had to add two!");
        }

        obj.transform.position = pos;
        obj.gameObject.SetActive(true);
    }

    private void TryToReturnFlowers()
    {
        foreach (SeedFlowerBehaviour flower in allObj)
        {
            if (flower == null)
                Debug.LogError("this flower does not exist");
            if (flower.gameObject.activeSelf && !MainCameraBehaviour.IsWithinView(flower.transform.position))
            {
                flower.ReturnToPool();
                ReturnToPool(flower);
            }
        }
    }
}
