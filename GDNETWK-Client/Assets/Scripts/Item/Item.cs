using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    private int id;
    [SerializeField] private GameObject explosion;

    public void Init(int inID)
    {
        id = inID;
    }

    public void Explode()
    {
        Destroy(Instantiate(explosion, transform.position, transform.rotation), 1.0f);
    }
}
