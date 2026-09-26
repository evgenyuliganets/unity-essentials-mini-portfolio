using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float rotationSpeed;
    public GameObject onCollectEffect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed);
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        //Destroy the collectible object when the player collides with it
        if (onCollectEffect != null)
        {
            Instantiate(onCollectEffect, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}