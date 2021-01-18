using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfDrivingBeeBehaviour : MonoBehaviour
{
    [SerializeField]
    private Vector3 directionBeeIsLooking = -Vector3.up;

    private BeehiveBehaviour beehive;
    public delegate void OnBeeEvent(int energy, Vector3 position);
    public static OnBeeEvent onGettingEnergy;
    public static OnBeeEvent onTakeDamage;
    public static OnBeeEvent onReachingHome;

    [SerializeField]
    private Transform originalTarget;
    [SerializeField]
    private Transform target;

    private Collider2D _cl;

    void Start()
    {
        target = originalTarget;
        _cl = GetComponent<Collider2D>();
        beehive = GetComponent<BeehiveBehaviour>();
        Debug.Assert(beehive != null, "Could not find beehive on the " + gameObject.name);
        ScenesLoader.onChangingScene += Destroy;
    }

    private void Destroy(Level lvl)
    {
        ScenesLoader.onChangingScene -= Destroy;
        GameObject.Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ScenesLoader.onChangingScene -= Destroy;
    }

    private void FixedUpdate()
    {
        Look();
        Fly();
    }

    public void gotFlower(int energy)
    {
        target = originalTarget;
        onGettingEnergy(energy, transform.position);
    }

    // Cone? https://forum.unity.com/threads/raycasting-a-cone-instead-of-single-ray.39426/
    private void Look()
    {
        // Cast a ray straight down.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionBeeIsLooking * 10f);
        Debug.DrawRay(transform.position, directionBeeIsLooking * 10f, Color.green);
        // If it hits something...
        if (hit.collider != null)
        {
            SeedFlowerBehaviour flower = hit.collider.transform.GetComponentInParent<SeedFlowerBehaviour>();
            if (flower != null && flower.hasGrown && flower.CanPickUp)
            {
                if (hit.collider.IsTouching(_cl))
                {
                    gotFlower(flower.amountOfEnergyFlowerGives);
                    flower.PickUp();
                }
                else
                    target = flower.transform;
                return;
            }
        }
    }

    private void Fly()
    {

        // Move our position a step closer to the target.
        float step = Settings.s.beeFlyingSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target.position.With(z:transform.position.z), step);

    }

    private void TakeDamage(int damage)
    {
        onTakeDamage?.Invoke(damage, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SeedFlowerBehaviour flower = collision.gameObject.GetComponentInParent<SeedFlowerBehaviour>();
        if (flower != null && flower.hasGrown && flower.CanPickUp)
        {
            gotFlower(flower.amountOfEnergyFlowerGives);
            flower.PickUp();
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            EnemyType enemyType = collision?.GetComponentInParent<EnemyBehaviour>()?.CurrentEnemy ?? EnemyType.DragonFly;
           // Debug.Log("This enemy is: " + enemyType.ToString());
            int damage = 1;
            if(enemyType == EnemyType.Fire || enemyType == EnemyType.Hurricane)
            {
                damage = Mathf.FloorToInt(beehive.NumberOfBees / 3);
                damage = damage == 0 ? 1 : damage;
                //Debug.Log("I am fire or hurricane, so I kill " + damage + " of bees, since there was only " + beehive.NumberOfBees + " bees");
            }
            TakeDamage(damage);
        }

        if (collision.CompareTag("Finish"))
        {
            Debug.Log("SelfDrivingBee has reached the finish");
            onReachingHome?.Invoke(GetComponent<BeehiveBehaviour>()?.NumberOfBees ?? 0, transform.position);
        }
    }
}
