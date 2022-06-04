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
        Application.targetFrameRate = 60;

        Server.Start(maxPlayers, port);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
    }

    public Projectile InstantiateProjectile(ProjectileSource source, Transform origin)
    {
        return Instantiate(source == ProjectileSource.Player ? projectilePrefab : enemyProjectilePrefab, origin.position + origin.forward, origin.rotation).GetComponent<Projectile>();
    }

    public Enemy InstantiateEnemy(Vector3 position)
    {
        return Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
    }
}
