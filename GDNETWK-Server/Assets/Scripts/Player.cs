using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageInterface
{
    private int id;
    public int ID
    {
        get
        {
            return id;
        }
    }
    public string username;
    [SerializeField]
    private CharacterController controller;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5.0f;
    private bool isMoving = false;
    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
    }

    [Header("Attributes")]
    [SerializeField]
    private float maxHealth = 100.0f;
    public float MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }

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
            ServerSend.PlayerHealth(this);
        }
    }
    [SerializeField]
    private float fireRate = 0.2f;
    private bool isShooting = false;

    [SerializeField]
    private GameObject pod;
    public GameObject Pod
    {
        get
        {
            return pod;
        }
    }

    private bool[] inputs;

    public void Init(int inID, string inUsername)
    {
        id = inID;
        username = inUsername;

        inputs = new bool[4];

        health = maxHealth;
    }

    private void Start()
    {
        StartCoroutine(Shoot());
    }

    private Vector2 GatherMovementInput()
    {
        Vector2 input = Vector2.zero;

        if (inputs[0])
        {
            input.y += 1;
        }
        if (inputs[1])
        {
            input.y -= 1;
        }
        if (inputs[2])
        {
            input.x -= 1;
        }
        if (inputs[3])
        {
            input.x += 1;
        }

        return input;
    }

    private void FixedUpdate()
    {
        Vector2 input = GatherMovementInput().normalized;
        Vector3 direction = new Vector3(input.x, 0.0f, input.y);
        isMoving = direction.sqrMagnitude > 0.0f;

        if (isMoving)
        {
            //Move player
            controller.Move(((moveSpeed * direction) + Physics.gravity) * Time.fixedDeltaTime);
            //Face player to direction
            transform.forward = direction;
        }

        ServerSend.PlayerTransform(this);
    }

    public void MovementInput(bool[] inInputs)
    {
        inputs = inInputs;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        Debug.Log("Take Damage");
    }

    public void ProjectileAbility(bool inIsShooting)
    {
        isShooting = inIsShooting;
    }

    private IEnumerator Shoot()
    {
        var wait = new WaitForSeconds(fireRate);

        while (true)
        {
            if (isShooting)
            {
                Projectile projectile = NetworkManager.Instance.InstantiateProjectile(ProjectileSource.Player, pod.transform);
                projectile.Init(id);
                yield return wait;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
            
        }
    }

    public void RotatePod(Quaternion rotation)
    {
        pod.transform.rotation = rotation;
        ServerSend.PodTransform(this);
    }
}