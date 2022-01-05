using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTravel : MonoBehaviour
{
    public int damage;
    public Transform target;

    public void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player"){
            other.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
