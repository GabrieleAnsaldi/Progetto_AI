using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;

public class CarsManager : MonoBehaviour
{
    [SerializeField] GameObject carPrefab;
    [SerializeField] Vector3 startPosition;
    [SerializeField] int n_cars;
    public int[] shape = new int[] { 7, 10, 10, 2 };
    NeuralNetwork baseNN;
    List<GameObject> carsObjects = new List<GameObject>();
    List<Car> cars = new List<Car>();
    int BestScore, runningCars, checkpoints;
    Car bestCar;
    
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
        checkpoints = checkpointsParent.transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCarStop(object sender, EventArgs e)
    {
        
        //check if the car was the last one running
        Car car = (sender as GameObject).GetComponent<Car>();
        runningCars--;
        if(car.MaxScore > BestScore)
        {
            BestScore = car.MaxScore;
            bestCar = car;
        }
        if (runningCars == 0)
        {
            // new generation with the nn of the best car
            
            MutationAmount = Mathf.Clamp(checkpoints - BestScore, 0.01f, 0.5f);
            MutationChance = Mathf.Clamp(checkpoints - BestScore, 0.01f, 0.5f);

            baseNN = new NeuralNetwork(shape, bestCar.NN.CopyLayers());

            NewGeneration();
        }
        //UpdateCamera();
    }

    void NewGeneration()
    {
        foreach (Car car in cars) Destroy(car.gameObject);
        carsObjects = new List<GameObject>();
        cars = new List<Car>();
        runningCars = n_cars;
        for (int i = 0; i < n_cars; i++)
        {
            carsObjects.Add(Instantiate(carPrefab, startPosition, Quaternion.identity));
            cars.Add(carsObjects[i].GetComponent<Car>());
            NeuralNetwork NN = new(shape, baseNN.CopyLayers());
            NN.Mutate(MutationAmount, MutationChance);
            cars[i].NN = NN;

            cars[i].CarStopped += OnCarStop;
        }
    }
}
