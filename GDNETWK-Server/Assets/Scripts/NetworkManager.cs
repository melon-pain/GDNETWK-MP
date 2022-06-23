using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private int maxPlayers = 4;
    [SerializeField]
    private int port = 8080;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyProjectilePrefab;

    [SerializeField]
    private List<Transform> spawnPoints = new();

    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        Server.Start(maxPlayers, port);

        StartCoroutine(IncreaseEnemySpawn());
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer(int clientID)
    {
        return Instantiate(playerPrefab, spawnPoints[clientID - 1].position, Quaternion.identity).GetComponent<Player>();
    }

    public Projectile InstantiateProjectile(ProjectileSource source, Transform origin)
    {
        return Instantiate(source == ProjectileSource.Player ? projectilePrefab : enemyProjectilePrefab, origin.position + origin.forward, origin.rotation).GetComponent<Projectile>();
    }

    public Enemy InstantiateEnemy(Vector3 position)
    {
        return Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
    }

    private IEnumerator IncreaseEnemySpawn()
    {
        // Increase max spawned enemies every 30s
        while (Enemy.SpawnMultiplier < Enemy.maxSpawnMultiplier)
        {
            // Do not increase spawn if no clients are connected
            while (!Server.AreAnyClientsConnected())
            {
                yield return null;
            }

            yield return new WaitForSeconds(30.0f);
            Enemy.IncreaseSpawnMultiplier();
        }

        yield break;
    }

    public void RespawnPlayer(Player player)
    {
        player.transform.position = spawnPoints[player.ID - 1].position;

        player.Respawn();
    }
}
