using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

using Random = UnityEngine.Random;

public class GetFoodAgent : Agent
{
    [SerializeField] private Button targetButton;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    private bool hasReachedButton;
    private bool hasFoodSpawned;
    private Transform foodTransform;
    
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-4f, -1f), 0, Random.Range(-3f, 3f));
        targetButton.Reset();
        targetButton.SetSpawnPoint(new Vector3(Random.Range(-4f, -1f), 0, Random.Range(-3f, 3f)));
        targetButton.transform.localPosition = new Vector3(Random.Range(4f, 1f), 0, Random.Range(-3f, 3f));
        hasReachedButton = false;
        hasFoodSpawned = false;
        if (foodTransform != null) {
            Destroy(foodTransform.gameObject);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetButton.transform.localPosition);
        sensor.AddObservation(hasReachedButton ? 1 : 0);
        sensor.AddObservation(hasFoodSpawned ? 1 : 0);

        if (hasFoodSpawned) {
            sensor.AddObservation(foodTransform.localPosition);
        } else {
            sensor.AddObservation(Vector3.zero);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        bool useButton = (actions.DiscreteActions[0] == 0 ? false : true);
        if (hasReachedButton && useButton && !hasFoodSpawned) {
            if (targetButton.TryPressButton(out Transform foodTransform)) {
                this.foodTransform = foodTransform;
                hasFoodSpawned = true;
                AddReward(1);
            }
        }

        float moveSpeed = 3f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Convert.ToInt32(Input.GetKey(KeyCode.F));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Button>(out Button button)) {
            hasReachedButton = true;

            if (!hasFoodSpawned) {
                AddReward(1f);
            }
        } else if (other.TryGetComponent<Wall>(out Wall wall)) {
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        } else if (other.TryGetComponent<Goal>(out Goal goal)) {
            AddReward(5f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Button>(out Button button)) {
            hasReachedButton = false;

            if (!hasFoodSpawned) {
                AddReward(-1f);
            }
        }
    }
}
