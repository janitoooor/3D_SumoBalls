using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehavour : MonoBehaviour
{
    private Transform target;
    private bool homing;

    private float rocketStrength = 15.0f;
    private float aliveTimer = 1.5f;
    private float speed = 15.0f;

    

    // Update is called once per frame
    void Update()
    {
        if(homing && target != null)
        {
            Vector3 moveDirection = (target.transform.position - transform.position).normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
            transform.LookAt(target);
        }
    }

    public void Fire(Transform newTarget)
    {
        target = newTarget;
        homing = true;
        Destroy(gameObject, aliveTimer);
    }

    void OnCollisionEnter(Collision col)
    //col = collision//Этот метод сначала проверяет, есть ли у нас цель.
    //Если да, то мы сравниваем тег столкнувшегося объекта с тег цели.Если они совпадают, мы получаем твердое тело цели.
    //Затем мы используем нормаль контакт столкновения, чтобы определить, в каком направлении толкать цель.
    //Наконец, мы применяем силу к цель и уничтожить ракету.
    {
        if (target != null)
        {
            if (col.gameObject.CompareTag(target.tag))
            {
                Rigidbody targetRigidbody = col.gameObject.GetComponent<Rigidbody>();
                Vector3 away = -col.contacts[0].normal;
                targetRigidbody.AddForce(away * rocketStrength, ForceMode.Impulse);
                Destroy(gameObject);
            }
        }

    }

    

  
}
