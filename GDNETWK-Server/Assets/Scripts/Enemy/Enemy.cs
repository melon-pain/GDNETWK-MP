using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageInterface
{
    public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();
    private static int nextID = 1;
    private int id;
    [SerializeField] private GameObject itemPrefab;
    public int ID
    {
        get
        {
            return id;
        }
    }

    private static float spawnMultiplier = 1.0f;
    public static float SpawnMultiplier => spawnMultiplier;
    public const float maxSpawnMultiplier = 3.0f;

    private static int maxEnemiesSpawned = 12;
    public static int MaxEnemiesSpawned => (int)(maxEnemiesSpawned * spawnMultiplier);

    private Player targetPlayer = null;

    [Header("Movement")]
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private float moveSpeed = 5.0f;

    [Header("Attributes")]
    [SerializeField]
    private float maxHealth = 100.0f;
    [SerializeField]
    private float health;
    public float Health
    {
        get
        {
            return health;
        }
        private set
        {
            health = Mathf.Clamp(value, 0.0f, maxHealth);
            ServerSend.EnemyHealth(this);

            if (value <= 0.0f)
            {
                Death();
            }
        }
    }
    [SerializeField]
    private float fireRate = 0.5f;
    [SerializeField]
    private float range = 10.0f;
    [SerializeField]
    private GameObject shootOrigin;

    [SerializeField]
    private LayerMask layerMask;

    private void Start()
    {
        id = nextID;
        nextID++;
        enemies.Add(id, this);

        health = maxHealth;

        ServerSend.SpawnEnemy(this);

        StartCoroutine(LookForPlayer());
        StartCoroutine(Shoot());
    }

    private void FixedUpdate()
    {
        if (!targetPlayer)
            return;

        if (!targetPlayer.IsDead)
        {
            Vector3 dir = (targetPlayer.transform.position - transform.position).normalized;
            transform.LookAt(targetPlayer.transform);
            transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);

            controller.Move(((dir * moveSpeed) + (Physics.gravity)) * Time.fixedDeltaTime);
        }
        else
        {
            LookForNewPlayer();
        }

        ServerSend.EnemyTransform(this);
    }

    private IEnumerator LookForPlayer()
    {
        while (Health > 0.0f)
        {
            LookForNewPlayer();

            yield return new WaitForSeconds(3.0f);
        }

        yield break;
    }

    private bool LookForNewPlayer()
    {
        float dist = float.PositiveInfinity;

        foreach (var client in Server.clients.Values)
        {
            if (client.player == null)
            {
                continue;
            }
            if (!client.player.IsDead)
            {
                float clientDist = Vector3.Distance(transform.position, client.player.transform.position);
                if (clientDist < dist)
                {
                    dist = clientDist;
                    targetPlayer = client.player;
                }
            }
        }

        //Target not found
        if (dist == float.PositiveInfinity)
        {
            targetPlayer = null;
            return false;
        }

        return true;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
    }

    private void Death()
    {
        ServerSend.EnemyDestroyed(this);

        GameObject instance = Instantiate(itemPrefab, transform.position, transform.rotation);

        enemies.Remove(id);
        gameObject.SetActive(false);
        Destroy(gameObject, 3.0f);
    }

    private IEnumerator Shoot()
    {
        var wait = new WaitForSeconds(fireRate);

        while (Health > 0.0f)
        {
            yield return wait;

            while(!targetPlayer)
            {
                yield return null;
            }

            if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= range && !targetPlayer.IsDead)
            {
                NetworkManager.Instance.InstantiateProjectile(ProjectileSource.Enemy, shootOrigin.transform);
            }
        }
    }

    public static void IncreaseSpawnMultiplier()
    {
        spawnMultiplier = Mathf.Clamp(spawnMultiplier + 0.2f, 0, maxSpawnMultiplier);
    }
}
