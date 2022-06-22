using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public static Dictionary<int, Item> items = new Dictionary<int, Item>();
    private const int healAmount = 10;
    private static int nextId = 1;
    private int id;
    public int ID
    {
        get
        {
            return id;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        id = nextId;
        nextId++;
        items.Add(id, this);

        ServerSend.SpawnItem(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Heal(healAmount);
            DestroyItem();
        }
    }

    private void DestroyItem()
    {
        ServerSend.DestroyItem(this);

        items.Remove(id);
        gameObject.SetActive(false);
        Destroy(gameObject, 3.0f);
    }
}
