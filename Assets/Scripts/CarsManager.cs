using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;
using TMPro;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Search;

public class CarsManager : MonoBehaviour
{
    [SerializeField] Camera gameCamera;
    [SerializeField] GameObject bestScoreText;
    [SerializeField] GameObject bestCarText;
    [SerializeField] GameObject carPrefab;
    [SerializeField] Vector3 startPosition;
    [SerializeField] int n_cars;
    public int[] shape = new int[] { 7, 10, 10, 2 };
    NeuralNetwork baseNN;
    List<GameObject> carsObjects = new List<GameObject>();
    List<Car> cars = new List<Car>();
    int BestScore = 0, runningCars, checkpoints;
    Car bestCar;
    Car bestRunningCar;
    int bestRoundScore;
    int previousScore = 0;
    
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
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            if (obj.name == "Trail")
                Destroy(obj);
    }

    void OnCarStop(object sender, EventArgs e)
    {
        //check if the car was the last one running
        Car car = (sender as GameObject).GetComponent<Car>();
        runningCars--;
        if(car.MaxScore > BestScore)
        {
            BestScore = car.MaxScore;
            bestRoundScore = BestScore;
            bestCar = car;
        }
        if (runningCars == 0)
        {
            // new generation with the nn of the best car
            
            MutationAmount = Mathf.Clamp(Mathf.Log(Mathf.Abs(BestScore-checkpoints)), 0, 0.6f);
            MutationChance = Mathf.Clamp(Mathf.Log(Mathf.Abs(BestScore - previousScore)), 0, 0.6f);

            baseNN = new NeuralNetwork(shape, bestCar.NN.CopyLayers());
            bestScoreText.GetComponent<TextMeshProUGUI>().text = "Best Score:" + BestScore;
            bestCarText.GetComponent<TextMeshProUGUI>().text = "Best Car:" + bestCar.id;
            NewGeneration();
        }
        //UpdateCamera();
    }

    public event EventHandler CameraUpdateHandler;

    void OnCheckpointReached(object sender, EventArgs e)
    {
        Car car = (sender as GameObject).GetComponent<Car>();
        if (car.MaxScore > bestRoundScore)
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
        previousScore = bestRoundScore;
        bestRoundScore = 0;
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
