using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Вешается на хитбоксы атаки игрока
public class PlayerAttackHitbox : MonoBehaviour
{
    //Наносимый игроком урон
    [SerializeField]
    float damage;

    //Направление толчка (Плюс - вправо, Минус - влево, я ставлю 10 или -10)
    [SerializeField]
    float xDirection = 0;

    //Алгоритм столкновения хитбокса с противником (или разрушаемым объектом)
    private void OnTriggerEnter2D(Collider2D triggerCollision)
    {
        if (triggerCollision.gameObject.tag == "Enemy" || triggerCollision.gameObject.tag == "Breakable")
        {
            //Наносит урон
            triggerCollision.gameObject.GetComponent<EnemyHealth>().DealDamage(damage);

            //Толкает цель
            Vector2 push = new Vector2(xDirection, 0);
            triggerCollision.gameObject.GetComponent<Rigidbody2D>().velocity = push;
        }

    }
}
