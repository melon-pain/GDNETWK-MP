using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int id;

    private Vector3 targetPosition;
    private Coroutine updatePositionCoroutine = null;

    [SerializeField]
    private GameObject explosion;

    public void Init(int inID)
    {
        id = inID;
    }

    private void Start()
    {
        targetPosition = transform.position;
    }

    public void UpdatePosition(Vector3 target)
    {
        targetPosition = target;

        if (updatePositionCoroutine != null)
        {
            StopCoroutine(updatePositionCoroutine);
            updatePositionCoroutine = null;
        }

        updatePositionCoroutine = StartCoroutine(LerpPosition());
    }

    private IEnumerator LerpPosition()
    {
        float t = 0.0f;

        Vector3 oldPosition = transform.position;

        while (t < Time.fixedDeltaTime && Time.deltaTime < Time.fixedDeltaTime)
        {
            if (Time.deltaTime < Time.fixedDeltaTime)
            {
                break;
            }

            transform.position = Vector3.Lerp(oldPosition, targetPosition, t / Time.fixedDeltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        updatePositionCoroutine = null;
        yield break;
    }

    public void Explode()
    {
        Destroy(Instantiate(explosion, transform.position, transform.rotation), 1.0f);
    }
}
