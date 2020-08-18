using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
	[SerializeField]
	Rigidbody2D rb;

	[SerializeField]
	bool chase = false;

	[SerializeField]
	float detectionRange = 10.0f;

	[SerializeField]
	Transform chaseTarget;

	[SerializeField]
	bool rotateOnMovement = false;
	
	[SerializeField]
	float speed = 1.0f;

	[SerializeField]
	float minDistance = 0f;

	[SerializeField]
	bool isProjectile = false;

	[SerializeField]
	//Leave at zero to last forever
	float lifetime = 0.0f;

	[SerializeField]
	bool damageOnImpact = false;

	[SerializeField]
	float damage = 0f;

	[SerializeField]
	bool destroyOnImpact = false;

	[SerializeField]
	GameObject projectieSpawner;

	[SerializeField]
	float attackRange = 10.0f;


	float xMovement;
	float yMovement;
	Vector2 movement;
	Vector3 direction;
	float angle;
	float distance;
	float lifeTimer;

	void Start()
	{
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
		if (chaseTarget != null)
		{
			distance = Vector2.Distance(transform.position, chaseTarget.position);
		}
		if (chase && distance < detectionRange && (distance > minDistance) && chaseTarget != null)
		{
			xMovement = (chaseTarget.position.x - transform.position.x);
			yMovement = (chaseTarget.position.y - transform.position.y);
			movement = new Vector2(xMovement, yMovement);
			movement = movement.normalized * speed;
			rb.velocity = movement;
		}
        else
        {
			rb.velocity = new Vector2(0, 0);
        }
		if (projectieSpawner)
        {
			if (distance < attackRange)
            {
				projectieSpawner.SetActive(true);
            }
            else
            {
				projectieSpawner.SetActive(false);
			}
        }
		if (lifetime != 0)
        {
			if (lifeTimer >= lifetime)
			{
				Destroy(gameObject);
			}
			lifeTimer += Time.deltaTime;
			print(lifeTimer);
        }

	}

	void LateUpdate()
	{
		if (rotateOnMovement)
		{
			direction.x = chaseTarget.position.x - transform.position.x;
			direction.y = chaseTarget.position.y - transform.position.y;
			angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player" && damageOnImpact)
		{
			collision.gameObject.GetComponent<PlayerHealth>().DealDamage(damage);
		}
		if (collision.gameObject.tag == "Player" && destroyOnImpact)
        {
			Destroy(gameObject);
        }
	}
	private void OnTriggerEnter2D(Collider2D triggerCollision)
    {
		if (triggerCollision.gameObject.tag == "Player" && damageOnImpact)
		{
			triggerCollision.gameObject.GetComponent<PlayerHealth>().DealDamage(damage);
		}
		if (triggerCollision.gameObject.tag == "Player" && destroyOnImpact)
		{
			Destroy(gameObject);
		}
	}


}