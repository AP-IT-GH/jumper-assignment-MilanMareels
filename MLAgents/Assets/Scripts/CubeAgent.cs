using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class CubeAgent : Agent
{
    [SerializeField]
    Transform _target = null;

    [SerializeField]
    float _speed = 1.0f;

    // Called when the agent is first initialized
    // public override void Initialize()
    // {
    // }

    // Called when the agent requests an action after a reset or a decision for example
    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        {
            resetPosition();
        }

        this._target.localPosition = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
    }

    private void resetPosition()
    {
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        this.transform.rotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this._target.localPosition);
        sensor.AddObservation(this.transform.localPosition); // Een vector heeft een x,y en een z dus je observeert in totaal 6 waardes (3 voor de target en 3 voor de agent)
    }

    // Called when the agent receives an action to execute. Something like the Update method
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controller = Vector3.zero;
        controller.x = actions.ContinuousActions[0];
        controller.z = actions.ContinuousActions[1];

        transform.Translate(controller * this._speed);

        if (transform.position.y < 0)
        {
            EndEpisode();
            return;
        }

        var distanceToTarget = Vector3.Distance(this.transform.localPosition, this._target.localPosition);

        if (distanceToTarget < 1.5)
        {
            SetReward(1);
            EndEpisode();
            return;
        }
    }

    // Called when the agent receives a message from the Academy (for example to reset the agent)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}
