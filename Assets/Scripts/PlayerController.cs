using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject powerUpIndicator;
    public PowerUpType currentPowerUp = PowerUpType.None;

    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;

    private float xMod = 2;
    private float floorY;
    private float powerUpStrength = 15.0f;
    private float powerUpDuration = 6.0f;
    public float speed = 5.0f;
    public float strengthMode = 50.0f;
    public float smashSpeed;
    public float hangTime;
    public float explosionForce;
    public float explosionRadius;

    private GameObject focalPoint;
    private Rigidbody playerRb;
    private SpawnManager spawnManager;

    public bool hasPowerUp;
    public bool gameOver = false;
    public bool smashing = false;
    

    private Vector3 offset = new Vector3(0, -0.65f, 0);

    // Start is called before the first frame update
    void Start()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        
    }

    // Update is called once per frame
    void Update()
    {
        
        ////����������� �������� ���������� � �������� ������
        powerUpIndicator.transform.position = transform.position + offset;

        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * verticalInput);

        if (transform.position.y < -10)
        {
            gameOver = true;
            Debug.Log("Game Over!Wave" + spawnManager.waveNumber);
        }

        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets();
        }

        if(currentPowerUp == PowerUpType.Jumper && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }
    }

    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehavour>().Fire(enemy.transform);
        }
    }

    //����� ��� �������������� � ����������
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            Destroy(other.gameObject);
            powerUpIndicator.gameObject.SetActive(true);//SetActive ������� ������� �������� ��� ��������� ������ �� �����
            Debug.Log("You have PowerUp!");
        }
        if (powerupCountdown != null)
        {
            StopCoroutine(powerupCountdown);
        }
        powerupCountdown = StartCoroutine(PowerCountdownRoutine()); ////��������� �������� ������
    }
    // IEnumerator ����� ��������� ������ ����������� ���� ��� ������ ����� Update()
    IEnumerator PowerCountdownRoutine()
    {
        //WaitForSeconds ������� ��������� ��������� ������, ����������� � ������� Coroutines
        yield return new WaitForSeconds(powerUpDuration); //yield  ��������� ��� ��������� ������� WaitForSeconds
        currentPowerUp = PowerUpType.None;
        hasPowerUp = false;
        powerUpIndicator.gameObject.SetActive(false);
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();

        //��������� ������� y ����� ������
        floorY = transform.position.y;

        // ���������� ���������� �������, �� ������� �� ����� �����������
        float jumpTime = Time.time + hangTime;

        // ���������� ������ �����, �������� ��� ���� ��� �������� x.
        while (Time.time < jumpTime)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        //���������� ������ ���� 
        while(transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * xMod);
            yield return null;
        }

        //�������� �� ���� ������
        for (int i = 0; i < enemies.Length; i++)
        {
            //��������� ���� ������, ������� ������� �� ����� �������
            if(enemies[i] != null)
            enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
        }
        
            //// �� ������ �� ���������, ������� ������������� ���������� �������� � false
        smashing = false;
    }


    //���� ����� ���-�� ������� � �������
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerUp && currentPowerUp == PowerUpType.Pushback)
        {
            //�������� ��������� ����� � ������� �����������
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            // �������� ����� �������� ����� �� ������ 
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);

            //��������� ���� �� �����, � ��������(������������ �������� �� ������)
            enemyRigidbody.AddForce(awayFromPlayer * strengthMode * powerUpStrength, ForceMode.Impulse);

            Debug.Log("Player collided with:" + collision.gameObject.name + "with powerup set to " + currentPowerUp.ToString());
        }

        
    }
}