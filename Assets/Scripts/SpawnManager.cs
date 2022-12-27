using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    public GameObject[] powerUpPrefabs;
    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefabs;

    private PlayerController playerController;

    private float spawnRangePos = 9;
    private float spawnPlayer = 2;

    public int bossRound;
    public int enemyCount;
    public int waveNumber = 1;
    public int miniEnemysToSpawn = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        SpawnEnemyWave(waveNumber);
        SpawnPowerUp();
    }

    void SpawnEnemyWave(int enemiesToSpawn) // (int enemiesToSpawn, ...) параметры, их может быть несколько,
    {
        // цикл повторяет действие чего либо определное количество раз
        // (с чего начать, условие для стоп цикла, как мы получим условие для стоп цикла)
        if (playerController.gameOver != true)
        {
            for (int i = 0; i < enemiesToSpawn; i++)// i++ ; i = i +1; i +=1; это всё одинаковые значения;
            {
                int prefabIndex = Random.Range(0, enemyPrefab.Length);
                Instantiate(enemyPrefab[prefabIndex], GenerateSpawnPosition(), enemyPrefab[prefabIndex].transform.rotation);
                Debug.Log("Wave"+ waveNumber);
            }
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(Random.Range(-spawnRangePos, -spawnPlayer), Random.Range(spawnRangePos, spawnPlayer));
        float spawnPosZ = Random.Range(Random.Range(-spawnRangePos, -spawnPlayer), Random.Range(spawnRangePos, spawnPlayer));
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }

    void SpawnBossWave(int CurrentRound)
    {
        miniEnemysToSpawn++;
        if (bossRound != 0)
        {
            miniEnemysToSpawn = CurrentRound / bossRound;
        }
        else
        {
            miniEnemysToSpawn = 1;
        }

        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn;
    }

    public void SpawnMiniEnemy(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length);
            Instantiate(miniEnemyPrefabs[randomMini], GenerateSpawnPosition(), miniEnemyPrefabs[randomMini].transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;//FindObjectOfType находит обьекты на сцене по названию типа
        if (enemyCount == 0 && playerController.gameOver != true)
        {
            waveNumber++;
            if (waveNumber % bossRound == 0)
            {
                SpawnBossWave(waveNumber);
            }
            else
            {
                SpawnEnemyWave(waveNumber);
            }
            SpawnPowerUp();
        }
    }

    private void SpawnPowerUp()
    {
        int randomPowerUp = Random.Range(0, powerUpPrefabs.Length);
        if(playerController.gameOver != true)
        {
            Instantiate(powerUpPrefabs[randomPowerUp], GenerateSpawnPosition(), powerUpPrefabs[randomPowerUp].transform.rotation);
        }
    }
}
