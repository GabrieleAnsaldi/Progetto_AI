using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using VehicleBehaviour;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;


public class Car : MonoBehaviour
{
    [SerializeField] LayerMask bordersLayer;
    GameObject Manager;
    int RayCasts;

    public NeuralNetwork NN;
    [SerializeField] int time = 10;
    public bool Eliminated = false;
    public bool LapFinished = false;
    public int MaxCheckpoints = 0;
    [SerializeField] float viewdistance;
    static int i = 0;
    public int id;
    Rigidbody rb;

    int TimeAlive;
    public int Checkpoints = 0;
    public float Distance;
    public float DistanceFromLastCheckpoint;

    public Vector3 LastCheckpointPosition;
    public float fitness;

    public event EventHandler CarStopped;
    public event EventHandler CheckpointReached;
    float[] outputs;

    void Start()
    {
        LastCheckpointPosition = transform.position;
        StartTimer();
        id = i++;
        Manager = GameObject.Find("Manager");
        RayCasts = Manager.GetComponent<CarsManager>().shape[0];
        rb = GetComponent<Rigidbody>();
    }

    public void StartTimer()
    {
        time = 250;
        //timer?.Dispose();
        //if(timer != null) timer.Elapsed -= OnTimedEvent;
        //timer = new Timer(time);
        //timer.Elapsed += OnTimedEvent;
        //timer.Start();
    }

    //private void OnTimedEvent(object sender, ElapsedEventArgs e)
    //{
    //    Debug.Log(gameObject.name + " ha finito il tempo.");
    //    Stop();
    //}

    public void Update()
    {
        float speed = transform.InverseTransformDirection(rb.velocity).z * 3.6f;
        List<float> inputs = CreateRaycasts(RayCasts - 1, 150f / (RayCasts -1)).ToList();
        inputs.Add(speed);
        outputs = NN.Brain(inputs.ToArray());
        MoveCar(outputs[0], outputs[1]);
        DistanceFromLastCheckpoint = Vector3.Distance(transform.position, LastCheckpointPosition);

        fitness = CalculateFitness();
    }

    public float CalculateFitness()
    {
        float checkpointWeight; 
        float distanceWeight;    
        float timeWeight;        
        float distanceFromLastCheckpointWeight;
        if (Manager.GetComponent<CarsManager>().LapFinished) //una volta finito il giro contano di più i checkpoint e meno il tempo sopravvissuto
        {
            checkpointWeight = 110f; // Assign a high weight if checkpoints are important
            distanceWeight = -.1f;    // Distance might be a base score with a weight of 1
            timeWeight = -.15f;        // If staying alive longer is important, assign a higher weight
            distanceFromLastCheckpointWeight = -.1f; // Assign a high weight if distance from last checkpoint is important
        }
        else
        {
            checkpointWeight = 10.0f; // Assign a high weight if checkpoints are important
            distanceWeight = 1.0f;    // Distance might be a base score with a weight of 1
            timeWeight = .01f;        // If staying alive longer is important, assign a higher weight
            distanceFromLastCheckpointWeight = 1.0f; // Assign a high weight if distance from last checkpoint is important
        }/*
        checkpointWeight = 110f; // Assign a high weight if checkpoints are important
        distanceWeight = -.1f;    // Distance might be a base score with a weight of 1
        timeWeight = -.1f;        // If staying alive longer is important, assign a higher weight
        distanceFromLastCheckpointWeight = -.1f; // Assign a high weight if distance from last checkpoint is important*/

        // Calculate distance from last checkpoint


        // Calculate weighted scores
        float checkpointScore = Checkpoints * checkpointWeight;
        float distanceScore = Distance * distanceWeight;
        float timeScore = TimeAlive * timeWeight;
        float distanceFromLastCheckpointScore = DistanceFromLastCheckpoint * distanceFromLastCheckpointWeight;

        // Combine the scores to calculate the total fitness
        float fitness = checkpointScore + distanceScore + timeScore + distanceFromLastCheckpointScore;

        return fitness;
    }

    private void FixedUpdate()
    {
        time--;
        if(!Eliminated) TimeAlive++;
        if (time <= 0 && !Eliminated)
        {
            Debug.Log("Auto " + gameObject.GetComponent<Car>().id + " ha finito il tempo.");
            Fail();
        }
    }

