using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;

    void OnTriggerEnter2D(Collider2D other) {

        // TODO: use 'other' to see if it is enemy and do stuff

        GameObject effect = Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);
        Destroy(effect, 0.6f);
        Destroy(gameObject);    
    }
}
