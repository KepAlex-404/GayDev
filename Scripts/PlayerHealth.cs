using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    //Старый скрипт системы ХП персонажа.
    //Сейчас интегрирован в PlayerController

    
    [SerializeField]
    float MaxHP;
    [SerializeField]
    float HP;
    [SerializeField]
    float Invincibility = 1.5f;

    bool invincible = false;
    float invTimer = 0;

    [SerializeField]
    Text healthText;

    [SerializeField]
    GameObject painCanvas;

    private void Start()
    {
        healthText.text = HP.ToString();
    }
    void Update()
    {
      if(HP > MaxHP)
        {
            HP = MaxHP;
        }
      if(HP <= 0)
        {
            HP = 0;
            PlayerDeath();
        }
      if (invincible == true)
        {
            if (invTimer >= 0.2f)
            {
                painCanvas.SetActive(false);
            }
            if (invTimer >= Invincibility)
            {
                invincible = false;
            }
            else
            {
                invTimer += Time.deltaTime;
            }
        }
    }

    private void PlayerDeath()
    {
        SceneManager.LoadScene("TestLevel");
    }

    public void DealDamage(float amount)
    {
        if (invincible == false)
        {
            HP -= amount;
            invTimer = 0;
            invincible = true;
            painCanvas.SetActive(true);

            healthText.text = HP.ToString();
        }
    }
}
