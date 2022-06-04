using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField, Min(0.0f)]
    private float delay = 0.0f;
    [SerializeField, Min(0.0f)]
    private float minSpawnTime = 2.0f;
    [SerializeField, Min(0.0f)]
    private float maxSpawnTime = 10.0f;

    private float spawnRate = 1.0f;
    private float maxSpawnRate = 3.0f;

    private void Start()
    {
        StartCoroutine(ActivationDelay());
    }

    private IEnumerator ActivationDelay()
    {
        yield return new WaitForSeconds(delay);

        StartCoroutine(SpawnEnemy());
        StartCoroutine(IncreaseSpawnRate());

        yield break;
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (Server.AreAnyClientsConnected())
            {
                if (Enemy.enemies.Count < Enemy.MaxEnemiesSpawned)
                {
                    yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime) / spawnRate);
                    NetworkManager.Instance.InstantiateEnemy(transform.position);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator IncreaseSpawnRate()
    {
        while (spawnRate < maxSpawnRate)
        {
            yield return new WaitForSeconds(15.0f);
            spawnRate += 0.1f;
        }

        yield break;
    }
}
