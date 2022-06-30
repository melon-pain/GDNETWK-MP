using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public static Dictionary<int, Player> players = new Dictionary<int, Player>();
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();
    public static Dictionary<int, Item> items = new Dictionary<int, Item>();

    [SerializeField]
    private GameObject localPlayerPrefab;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject cameraPrefab;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyProjectilePrefab;
    [SerializeField]
    private GameObject itemPrefab;

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
    }

    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject instance;
        if (id == Client.Instance.ID)
        {
            instance = Instantiate(localPlayerPrefab, position, rotation);
            Instantiate(cameraPrefab).GetComponentInChildren<CinemachineVirtualCamera>().Follow = instance.transform;
        }
        else
        {
            instance = Instantiate(playerPrefab, position, rotation);
        }

        Player player = instance.GetComponent<Player>();
        player.Init(id, username);

        players.Add(id, player);
    }

    public void SpawnProjectile(int id, int source, Vector3 position, Quaternion rotation)
    {
        GameObject instance = Instantiate(source == 0 ? projectilePrefab : enemyProjectilePrefab, position, rotation);
        Projectile projectile = instance.GetComponent<Projectile>();
        projectile.Init(id);
        projectiles.Add(id, projectile);
    }

    public void SpawnEnemy(int id, Vector3 position, Quaternion rotation)
    {
        GameObject instance = Instantiate(enemyPrefab, position, rotation);
        Enemy enemy = instance.GetComponent<Enemy>();
        enemy.Init(id);
        enemies.Add(id, enemy);
    }

    public void SpawnItem(int id, Vector3 position)
    {
        GameObject instance = Instantiate(itemPrefab, position, Quaternion.identity);
        Item item = instance.GetComponent<Item>();
        item.Init(id);
        items.Add(id, item);
    }
}
