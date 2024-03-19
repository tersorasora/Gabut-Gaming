using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 1f;
    Rigidbody2D enemyRB;
    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        enemyRB.velocity = new Vector2(movementSpeed, 0f);
    }

    void OnTriggerExit2D(Collider2D other) {
        movementSpeed = -movementSpeed;
        flipSprite();
    }

    void flipSprite(){
        transform.localScale = new Vector2(-Mathf.Sign(enemyRB.velocity.x), 1f);
    }
}
