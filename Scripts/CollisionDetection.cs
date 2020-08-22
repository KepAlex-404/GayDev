using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Тестовый скрипт обнаружения столкновений
//Сейчас нигде не используется
public class CollisionDetection : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("Pickup detected!");
        Destroy(gameObject);
    }
}
