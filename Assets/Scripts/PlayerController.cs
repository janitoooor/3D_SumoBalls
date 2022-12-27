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
        
        ////привязываем движение индекатора к движению игрока
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

    //метод для взаимодействия с триггерами
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            Destroy(other.gameObject);
            powerUpIndicator.gameObject.SetActive(true);//SetActive функция которая включает или выключает обьект на сцене
            Debug.Log("You have PowerUp!");
        }
        if (powerupCountdown != null)
        {
            StopCoroutine(powerupCountdown);
        }
        powerupCountdown = StartCoroutine(PowerCountdownRoutine()); ////Запускаем обратный отсчёт
    }
    // IEnumerator метод способный делать определённые вещи вне нашего цикла Update()
    IEnumerator PowerCountdownRoutine()
    {
        //WaitForSeconds функция подождите несколько секунд, запускается с помощью Coroutines
        yield return new WaitForSeconds(powerUpDuration); //yield  позволяет нам запустить функцию WaitForSeconds
        currentPowerUp = PowerUpType.None;
        hasPowerUp = false;
        powerUpIndicator.gameObject.SetActive(false);
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();

        //Сохраняем позицию y перед взлётом
        floorY = transform.position.y;

        // Рассчитаем количество времени, за которое мы будем подниматься
        float jumpTime = Time.time + hangTime;

        // перемещаем игрока вверх, сохраняя при этом его скорость x.
        while (Time.time < jumpTime)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        //перемещаем игрока вниз 
        while(transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * xMod);
            yield return null;
        }

        //пройтись по всем врагам
        for (int i = 0; i < enemies.Length; i++)
        {
            //применяем силу взрыва, которая исходит из нашей позиции
            if(enemies[i] != null)
            enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
        }
        
            //// Мы больше не разбиваем, поэтому устанавливаем логическое значение в false
        smashing = false;
    }


    //если нужно что-то сделать с физикой
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerUp && currentPowerUp == PowerUpType.Pushback)
        {
            //получаем компонент врага с которым столкнулись
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            // получаем линию движения врага от игрока 
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);

            //Применяем силу на врага, с вектором(направлением движения от игрока)
            enemyRigidbody.AddForce(awayFromPlayer * strengthMode * powerUpStrength, ForceMode.Impulse);

            Debug.Log("Player collided with:" + collision.gameObject.name + "with powerup set to " + currentPowerUp.ToString());
        }

        
    }
}