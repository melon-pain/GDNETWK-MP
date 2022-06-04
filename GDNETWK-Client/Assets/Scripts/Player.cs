using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int id;
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

    [Header("Attributes")]
    [SerializeField]
    private float maxHealth = 100.0f;
    [SerializeField]
    private float health;
    [SerializeField]
    private Image healthBar;

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
    }

    private void Start()
    {
        targetPosition = transform.position;
        targetPodRotation = pod.transform.rotation;
    }

    private void Update()
    {
        if (id == Client.Instance.ID)
        {
            Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition) * 2.0f - Vector3.one;
            float angle = (Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg) - 90.0f;
            Vector3 direction = new Vector3(0.0f, -angle, 0.0f);

            pod.transform.rotation = Quaternion.Euler(direction);
        }
    }

    private void FixedUpdate()
    {
        SendMovementInputServer();

        ClientSend.PlayerProjectile(Input.GetMouseButton(0));

        ClientSend.PodRotation(pod.transform.rotation);
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

    private void Death()
    {

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
            transform.SetPositionAndRotation(Vector3.Lerp(oldPosition, targetPosition, t / Time.fixedDeltaTime), Quaternion.Lerp(oldRotation, targetRotation, t / Time.fixedDeltaTime));
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
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

        while (t < Time.fixedDeltaTime)
        {
            pod.transform.rotation = Quaternion.Lerp(oldPodRotation, targetPodRotation, t / Time.fixedDeltaTime);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        updatePodRotationCoroutine = null;
        yield break;
    }
}
