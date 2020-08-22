using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Основной скрипт управления персонажем. Вешается на родителя.
public class PlayerController : MonoBehaviour
{

    //Спрайты персонажа. В последствии надо будет заменить на нормальную анимацию.
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite idleSprite;
    [SerializeField]
    Sprite attackSprite;

    //Скорость передвижения
    [SerializeField]
    float moveSpeed = 5;

    //Rigibody2D родителя
    [SerializeField]
    Rigidbody2D rb;

    //Дочерние пустые объекты-триггерколлайдеры. По дефолту выключены. 
    [SerializeField]
    GameObject attackColliderLeft;
    [SerializeField]
    GameObject attackColliderRight;

    //Звук атаки
    [SerializeField]
    AudioClip attackSound;

    //Текущее и максимальное ХП
    [SerializeField]
    float MaxHP;
    [SerializeField]
    float HP;

    //Длительность неуязвимости после получения урона
    [SerializeField]
    float Invincibility = 1.5f;

    //"Неуязвимость" и её таймер
    bool invincible = false;
    float invTimer = 0;

    //Цифра на основном UI отвечающая за ХП
    [SerializeField]
    Text healthText;

    //Цифра на основном UI отвечающая за количество зарядов атаки
    [SerializeField]
    Text chargesText;

    //Красный UI "боли" который включается при получении урона
    [SerializeField]
    GameObject painCanvas;

    //Переменные под вводные данные по перемещению и атаке
    float moveX;
    float moveY;
    float attack;

    //Таймеры атаки и паузы между атаками
    float attackTimer;
    float attackCooldownTimer;
    bool isAttacking = false;
    bool isInCooldown = false;

    //Сила, которая прикладывается к персонажу во время атаки влево и вправо соответственно
    Vector2 chargeLeft = new Vector2(-10, 0);
    Vector2 chargeRight = new Vector2(10, 0);

    //Количество зарядов атаки и таймер перезарядки
    int attackCharges = 3;
    float attackRechargeTimer;

    private void Start()
    {
        //Выставляет изначальное значение ХП и зарядов атаки
        healthText.text = HP.ToString();
        chargesText.text = attackCharges.ToString();
    }

