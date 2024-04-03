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
            if(shurikenAudioSource.enabled){
                Debug.Log("Audio enabled");
                shurikenAudioSource.clip = killSfx;
                shurikenAudioSource.Play();
            }
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
            if(shurikenAudioSource.enabled){
                Debug.Log("breaks enabled");
                shurikenAudioSource.clip = crashSfx;
                shurikenAudioSource.Play();
            }
            Destroy(gameObject);
    }
}
