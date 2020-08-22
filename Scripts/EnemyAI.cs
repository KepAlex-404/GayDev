using UnityEngine;
using System.Collections;

//Универсальный скрипт ИИ противника. Вешается на родителя.
//Существует упрощенная версия для снарядов.
public class EnemyAI : MonoBehaviour
{
	//Rigibody2D противника
	[SerializeField]
	Rigidbody2D rb;

	//Будет ли противник гнаться за персонажем
	[SerializeField]
	bool chase = false;

	//Растояние, на котором противник видит/теряет персонажа
	[SerializeField]
	float detectionRange = 10.0f;

	//Объект погони. По умолчанию - персонаж игрока.
	[SerializeField]
	Transform chaseTarget;

	//Будет ли противник крутиться в сторону направления движения
	[SerializeField]
	bool rotateOnMovement = false;
	//P.S. Если у противника отключен chase/нулевая скорость, то он всегда будет
	//смотреть на игрока, словно турель

	//Скорость движения
	[SerializeField]
	float speed = 1.0f;

	//Минимальная дистанция, на которую подойдет противник к игроку
	[SerializeField]
	float minDistance = 0f;

	//Длительность жизни противника
	//Оставь на нуле, чтобы противник жил вечно
	[SerializeField]
	float lifetime = 0.0f;

	//Наносит ли противник урон при столкновении с игроком
	[SerializeField]
	bool damageOnImpact = false;
	
	//Наносимый урон
	[SerializeField]
	float damage = 0f;

	//Разрушаеться ли противник при столкновении с игроком
	[SerializeField]
	bool destroyOnImpact = false;

	//Если у противника есть спавнер снарядов то его нужно запихнуть сюда
	[SerializeField]
	GameObject projectieSpawner;

	//Расстояние между противником и игроком, при котором включается спавнер снарядов
	[SerializeField]
	float attackRange = 10.0f;

	//Переменные под данные перемещения, разворота и дистанции между противником и игроком.
	float xMovement;
	float yMovement;
	Vector2 movement;
	Vector3 direction;
	float angle;
	float distance;

	//Таймер жизни
	float lifeTimer;

	//Рычаг отталкивания и его таймер
	bool pushed = false;
	float pushedTimer = 0;

	void Start()
	{
		//Если не задан объект погони, то найти объект с тегом "Player" и преследовать его
		if (chaseTarget == null)
		{

			if (GameObject.FindWithTag("Player") != null)
			{
				chaseTarget = GameObject.FindWithTag("Player").GetComponent<Transform>();
			}
		}
	}

	void Update()
	{
		//Таймер отталкивания.
		//Пока противника отталкивают атакой он не может двигаться
		if (pushed)
        {
			pushedTimer += Time.deltaTime;
			if (pushedTimer >= 0.5)
            {
				pushed = false;
            }
        }
		//Если у противника есть объект погони, то он каждый кадр расчитывает расстояние до него
		if (chaseTarget != null)
		{
			distance = Vector2.Distance(transform.position, chaseTarget.position);
		}

		//Противник будет двигаться к персонажу если:
		// * Влючен режим погони
		// * Объект погони существует
		// * Игрок находится в пределах радиуса обнаружения
		// * Игрок нахожиться вне пределов минимальной дистанции
		// * Противника не отталкивает игрок
		if (chase && chaseTarget != null && (distance < detectionRange) && (distance > minDistance) && !pushed)
		{
			//Вектор перемещения считаеться как разника положений объекта погони и противника
			xMovement = (chaseTarget.position.x - transform.position.x);
			yMovement = (chaseTarget.position.y - transform.position.y);
			movement = new Vector2(xMovement, yMovement);
			rb.velocity = movement.normalized * speed;

			//Важный момент - .normalized делает из вектора единичный вектор.
			//Сделано это для избежания бага с диагональным движением.
			//После этого умножает вектор на заданную скорость передвижения.
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

		//Если у противника есть спавнер снарядов ...
		if (projectieSpawner)
        {
			// ... то его надо включить, если игрок находиться в пределах радиуса атаки ...
			if (distance < attackRange)
            {
				projectieSpawner.SetActive(true);
            }
			// ... и выключить, если игрок находиться вне него.
            else
            {
				projectieSpawner.SetActive(false);
			}
        }

		//Если у противника ограниченное время жизни ...
		if (lifetime != 0)
        {
			lifeTimer += Time.deltaTime;
			// ... его нужно удалить по окончанию этого времени 
			if (lifeTimer >= lifetime)
			{
				Destroy(gameObject);
			}
        }

	}

	//Не до конца шарю как этот лейт апдейт работает, но вроде как
	//он вызываеться каждый кадр после всех остальных апдейтов
	void LateUpdate()
	{
		//Алгоритм кручения объекта при движении
		if (rotateOnMovement)
		{
			direction.x = chaseTarget.position.x - transform.position.x;
			direction.y = chaseTarget.position.y - transform.position.y;
			//Расчитывает угол, на который надо крутиться (математека)
			angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		}
	}

	//Алгоритм столкновения противника с игроком
	private void OnCollisionEnter2D(Collision2D collision)
	{
		//Если противник наносит урон при столкновении и он столкнулся действительно с игроком ...
		if (collision.gameObject.tag == "Player" && damageOnImpact)
		{
			// ... то игрок получает урон
			collision.gameObject.GetComponent<PlayerController>().DealDamage(damage);
		}
		//Если противник уничтожается при столкновении и он столкнулся действительно с игроком ...
		if (collision.gameObject.tag == "Player" && destroyOnImpact)
        {
			// .. то он уничтожается. вау. 
			Destroy(gameObject);
        }
	}

	//Тоже, что и выше, но для триггер-коллайдеров типо шипов
	private void OnTriggerEnter2D(Collider2D triggerCollision)
    {
		if (triggerCollision.gameObject.tag == "Player" && damageOnImpact)
		{
			triggerCollision.gameObject.GetComponent<PlayerController>().DealDamage(damage);
		}
		if (triggerCollision.gameObject.tag == "Player" && destroyOnImpact)
		{
			Destroy(gameObject);
		}
	}

	//Тоже, что и выше, но для тех случаев, когда игрок не хочет выходить из шипов
    private void OnTriggerStay2D(Collider2D triggerCollision)
    {
		if (triggerCollision.gameObject.tag == "Player" && damageOnImpact)
		{
			triggerCollision.gameObject.GetComponent<PlayerController>().DealDamage(damage);
		}
	}

	//Метод отталкивания
	// (Вызывается хитбоксами атаки игрока)
	public void Push(Vector2 push)
    {
		//Толкает противника и запускает таймер отталкивания
		pushed = true;
		rb.AddForce(push);
		pushedTimer = 0;
    }
}