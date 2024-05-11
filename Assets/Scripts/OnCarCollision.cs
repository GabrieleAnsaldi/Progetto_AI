using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class OnCarCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
        gameObject.GetComponent<WheelVehicle>().IsPlayer = true;
        // make it so that if the other object has the tag "railing" then the car will be destroyed
        if (collision.gameObject.tag == "railing")
        {
            Debug.Log("Auto colpisce barriera");
            //freeze the car
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.GetComponent<WheelVehicle>().IsPlayer = false;
            gameObject.GetComponent<Car>().Fail();
        }    
    }
}
