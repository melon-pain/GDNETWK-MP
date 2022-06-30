using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int id;
    public int ID => id;
    [SerializeField]
    private string username;
    [SerializeField]
    private Text nameTag;

    [Header("Animation")]
    [SerializeField]
    private Animator animator;
    public Animator Animator
    {
        get
        {
            return animator;
        }
    }
    [SerializeField]
    private GameObject model;

    [Header("Attributes")]
    [SerializeField]
    private float maxHealth = 100.0f;
    [SerializeField]
    private float health;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private bool isDead = false;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = Mathf.Clamp(value, 0.0f, maxHealth);
            healthBar.fillAmount = health / maxHealth;

            if (value <= 0.0f)
            {
                Death();
            }
        }
    }

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Coroutine updateTransformCoroutine = null;

    [SerializeField]
    private GameObject pod;
    private Quaternion targetPodRotation;
    private Coroutine updatePodRotationCoroutine = null;


    public void Init(in int inID, in string inUsername)
    {
        id = inID;
        username = inUsername;
        nameTag.text = inUsername;

        health = maxHealth;

        isDead = false;
    }

    private void Start()
    {
        targetPosition = transform.position;
        targetPodRotation = pod.transform.rotation;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        //if (id == Client.Instance.ID)
        //{
        //    Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition) * 2.0f - Vector3.one;
        //    float angle = (Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg) - 90.0f;
        //    Vector3 direction = new Vector3(0.0f, -angle, 0.0f);

        //    pod.transform.rotation = Quaternion.Euler(direction);
        //}
    }

    private void FixedUpdate()
    {
        if (isDead || !Application.isFocused)
        {
            return;
        }

        SendMovementInputServer();

        ClientSend.PlayerProjectile(Input.GetMouseButton(0));

        ClientSend.PodRotation(Camera.main.ScreenToViewportPoint(Input.mousePosition) * 2.0f - Vector3.one);
    }

    private void SendMovementInputServer()
    {
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
        };

        ClientSend.PlayerMovement(inputs);
    }

    public void Death()
    {
        isDead = true;
        model.SetActive(false);
        pod.SetActive(false);
    }

    public void Respawn()
    {
        isDead = false;
        model.SetActive(true);
        pod.SetActive(true);
    }
    
    public void UpdateTransform(Vector3 targetPos, Quaternion targetRot)
    {
        targetPosition = targetPos;
        targetRotation = targetRot;

        if (updateTransformCoroutine != null)
        {
            StopCoroutine(updateTransformCoroutine);
            updateTransformCoroutine = null;
        }

        updateTransformCoroutine = StartCoroutine(LerpTransform());
    }

    private IEnumerator LerpTransform()
    {
        float t = 0.0f;

        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        while (t < Time.fixedDeltaTime && Time.deltaTime < Time.fixedDeltaTime)
        {
            if (Time.deltaTime < Time.fixedDeltaTime)
            {
                break;
            }

            transform.SetPositionAndRotation(Vector3.Lerp(oldPosition, targetPosition, t / Time.fixedDeltaTime), Quaternion.Lerp(oldRotation, targetRotation, t / Time.fixedDeltaTime));
            t += Time.deltaTime;
            yield return null;
        }

        transform.SetPositionAndRotation(targetPosition, targetRotation);

        updateTransformCoroutine = null;
        yield break;
    }

    public void UpdatePodRotation(Quaternion target)
    {
        targetPodRotation = target;

        if (updatePodRotationCoroutine != null)
        {
            StopCoroutine(updatePodRotationCoroutine);
            updatePodRotationCoroutine = null;
        }

        updatePodRotationCoroutine = StartCoroutine(LerpPodRotation());
    }

    private IEnumerator LerpPodRotation()
    {
        float t = 0.0f;

        Quaternion oldPodRotation = pod.transform.rotation;

        while (t < Time.fixedDeltaTime && Time.deltaTime < Time.fixedDeltaTime)
        {
            if (Time.deltaTime < Time.fixedDeltaTime)
            {
                break;
            }
            pod.transform.rotation = Quaternion.Lerp(oldPodRotation, targetPodRotation, t / Time.fixedDeltaTime);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        pod.transform.rotation = targetPodRotation;

        updatePodRotationCoroutine = null;
        yield break;
    }
}
