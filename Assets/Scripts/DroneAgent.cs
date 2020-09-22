using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class DroneAgent : Agent
{
    [SerializeField] [Range(0f, 10f)] private float speed = 10.0f;
    [SerializeField] private float distanceRequired = 1.5f, distanceToEnemy = 1.5f;
    [SerializeField] private GameObject target/*, enemy*/;
    [SerializeField] private Material defaultMaterial, successMaterial, faliureMaterial;
    [SerializeField] private MeshRenderer groundMeshRenderer;
    [SerializeField] private Transform fan1,fan2,fan3,fan4;

    private TimeRewarder timeRewarder;
    private Rigidbody playerRigidBody;
    private Vector3 originalPosition, originalTargetPosiion;

    public override void Initialize()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        originalPosition = transform.localPosition;
        originalTargetPosiion = target.transform.localPosition;
        timeRewarder = GetComponent<TimeRewarder>();
        transform.eulerAngles = Vector3.zero;
        timeRewarder.timer = 0f;
    }

    public override void OnEpisodeBegin()
    {
        playerRigidBody.velocity = Vector3.zero;
        target.transform.localPosition = new Vector3(originalTargetPosiion.x + Random.Range(-10f, 10f), Random.Range(5f, 20f), originalTargetPosiion.z + Random.Range(-10f, 10f));
        transform.localPosition = new Vector3(originalPosition.x + Random.Range(-13f,13f), Random.Range(1.5f,10f), originalPosition.z + Random.Range(-13f,13f));
        transform.eulerAngles = Vector3.zero;
        timeRewarder.timer = 0f;
    }

    public void RewardTime(float reward)
    {
        SetReward(reward);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.eulerAngles);
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(playerRigidBody.velocity);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 f1 = transform.up * Mathf.Abs(vectorAction[0]);
        Vector3 f2 = transform.up * Mathf.Abs(vectorAction[1]);
        Vector3 f3 = transform.up * Mathf.Abs(vectorAction[2]);
        Vector3 f4 = transform.up * Mathf.Abs(vectorAction[3]);

        playerRigidBody.AddForceAtPosition(f1 * speed, fan1.position);
        playerRigidBody.AddForceAtPosition(f2 * speed, fan2.position);
        playerRigidBody.AddForceAtPosition(f3 * speed, fan3.position);
        playerRigidBody.AddForceAtPosition(f4 * speed, fan4.position);


        var distanceFromTarget = Vector3.Distance(transform.localPosition, target.transform.localPosition);
        
        if(distanceFromTarget <= distanceRequired)
        {
            SetReward(1.0f);
            EndEpisode();
            timeRewarder.timer = 0f;
            StartCoroutine(SwapGroundMaterial(successMaterial,0.5f));
        }

        if(transform.localPosition.y < 0.3f || transform.localPosition.y > 30f || transform.localPosition.x > originalPosition.x+15f || transform.localPosition.x < originalPosition.x-15f || transform.localPosition.z > originalPosition.z+15f || transform.localPosition.z < originalPosition.z-15f )
        {
            if(transform.localPosition.y < 0.3f)
                SetReward(-0.05f);
            else
                SetReward(-0.01f);
            EndEpisode();
            timeRewarder.timer = 0f;
            StartCoroutine(SwapGroundMaterial(faliureMaterial,0.5f));
        }
    }

    public override void Heuristic(float[] actionOut)
    {
        actionOut[0] = Input.GetAxis("Fan1");
        actionOut[1] = Input.GetAxis("Fan1");
        actionOut[2] = Input.GetAxis("Fan1");
        actionOut[3] = Input.GetAxis("Fan1");
    }

    private IEnumerator SwapGroundMaterial(Material mat, float time)
    {
        groundMeshRenderer.material = mat;
        yield return new WaitForSeconds(time);
        groundMeshRenderer.material = defaultMaterial;
    }
}
