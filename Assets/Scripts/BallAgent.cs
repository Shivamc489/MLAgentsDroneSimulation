using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BallAgent : Agent
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float distanceRequired = 1.5f, distanceToEnemy = 1.5f;
    [SerializeField] private GameObject target, enemy;
    [SerializeField] private Material defaultMaterial, successMaterial, faliureMaterial;
    [SerializeField] private MeshRenderer groundMeshRenderer;

    private Rigidbody playerRigidBody;
    private Vector3 originalPosition, originalTargetPosiion;

    public override void Initialize()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        originalPosition = transform.localPosition;
        originalTargetPosiion = target.transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        playerRigidBody.velocity = Vector3.zero;
        transform.LookAt(target.transform);
        target.transform.localPosition = originalTargetPosiion;
        transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, Random.Range(-4f,4f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.transform.localPosition);
        // sensor.AddObservation(enemy.transform.localPosition);
        sensor.AddObservation(playerRigidBody.velocity.x);
        sensor.AddObservation(playerRigidBody.velocity.z);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Debug.Log("Hi");
        var vectorForce = new Vector3();
        vectorForce.x = -1*vectorAction[0];
        vectorForce.z = -1*vectorAction[1];

        playerRigidBody.AddForce(vectorForce * speed);

        var distanceFromTarget = Vector3.Distance(transform.localPosition, target.transform.localPosition);
        var distanceFromEnemy = Vector3.Distance(transform.localPosition, enemy.transform.localPosition);

        if(distanceFromTarget <= distanceRequired)
        {
            SetReward(1.0f);
            EndEpisode();
            StartCoroutine(SwapGroundMaterial(successMaterial,0.5f));
        }

        if(transform.localPosition.y < 0 || transform.localPosition.x > originalPosition.x+2 || transform.localPosition.x < originalPosition.x-30 || transform.localPosition.z > originalPosition.z+5 || transform.localPosition.z < originalPosition.z-10.5f || distanceFromEnemy <= distanceToEnemy)
        {
            // SetReward(-0.2f);
            EndEpisode();
            StartCoroutine(SwapGroundMaterial(faliureMaterial,0.5f));
        }
    }

    public override void Heuristic(float[] actionOut)
    {
        actionOut[0] = Input.GetAxis("Horizontal");
        actionOut[1] = Input.GetAxis("Vertical");
    }

    private IEnumerator SwapGroundMaterial(Material mat, float time)
    {
        groundMeshRenderer.material = mat;
        yield return new WaitForSeconds(time);
        groundMeshRenderer.material = defaultMaterial;
    }
}
