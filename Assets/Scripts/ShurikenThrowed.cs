using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ShurikenThrowed : MonoBehaviour
{
    Rigidbody2D shurikenRB;
    [SerializeField] float shurikenSpeed = 5f;
    [SerializeField] float rotationSpeed = 3f;
    [SerializeField] AudioClip killSfx, crashSfx;
    public AudioSource shurikenAudioSource;
    playerMovement Player;
    float xShuriken;
    float xRotation;
    void Start()
    {
        shurikenRB = GetComponent<Rigidbody2D>();
        Player = FindObjectOfType<playerMovement>();
        xShuriken = Player.transform.localScale.x * shurikenSpeed;
        xRotation = Player.transform.localScale.x * rotationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        shurikenRB.velocity = new Vector2(xShuriken, 0f);
        shurikenRB.rotation += -xRotation;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy"){
            Player.audioSource.clip = killSfx;
            Player.audioSource.Play();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
            Player.audioSource.clip = crashSfx;
            Player.audioSource.Play();
            Destroy(gameObject);
    }
}
