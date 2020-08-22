using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    //Старый скрипт, раньше использовался на шипах и противниках.
    //Перезагружает уровень при столкновении
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        SceneManager.LoadScene("SampleScene");
    }
    private void OnTriggerEnter2D(Collider2D triggerCollision)
    {
        if (triggerCollision.gameObject.tag == "Player")
            SceneManager.LoadScene("SampleScene");
    }
}
