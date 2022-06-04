using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private int id;

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

    public void Init(int inID)
    {
        id = inID;
    }

    private void Start()
    {
        health = maxHealth;
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

        while (t < Time.fixedDeltaTime)
        {
            transform.SetPositionAndRotation(
                Vector3.Lerp(oldPosition, targetPosition, t / Time.fixedDeltaTime), 
                Quaternion.Lerp(oldRotation, targetRotation, t / Time.fixedDeltaTime));

            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        updateTransformCoroutine = null;
        yield break;
    }

    private void Death()
    {

    }
}
