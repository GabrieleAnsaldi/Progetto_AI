using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;
using TMPro;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Search;
using System.Linq;

public class CarsManager : MonoBehaviour
{
    [SerializeField] Camera gameCamera;
    [SerializeField] GameObject BestFitnessText;
    [SerializeField] GameObject bestCarText;
    [SerializeField] GameObject carPrefab;
    [SerializeField] Vector3 startPosition;
    [SerializeField] int n_cars;
    [SerializeField] GameObject BordLeft;
    [SerializeField] GameObject BordRight;
    public int[] shape;
    NeuralNetwork baseNN;
    List<GameObject> carsObjects = new List<GameObject>();
    List<Car> cars = new List<Car>();
    float BestFitness = 0, BestRoundFitness = 0, previousFitness = 0;
    int runningCars, checkpoints;
    Car bestCar;
    Car bestRunningCar;
    
    float MutationAmount = .5f, MutationChance = .5f;

    [SerializeField] GameObject checkpointsParent;
     
    /*List<GameObject> checkpoints = new List<GameObject>();*/
    
    // Start is called before the first frame update
    void Start()
    {
        /*for (int i = 0; i < checkpointsParent.transform.childCount; i++) //lista di checkpoints
            checkpoints.Add(checkpointsParent.transform.GetChild(i).gameObject);*/
        baseNN = new NeuralNetwork(shape);
        NewGeneration();
        bestCar = cars[0];
        bestRunningCar = bestCar;
        checkpoints = checkpointsParent.transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        //Destroy all untagged objects
        /*foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            if (obj.name == "Trail")
                Destroy(obj);*/
        for (int i = 0; i < BordRight.transform.GetChild(0).childCount; i++)
        {
            BordRight.transform.GetChild(0).GetChild(i).gameObject.layer = BordRight.layer;
            BordRight.transform.GetChild(0).GetChild(i).gameObject.tag = BordRight.tag;
        }
        for (int i = 0; i < BordLeft.transform.GetChild(0).childCount; i++)
        {
            BordLeft.transform.GetChild(0).GetChild(i).gameObject.layer = BordLeft.layer;
            BordLeft.transform.GetChild(0).GetChild(i).gameObject.tag = BordLeft.tag;
        }
    }

    void OnCarStop(object sender, EventArgs e)
    {
        //check if the car was the last one running
        Car car = (sender as GameObject).GetComponent<Car>();
        runningCars--;
        if(car.fitness > BestFitness)
        {
            BestFitness = car.fitness;
            BestRoundFitness = BestFitness;
            bestCar = car;
        }
        if (runningCars == 0)
        {
            // new generation with the nn of the best car
            
            MutationAmount = Mathf.Clamp(Mathf.Log(Mathf.Abs(BestFitness-checkpoints)), 0, 0.6f);
            MutationChance = Mathf.Clamp(Mathf.Log(Mathf.Abs(BestFitness - previousFitness)), 0, 0.6f);

            baseNN = new NeuralNetwork(shape, bestCar.NN.CopyLayers());
            BestFitnessText.GetComponent<TextMeshProUGUI>().text = "Best Score:" + BestFitness;
            bestCarText.GetComponent<TextMeshProUGUI>().text = "Best Car:" + bestCar.id;
            NewGeneration();
        }
        //UpdateCamera();
    }

    public event EventHandler CameraUpdateHandler;

    void OnCheckpointReached(object sender, EventArgs e)
    {
        Car car = (sender as GameObject).GetComponent<Car>();
        if (car.fitness > BestRoundFitness)
        {
            bestRunningCar = car;
            CameraUpdateHandler?.Invoke(bestRunningCar.gameObject, EventArgs.Empty);
        }
    }

    void NewGeneration()
    {
        foreach (Car car in cars) Destroy(car.gameObject);
        carsObjects = new List<GameObject>();
        cars = new List<Car>();
        previousFitness = BestRoundFitness;
        BestRoundFitness = 0;
        runningCars = n_cars;
        for (int i = 0; i < n_cars; i++)
        {
            carsObjects.Add(Instantiate(carPrefab, startPosition, Quaternion.identity));
            cars.Add(carsObjects[i].GetComponent<Car>());
            NeuralNetwork NN = new(shape, baseNN.CopyLayers());
            NN.Mutate(MutationAmount, MutationChance);
            cars[i].NN = NN;

            cars[i].CarStopped += OnCarStop;
            cars[i].CheckpointReached += OnCheckpointReached;
        }
    }
}
