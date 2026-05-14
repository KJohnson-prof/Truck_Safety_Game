using UnityEngine;
using TMPro;
using System;

public class ScoreHandler : MonoBehaviour
{
    //makes singleton because only 1 instance of this code should be running
    public static ScoreHandler instance;

    [SerializeField] TextMeshProUGUI distanceTravelledText;

    //Reference
    TruckController playerTruckController;

    float truckStartPositionZ;
    float truckStartPositionX;
    float distanceTravelled = 0;

    public float GetScore() => distanceTravelled;

    private void Awake()
    {
        instance = this;
        playerTruckController = GameObject.FindGameObjectWithTag("Truck").GetComponent<TruckController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        truckStartPositionX = playerTruckController.transform.position.x;
        truckStartPositionZ = playerTruckController.transform.position.z;
    }

    // Update is called once per frame
    private void Update()
    {
        distanceTravelled += (float)Math.Sqrt(Math.Pow(playerTruckController.transform.position.z - truckStartPositionZ, 2) + Math.Pow(playerTruckController.transform.position.x - truckStartPositionX, 2));
        truckStartPositionX = playerTruckController.transform.position.x;
        truckStartPositionZ = playerTruckController.transform.position.z;

        distanceTravelledText.text = distanceTravelled.ToString("000000");
    }
}
