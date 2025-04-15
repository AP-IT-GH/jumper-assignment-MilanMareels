using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CubeAgentJumper : Agent
{
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _targetSpeed = 1.2f;
    private Rigidbody _rb;
    private bool _isGrounded = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        ResetAgent();
        ResetTarget();
        _isGrounded = false;  // Reset grounded state at the beginning of the episode
    }

    private void ResetAgent()
    {
        transform.localPosition = new Vector3(0, 0.5f, 0); // Reset position
        transform.rotation = Quaternion.identity;  // Reset rotation
        _rb.linearVelocity = Vector3.zero;  // Correct property for resetting velocity
        _rb.angularVelocity = Vector3.zero;  // Reset angular velocity
    }

    private void ResetTarget()
    {
        _target.gameObject.SetActive(true);
        _target.localPosition = new Vector3(0, 0.5f, 5f);  // Reset target position
        Rigidbody targetRb = _target.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            targetRb.linearVelocity = Vector3.zero;  // Reset target velocity if it has a Rigidbody
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_target.localPosition - transform.localPosition);  // Observation of target's position relative to agent
        sensor.AddObservation(_isGrounded ? 1.0f : 0.0f);  // Observation of whether the agent is grounded
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float jumpAction = actions.ContinuousActions[0];  // The action of jumping

        // Only jump if the agent is grounded and the action is above the threshold
        if (jumpAction > 0.5f && _isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);  // Apply an upward force
            _isGrounded = false;  // The agent is no longer grounded after jumping
        }

        _target.Translate(Vector3.back * _targetSpeed * Time.deltaTime);  // Move the target backward

        // If the agent has passed the target and is grounded, reward and end the episode
        if (_isGrounded && transform.position.z > _target.position.z + 0.5f)
        {
            SetReward(1f);
            EndEpisode();
        }

        // End episode if the agent falls below a certain height
        if (transform.position.y < -1f)
        {
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;  // Agent is grounded after colliding with the ground
        }
        if (collision.gameObject.CompareTag("Target"))
        {
            SetReward(-1f);  // Penalize for hitting the target
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        float distance = _target.position.z - transform.position.z;
        if (distance < 1.5f && distance > 0.1f && _isGrounded)
        {
            continuousActions[0] = 1f;  // Jump if close to the target
        }
        else
        {
            continuousActions[0] = 0f;  // Do nothing if not close enough
        }
    }
}
