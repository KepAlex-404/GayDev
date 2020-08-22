using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    //Система хитпоинтов противников.
    //Мало чем отличается от системы ХП персонажа


    //Максимальное и текущее ХП
    [SerializeField]
    float MaxHP;
    [SerializeField]
    float HP;

    //Длительность неуязвимости
    [SerializeField]
    float Invincibility = 1.5f;
    //Звук получения урона
    [SerializeField]
    AudioClip hurtSound;

    //Переключатель неуязвимости и её таймер
    bool invincible = false;
    float invTimer = 0;

    void Update()
    {
        //Если противник получает хил свыше максимального ХП, то оно приравнивается к максимальному
        if (HP > MaxHP)
        {
            HP = MaxHP;
        }

        //ХП меньше нуля? смэртб.
        if (HP <= 0)
        {
            HP = 0;
            EnemyDeath();
        }
        //Получение урона - ниже, отдельным методом

        //Неуязвимость после получения урона
        if (invincible == true)
        {
            //Таймер неуязвимости
            invTimer += Time.deltaTime;

            //Первые 0.2 секунды после получения урона противник красится в
            //красный цвет (в скрипте получения урона), после чего - обратно в белый(нейтральный) 
            if (invTimer >= 0.2f)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
            }

            //Неузвимость выключаеться по прошествии определенного времени
            if (invTimer >= Invincibility)
            {
                invincible = false;
            }
        }
    }

    //Алгоритм смерти
    private void EnemyDeath()
    {
        //Пока что просто удаляет объект
        Destroy(gameObject);
    }

    //Метод получения урона (вызывается хитбоксами атаки игрока)
    public void DealDamage(float amount)
    {
        //Противник получает урон, только если он не неуязвим
        if (!invincible)
        {
            //Проигрывает звук получения урона, если такой есть
            if (hurtSound)
            {
                //PlayClipAtPoint создает на позиции персонажа пустой объект с источником звука,
                //который удаляется при проигрыше
                AudioSource.PlayClipAtPoint(hurtSound, transform.position);
            }
            //Отнимает хп, включает неуязвимость, ставит таймер и красит противника в красный
            HP -= amount;
            invTimer = 0;
            invincible = true;
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
