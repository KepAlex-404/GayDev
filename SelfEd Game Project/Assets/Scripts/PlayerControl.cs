using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5;

    [SerializeField]
    Rigidbody2D rb;

    float xMovement;
    float yMovement;

    void Update()
    {
        Vector3 position = transform.position;
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        if (moveX != 0 || moveY != 0)
        {
            xMovement = moveX * moveSpeed;
            yMovement = moveY * moveSpeed;
            rb.velocity = new Vector2(xMovement, yMovement);

        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }
}
