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

    private bool pickedUp = false;
    public bool PickedUp
    {
        get
        {
            return pickedUp;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        id = nextId;
        nextId++;
        items.Add(id, this);

        ServerSend.SpawnItem(this);
        StartCoroutine(Expire());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            // Only heal when health is lower than max health
            if (player.Health < player.MaxHealth)
            {
                player.Heal(healAmount);
                pickedUp = true;
                DestroyItem();
            }
        }
    }

    private void DestroyItem()
    {
        ServerSend.DestroyItem(this);

        items.Remove(id);
        gameObject.SetActive(false);
        Destroy(gameObject, 3.0f);
    }

    private IEnumerator Expire()
    {
        yield return new WaitForSeconds(5.0f);
        DestroyItem();
    }
}
