using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRewarder : MonoBehaviour
{
    public float timer = 0f;
    private DroneAgent droneAgent;

    void Start()
    {
        droneAgent = GetComponent<DroneAgent>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= 0.25f)
        {
            droneAgent.RewardTime(0.01f);
            timer = 0;
        }
    }
}