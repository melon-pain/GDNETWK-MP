using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileSource
{
    Player,
    Enemy
}

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextID = 1;
    private int id;
    public int ID
    {
        get
        {
            return id;
        }
    }

    private int clientID;
    public int ClientID
    {
        get
        {
            return clientID;
        }
    }

    [Header("Projectile")]
    [SerializeField]
    private float damage = 10.0f;
    [SerializeField]
    private float speed = 10.0f;
    [SerializeField]
    private float range = 5.0f;
    [SerializeField]
    private ProjectileSource source;
    public ProjectileSource Source
    {
        get
        {
            return source;
        }
    }

    public void Init(int inClientID)
    {
        clientID = inClientID;
    }

    // Start is called before the first frame update
    private void Start()
    {
        id = nextID;
        nextID++;
        projectiles.Add(id, this);

        ServerSend.SpawnProjectile(this);

        StartCoroutine(DestroyProjectileAtMaxRange());
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime * transform.forward;
        ServerSend.ProjectileTransform(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<IDamageInterface>(out IDamageInterface target))
        {
            target.TakeDamage(damage);
        }

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        ServerSend.ProjectileDestroyed(this);

        projectiles.Remove(id);
        gameObject.SetActive(false);
        Destroy(gameObject, 3.0f);
    }

    private IEnumerator DestroyProjectileAtMaxRange()
    {
        yield return new WaitForSeconds(range / speed);
        DestroyProjectile();
        yield break;
    }
}
