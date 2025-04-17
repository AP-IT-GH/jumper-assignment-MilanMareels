using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CubeAgentJumper : Agent
{
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _jumpForce = 8f;
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

        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.6f);
    }

    private void ResetAgent()
    {
        transform.localPosition = new Vector3(0, 0.5f, 0);
        transform.rotation = Quaternion.identity;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        _isGrounded = false;
    }

    private void ResetTarget()
    {
        _target.gameObject.SetActive(true);

        float randomSpeed = Random.Range(1f, 5f);
        _targetSpeed = randomSpeed;


        float randomHeight = Random.Range(1f, 5f);
        _target.localScale = new Vector3(1f, randomHeight, 1f);

        
        _target.localPosition = new Vector3(0, randomHeight / 2f, 5f);

        Rigidbody targetRb = _target.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            targetRb.linearVelocity = Vector3.zero;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_target.localPosition - transform.localPosition);
        sensor.AddObservation(_target.localScale.y); 
        sensor.AddObservation(_isGrounded ? 1.0f : 0.0f); 
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float jumpAction = actions.ContinuousActions[0];

        float jumpMultiplier = Mathf.Clamp(_target.localScale.y / 2f, 1f, 3f);

        if (jumpAction > 0.5f && _isGrounded && Vector3.Distance(transform.position, _target.position) < 3.0f)
        {
            _rb.AddForce(Vector3.up * _jumpForce * jumpMultiplier, ForceMode.Impulse);
            _isGrounded = false;
        }


        _target.Translate(Vector3.back * _targetSpeed * Time.deltaTime);

        if (_isGrounded && transform.position.z > _target.position.z + 0.5f)
        {
            SetReward(1f);
            EndEpisode();
        }
        else
        {
            AddReward(-0.001f);
        }

        if (transform.position.y < -1f)
        {
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Target"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

       
        float distance = _target.position.z - transform.position.z;

        if (distance < 2.0f && distance > 0.1f && _isGrounded) 
        {
            continuousActions[0] = 1f;
        }
        else
        {
            continuousActions[0] = 0f;
        }
    }
}
