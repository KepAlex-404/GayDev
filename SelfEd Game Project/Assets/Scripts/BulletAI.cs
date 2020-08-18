using UnityEngine;
using System.Collections;

public class BulletAI : MonoBehaviour
{

	public float speed = 1.0f;
	public float minDist = 1f;
	public Transform target;


	[SerializeField]
	Rigidbody2D rb;

	float xMovement;
	float yMovement;
	Vector2 movement;
	Vector3 direction;
	float angle;

	void Start()
	{
		if (target == null)
		{

			if (GameObject.FindWithTag("Player") != null)
			{
				target = GameObject.FindWithTag("Player").GetComponent<Transform>();
			}
		}
	}

	void Update()
	{
		if (target == null)
			return;

		float distance = Vector2.Distance(transform.position, target.position);
		if (distance > minDist)
		{
			xMovement = (target.position.x - transform.position.x) * speed;
			yMovement = (target.position.y - transform.position.y) * speed;
			movement = new Vector2(xMovement, yMovement);
			rb.velocity = movement.normalized;
		}	

	}

    void LateUpdate()
    {
		direction.x = target.position.x - transform.position.x;
		direction.y = target.position.y - transform.position.y;
		angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

	private void OnTriggerEnter2D(Collider2D triggerCollision)
	{
		if (triggerCollision.gameObject.tag == "Player")
		{
			triggerCollision.gameObject.GetComponent<PlayerHealth>().DealDamage(1);
			Destroy(gameObject);
		}
	}


}