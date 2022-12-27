using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isBoss = false;

    public float spawnInterval;
    public float speed = 5.0f;
    public float boundY = -10.0f;

    public int miniEnemySpawnCount;

    private float nextSpawn;

    private SpawnManager spawnManager;
    private GameObject player;
    private Rigidbody enemyRb;
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");

        if(isBoss)
        {
            spawnManager = FindObjectOfType<SpawnManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;

        enemyRb.AddForce(lookDirection * speed);

        if(isBoss)
        {
            if(Time.time > nextSpawn) //time.time נואכüםמו גנולÿ
            {
                nextSpawn = Time.time + spawnInterval;
                spawnManager.SpawnMiniEnemy(miniEnemySpawnCount);
            }
        }

        if (transform.position.y < boundY)
        {
            Destroy(gameObject);
        }
    }
}
