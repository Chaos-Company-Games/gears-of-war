using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public string waveName = "Wave 1";
    public List<EnemySpawnEntry> enemyPool; //Enemies that can spawn this wave

    public float spawnTimeMin = 3f;
    public float spawnTimeMax = 5f;
    public int killsToNextWave = 10;
}

[System.Serializable]
public class EnemySpawnEntry
{
    public GameObject enemyPrefab;
    [Range(0f,1f)] public float spawnWeight = 1f; //Chance to spawn 0%-100%
}
public class WaveManager : MonoBehaviour
{
    public static WaveManager instance {get; private set;}

    [Header("Waves")]
    public List<WaveData> waves;

    [Header("Spawn")]
    public Transform spawnPoint; //Enemy spawn on right of screen

    private int currentWaveIndex = 0;
    private int killsThisWave = 0;
    private bool spawning = false;
    private bool gameOver = false;
    void Awake()
    {
        //Singelton stuff
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        StartWave(0);
    }
     void StartWave(int index)
    {
        if (index >= waves.Count)
        {
            Debug.Log ("we outta waves");
            return;
        }

        currentWaveIndex = index;
        killsThisWave = 0;
        spawning = true;

        Debug.Log($"Wave {index + 1} started!: {waves[index].waveName}");
        StartCoroutine(SpawnLoop(waves[index]));
    }

    IEnumerator SpawnLoop(WaveData wave)
    {
        while (spawning && !gameOver)
        {
            SpawnEnemy(wave);
            float interval = Random.Range(wave.spawnTimeMin, wave.spawnTimeMax);
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnEnemy(WaveData wave)
    {
        GameObject prefab = PickWeightedEnemy(wave.enemyPool);
        if (prefab == null) return;

        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
    }

    GameObject PickWeightedEnemy(List<EnemySpawnEntry> pool)
    {
        //Lets epicly pick our enemy based on his chance
        float totalWeight = 0f;
        foreach (var e in pool) totalWeight += e.spawnWeight;
        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var e in pool)
        {
            cumulative += e.spawnWeight;
            if (roll <= cumulative)
            {
                return e.enemyPrefab;
            }
        }

        return pool[0].enemyPrefab;
    }
  
    public void OnEnemyKilled()
    {
        killsThisWave++;

        WaveData current = waves[currentWaveIndex];
        if (killsThisWave >= current.killsToNextWave)
        {
            spawning = false;
            StopAllCoroutines();

            int next = currentWaveIndex + 1;
            if (next < waves.Count)
            {
                Debug.Log($"Wave {currentWaveIndex + 1} complete! (Frick yeah) Starting next wave!");
                StartWave(next);
            }
            else
            {
                Debug.Log("All waevs done! Add more content lol");
            }
        }
    }

    public void OnGameOver()
    {
        gameOver = true;
        spawning = false;
        StopAllCoroutines();
    }
}
