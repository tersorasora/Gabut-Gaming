using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenThrowed : MonoBehaviour
{
    Rigidbody2D shurikenRB;
    [SerializeField] float shurikenSpeed = 5f;
    [SerializeField] float rotationSpeed = 3f;
    playerMovement arahPlayer;
    float xShuriken;
    float xRotation;
    void Start()
    {
        shurikenRB = GetComponent<Rigidbody2D>();
        arahPlayer = FindObjectOfType<playerMovement>();
        xShuriken = arahPlayer.transform.localScale.x * shurikenSpeed;
        xRotation = arahPlayer.transform.localScale.x * rotationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        shurikenRB.velocity = new Vector2(xShuriken, 0f);
        shurikenRB.rotation += -xRotation;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy"){
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
            Destroy(gameObject);
    }
}
