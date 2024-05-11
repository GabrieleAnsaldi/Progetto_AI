using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using VehicleBehaviour;


public class Car : MonoBehaviour
{
    public NeuralNetwork NN;
    public bool Eliminated = false;
    public int Score = 0;
    public bool LapFinished = false;
    public int MaxScore = 0;
    public float viewdistance = 10f;

    public event EventHandler CarStopped;
    float[] outputs;

    /*void Start()
    {
        NN = new NeuralNetwork(new int[] { 7, 10, 10, 2 });
    }*/
    public void Update()
    {
        outputs = NN.Brain(CreateRaycasts(7, 150f / 7f));
        MoveCar(outputs[0], outputs[1]);
    }

    private void MoveCar(float speed, float turn)
    {
        gameObject.GetComponent<WheelVehicle>().Move(speed, turn);
    }

    public float[] CreateRaycasts(int n_raycasts, float anglebetweenrays)
    {
        float[] distances = new float[n_raycasts];

        RaycastHit hit;
        Vector3 halfExtents = new (0.5f, 0.5f, 0.0001f); // Set the size of the box here
        for (int i = 0; i < n_raycasts; i++)
        {
            float angle = (2 * i + 1 - n_raycasts) * anglebetweenrays / 2;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 direction = rotation * transform.forward;

            Vector3 origin = transform.position + transform.up * 0.01f;
            // You can visualize the box with Debug.DrawRay or Debug.DrawLine
            Debug.DrawLine(origin, origin + direction * viewdistance, Color.green);
            if (Physics.BoxCast(origin, halfExtents, direction, out hit, transform.rotation, viewdistance))
            {
                Debug.DrawLine(origin, hit.point, Color.red); // Visualize hit

                if (hit.transform.gameObject.tag == "railing")
                    distances[i] = hit.distance;
                else
                    distances[i] = viewdistance;
            }
            else
            {
                distances[i] = viewdistance;
            }
        }
        return distances;
    }

    //public GameObject Spline;

    public void Fail()
    {
        //Vector3 carPosition = gameObject.transform.position;
        Stop();
    }

    public void Stop()
    {
        MaxScore = Score;
        Eliminated = true;
        Score = 0;
        CarStopped?.Invoke(gameObject, EventArgs.Empty);
    }
}

















/*public float[] CreateRaycasts(int n_raycasts, float anglebetweenrays)
    {
        float[] distances = new float[n_raycasts];

        RaycastHit hit;
        for (int i = 0; i < n_raycasts; i++)
        {
            float angle = (2 * i + 1 - n_raycasts) * anglebetweenrays / 2;
            //ruota il raggio in base all'angolo
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 direction = rotation * transform.forward;

            //incrementa il punto di partenza del raggio di 0.1 unità
            Vector3 origin = transform.position + transform.up * 0.01f;
            Debug.DrawLine(origin, origin + direction * viewdistance, Color.green);
            if (Physics.Raycast(origin, direction, out hit, viewdistance))
            {
                //disegna la linea del raggio
                Debug.DrawLine(origin, hit.point, Color.red);
                if (hit.transform.gameObject.tag == "railing")
                    distances[i] = hit.distance;
                else
                    distances[i] = viewdistance;
            }
            else
            {
                distances[i] = viewdistance;
            }
        }
        return distances;
    }*/