using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCarCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag == "railing")
        {
            //freeze the car
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            gameObject.GetComponent<
        }
    }
}