    void Update()
    {
        //Считывание вводных данных
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        attack = Input.GetAxis("Fire1");

        //Таймер перезарядки атаки
        if (attackCharges < 3)
        {
            attackRechargeTimer += Time.deltaTime;
            //Если зарядов меньше трех, каждую секунду сбрасывать таймер на ноль и добавлять один заряд
            if (attackRechargeTimer > 1)
            {
                attackRechargeTimer = 0;
                attackCharges++;
                chargesText.text = attackCharges.ToString();
            }
        }

        //Алгоритм передвижения. Передвижение во время атаки запрещено
        if (moveX != 0 || moveY != 0)
        {
            //При движении влево развернуть спрайт персонажа.
            //Во время атаки разворачиваться нельзя
            if (moveX < -0.01f && !isAttacking)
            {
                spriteRenderer.flipX = true;
            }
            //При движении вправо вернуть спрайт в исходное положение
            else if (moveX > 0.01f && !isAttacking)
            {
                spriteRenderer.flipX = false;
            }
            if (!isAttacking)
            {
                //Делает вектор из вводных данных перемещения
                Vector2 movement = new Vector2(moveX, moveY);
                //Важный момент - .normalized делает из вектора единичный вектор.
                //Сделано это для избежания бага с диагональным движением.
                //После этого умножает вектор на заданную скорость передвижения.
                rb.velocity = movement.normalized * moveSpeed;
            }
        }
        else
        {
            //Старая техника "торможения". Без неё персонаж долгое время "скользил" по поверхности.
            //Проблема была в том, что она насильно "тормозила" персонажа, когда пользователь не нажимал
            //на кнопки передвижения. А еще нельзя было реализовать рывок-атаку.

            //Сейчас вместо неё я использую параметр Linear Drag в Rigibody2D (около десятки),
            //Он добавляет объекту больше "трения" но все равно позволяет немножко скользить.
            //Имхо, как раз то, что нужно для динамичного геймплея.

            //rb.velocity = new Vector2(0, 0);
        }


        //Алгоритм атаки
        if (attack != 0 && !isAttacking && !isInCooldown && attackCharges != 0)
        {
            //Забирает один заряд и обновляет инфу о зарядах на интерфейсе
            attackCharges--;
            chargesText.text = attackCharges.ToString();
            //Если есть звук атаки - проиграть его
            if (attackSound)
            {
                //PlayClipAtPoint создает на позиции персонажа пустой объект с источником звука,
                //который удаляется при проигрыше.
                AudioSource.PlayClipAtPoint(attackSound, transform.position);
            }
            //Если персонаж смотрит влево, то он будет атаковать влево, соответственно, активируется левый хитбокс
            if (spriteRenderer.flipX)
            {
                attackColliderLeft.SetActive(true);
                rb.velocity = chargeLeft;
            }
            //Если вправо, то правый
            else
            {
                attackColliderRight.SetActive(true);
                rb.velocity = chargeRight;
            }
            //Меняет дефолтный спрайт персонажа на спрайт атаки.
            
            //!! Надо заменить анимацией

            spriteRenderer.sprite = attackSprite;

            //Запускает таймер атаки. В это время персонаж атакует и не может двигаться или разворачиваться
            isAttacking = true;
            attackTimer = 0;

            //Дает персонажу неполный период неуязвимости во время атаки.
            //Сделано для того, чтобы персонаж мог "толкать" врагов, не получая урон от столкновения
            invincible = true;
            invTimer = 1.0f;
        }

        //Процесс атаки
        if (isAttacking)
        {
            //Атака длиться 0.4 секунды. По прошествию этого времени ...
            attackTimer += Time.deltaTime;
            if (attackTimer >= 0.4f)
            {
                // ... отключаються оба коллайдера атаки ...
                attackColliderLeft.SetActive(false);
                attackColliderRight.SetActive(false);
                // ... спрайт меняется на обычный (опять же, нужна анимация!) ...
                spriteRenderer.sprite = idleSprite;
                // ... и персонаж выходит из режима "атаки" в режим "перезарядки" и запускает её таймер. 
                isAttacking = false;
                isInCooldown = true;
                attackCooldownTimer = 0;

            }
        }

        //Процесс перезарядки атаки
        if (isInCooldown)
        {
            //Перезарядка длиться всего 0.2 секунды. Просто чтобы не спамили :/
            attackCooldownTimer += Time.deltaTime;
            if (attackCooldownTimer >= 0.2f)
            {
                isInCooldown = false;
            }
        }

        //Система хитпоинтов
        //Если персонаж получает хил свыше максимального ХП, то оно приравнивается к максимальному
        if (HP > MaxHP)
        {
            HP = MaxHP;
        }
        //ХП меньше нуля? смэртб.
        if (HP <= 0)
        {
            HP = 0;
            PlayerDeath();
        }
        //Получение урона - ниже, отдельным методом

        //Неуязвимость после получения урона
        if (invincible == true)
        {
            //Таймер неуязвимости 
            invTimer += Time.deltaTime;

            //Первые 0.2 секунды игрок видит красный оверлей
            // (Включается он в методе получения урона)
            if (invTimer >= 0.2f)
            {
                painCanvas.SetActive(false);
            }

            //Неузвимость выключаеться по прошествии определенного времени
            if (invTimer >= Invincibility)
            {
                invincible = false;
            }
        }


    }

    //Алгоритм смерти
    private void PlayerDeath()
    {
        //Пока что просто перезагружает уровень
        SceneManager.LoadScene("TestLevel");
    }

    //Метод получения урона (вызывается противниками)
    public void DealDamage(float amount)
    {
        //Игрок получает урон, только если он не неуязвим
        if (!invincible)
        {
            //Отнимает хп, включает неуязвимость, ставит таймер и включает красный оверлей
            HP -= amount;
            invTimer = 0;
            invincible = true;
            painCanvas.SetActive(true);

            //Обновляет информацию о ХП на интерфейсе
            healthText.text = HP.ToString();
        }
    }
}