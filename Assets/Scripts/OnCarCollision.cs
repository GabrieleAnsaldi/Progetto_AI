using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using VehicleBehaviour;
public class OnCarCollision : MonoBehaviour
{
    [SerializeField] GameObject checkpointsParent;
    Car car;
    private void Start()
    {
        car = gameObject.GetComponent<Car>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("OnCollisionEnter");
        gameObject.GetComponent<WheelVehicle>().IsPlayer = true;
        if (collision.gameObject.tag == "railing")
        {
            Debug.Log("Auto colpisce barriera");
            StopCar();
            car.Fail();
        }
        if (collision.gameObject.tag == "checkpoint")
        {
            Debug.Log("Checkpoint");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter");
        if (other.gameObject.tag == "checkpoint")
        {
            int checkpoint = int.Parse(other.gameObject.name.ToString());
            Debug.Log("Auto " +car.id + " colpisce checkpoint " + checkpoint);
            if (checkpoint - car.Score == 1)
            {
                car.ReachedCheckpoint(checkpoint);
            }
            else if (checkpoint == 0 && car.Score == checkpointsParent.transform.childCount) //finito il giro
            {
                car.LapFinished = true;

                StopCar();
            }
            else StopCar(); //saltato un checkpoint
        }
    }

    public void StopCar()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<WheelVehicle>().IsPlayer = false;
    }
}