    private void MoveCar(float speed, float turn)
    {
        gameObject.GetComponent<WheelVehicle>().Move(Math.Max(0, speed), turn);
    }

    //public float[] CreateRaycasts(int n_raycasts, float anglebetweenrays)
    //{
    //    float[] distances = new float[n_raycasts];

    //    RaycastHit hit;
    //    //Vector3 halfExtents = new (.001f, .5f, 0.0001f); // Set the size of the box here
    //    for (int i = 0; i < n_raycasts; i++)
    //    {
    //        float angle = (2 * i + 1 - n_raycasts) * anglebetweenrays / 2;
    //        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
    //        Vector3 direction = rotation * transform.forward;

    //        Vector3 origin = transform.position + transform.up * 0.01f;
    //        // You can visualize the box with Debug.DrawRay or Debug.DrawLine
    //        //Debug.DrawLine(origin, origin + direction * viewdistance, Color.green);
    //        if (Physics.RayCast(origin, direction, out hit, transform.rotation, viewdistance))
    //        {
    //            Debug.DrawRay(origin, direction * hit.distance, Color.yellow);

    //            if (hit.transform.gameObject.tag == "plane")
    //            {
    //                //visualize the box cast
    //                Debug.DrawRay(origin, direction * hit.distance, Color.red);

    //                distances[i] = hit.distance;
    //            }
    //            else
    //                distances[i] = viewdistance;
    //        }
    //        else
    //        {
    //            distances[i] = viewdistance;
    //        }
    //    }
    //    return distances;
    //}

    //public float[] CreateRaycasts(int n_raycasts, float anglebetweenrays)
    //{
    //    float[] distances = new float[n_raycasts];

    //    RaycastHit hit;
    //    for (int i = 0; i < n_raycasts; i++)
    //    {
    //        float angle = (2 * i + 1 - n_raycasts) * anglebetweenrays / 2;
    //        //ruota il raggio in base all'angolo
    //        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
    //        Vector3 direction = rotation * transform.forward;

    //        //incrementa il punto di partenza del raggio di 0.1 unità
    //        Vector3 origin = gameObject.transform.position + transform.up * 0.01f;
    //        //Debug.DrawLine(origin, origin + direction * viewdistance, Color.green);
    //        if (Physics.Raycast(origin, direction, out hit, viewdistance))
    //        {
    //            //disegna la linea del raggio
    //            Debug.DrawLine(origin, hit.point, Color.red);
    //            if (hit.transform.gameObject.tag == "plane" || hit.transform.gameObject.tag == "railing")
    //            {
    //                Debug.Log("hit plane");
    //                distances[i] = hit.distance;
    //            }
    //            else
    //                distances[i] = viewdistance;
    //        }
    //        else
    //        {
    //            distances[i] = viewdistance;
    //        }
    //    }
    //    return distances;
    //}

    public float[] CreateRaycasts(int n_raycasts, float anglebetweenrays)
    {
        float[] distances = new float[n_raycasts];

        RaycastHit hit;

        for (int i = 0; i < n_raycasts; i++)
        {
            float angle = (2 * i + 1 - n_raycasts) * anglebetweenrays / 2;
            Vector3 carForwardXZ = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 direction = rotation * /*transform.forward*/ carForwardXZ;

            Vector3 origin = gameObject.transform.position + transform.up * 0.01f;
            Debug.DrawLine(origin, origin + direction * viewdistance, Color.green);
            // Use the layer mask in the raycast
            if (Physics.Raycast(origin, direction, out hit, viewdistance, (bordersLayer)))
            {
                Debug.DrawLine(origin, hit.point, Color.red);
                distances[i] = hit.distance;
            }
            else
            {
                distances[i] = viewdistance;
            }
        }
        return distances;
    }

    public void Fail()
    {
        //Vector3 carPosition = gameObject.transform.position;
        Stop();
        GetComponent<OnCarCollision>().StopCar();
    }

    public void Stop()
    {
        Debug.Log("Auto" + id + " è stato eliminata");
        MaxCheckpoints = Checkpoints;
        Eliminated = true;
        Checkpoints = 0;
        CarStopped?.Invoke(gameObject, EventArgs.Empty);
    }

    public void ReachedCheckpoint(int checkpoint)
    {
        Checkpoints = checkpoint;
        MaxCheckpoints = Checkpoints;
        CheckpointReached?.Invoke(gameObject, EventArgs.Empty);
        StartTimer();
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