using UnityEngine;
using System.Collections;

// Суть дела - упрощенный скрипт ИИ противника
// Используется для снарядов. Вешается на родителя.
public class BulletAI : MonoBehaviour
{
	//Наносимый снарядом урон
	[SerializeField]
	float damage = 1.0f;

	//Скорость снаряда
	[SerializeField]
	float speed = 1.0f;

	//Rigibody2D снаряда
	[SerializeField]
	Rigidbody2D rb;

	//Будет ли снаряд наводиться на игрока
	[SerializeField]
	bool homing = false;

	//Цель снаряда. По умолчанию - персонаж игрока
	[SerializeField]
	Transform target;

	//Длительность жизни снаряда.
	//Оставь на нуле, чтобы снаряд существовал вечно
	[SerializeField]
	float lifetime = 0.0f;

	//Звук при появлении. Можно оставить пустым
	[SerializeField]
	AudioClip spawnSound;


	//Переменные под данные перемещения, разворота и дистанции между снарядом и игроком.
	float xMovement;
	float yMovement;
	Vector2 movement;
	Vector3 direction;
	float angle;
	float lifeTimer;

	void Start()
	{
		//Если есть звук появления - проиграть его
		if (spawnSound)
        {
			AudioSource.PlayClipAtPoint(spawnSound, transform.position);
		}

		//Если цель не задана - найти объект с тегом "Player" и поставить его целью
		if (target == null)
		{
			if (GameObject.FindWithTag("Player") != null)
			{
				target = GameObject.FindWithTag("Player").GetComponent<Transform>();
			}
		}

		// Если снаряд НЕ самонаводящийся - лететь в сторону персонажа на момент появления снаряда 
		if (!homing)
		{

			//Вектор перемещения считаеться как разника положений объекта погони и противника
			xMovement = (target.position.x - transform.position.x);
			yMovement = (target.position.y - transform.position.y);
			movement = new Vector2(xMovement, yMovement);
			rb.velocity = movement.normalized * speed;

			//Важный момент - .normalized делает из вектора единичный вектор.
			//Сделано это для избежания бага с диагональным движением.
			//После этого умножает вектор на заданную скорость передвижения.

			//Расчитывает угол, на который надо крутиться (математека)
			direction.x = target.position.x - transform.position.x;
			direction.y = target.position.y - transform.position.y;
			angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		}
	}

	void Update()
	{
		//Если цель куда то пропала то ну шо делать - только лететь себе дальше
		if (target == null)
			return;

		//Если снаряд самонаводящийся - расчитывать направление движения каждый кадр
		if (homing)
		{
			xMovement = (target.position.x - transform.position.x);
			yMovement = (target.position.y - transform.position.y);
			movement = new Vector2(xMovement, yMovement);
		}
		//Если не самонаводящийся - продолжать лететь по изначально заданой траектории
		rb.velocity = movement.normalized * speed;

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
		if (homing)
		{
			direction.x = target.position.x - transform.position.x;
			direction.y = target.position.y - transform.position.y;
			//Расчитывает угол, на который надо крутиться (математека)
			angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		}
	}


	//Алгоритм столкновения снаряда с игроком или стеной
	private void OnTriggerEnter2D(Collider2D triggerCollision)
	{
		//При столкновении со стеной снаряд уничтожается
		if(triggerCollision.gameObject.tag == "Wall")
        {
			Destroy(gameObject);
        }

		//При столкновении с игроком снаряд уничтожается а игрок получает урон 
		if (triggerCollision.gameObject.tag == "Player")
		{
			triggerCollision.gameObject.GetComponent <PlayerController>().DealDamage(damage);
			Destroy(gameObject);
		}
	}


}