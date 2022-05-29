using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Transform ballTransform;

    bool canWalk = true;

    [Header("Speed Values")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float ballRotateSpeed;
    float moveSpeedAtStart;

    [Header("Movement Vector Values")]
    Vector3 lastMousePosition;
    Vector3 delta;
    Vector3 normalizedDelta;

    [Header("Other Components")]
    [SerializeField] float shootForce = 50f;
    [SerializeField] float powerUpEffectTime = 1f;

    [Header("Particle Effects")]
    [SerializeField] ParticleSystem fireworkParticle;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveSpeedAtStart = moveSpeed;
    }

    void FixedUpdate()
    {
        if (!canWalk) return;

        Move();
    }

    void Move()
    {
        // if normalized delta axis values bigger than 0.01f, then move
        if (normalizedDelta.x > 0.01f || normalizedDelta.x < -0.01f || normalizedDelta.z > 0.01f || normalizedDelta.z < -0.01f)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(normalizedDelta, Vector3.up),
                rotationSpeed * Time.deltaTime
                );

            ballTransform.Rotate(Vector3.right * ballRotateSpeed * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canWalk) return;

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            SetDeltaVariables();
        }

        if (Input.GetMouseButtonUp(0))
        {
            SetMovementVectorsZero();
            
            rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
            canWalk = false;
        }
    }

    private void SetMovementVectorsZero()
    {
        delta = Vector3.zero;
        normalizedDelta = Vector3.zero;
    }

    void SetDeltaVariables()
    {
        delta = Input.mousePosition - lastMousePosition;

        normalizedDelta = delta.normalized;
        normalizedDelta.z = normalizedDelta.y;
        normalizedDelta.y = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PowerUp"))
        {
            PowerUpType type = other.GetComponent<PowerUp>().GetPowerUpType();

            if(type == PowerUpType.SpeedUp)
            {
                moveSpeed = moveSpeedAtStart + 5;
                StartCoroutine(DeactiveSpeedEffect());
            }
            else if (type == PowerUpType.SpeedDown)
            {
                moveSpeed = moveSpeedAtStart - 5;
                StartCoroutine(DeactiveSpeedEffect());
            }
            else if (type == PowerUpType.ExtraShoot)
            {
                //
            }

            Destroy(other.gameObject);
        }

        if(other.gameObject.CompareTag("GoalTrigger"))
        {
            //
            Debug.Log("GOAL!!!!!!");
            LevelSuccess();
        }
    }

    IEnumerator DeactiveSpeedEffect()
    {
        yield return new WaitForSeconds(powerUpEffectTime);
        moveSpeed = moveSpeedAtStart;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            canWalk = false;

            Vector3 pos = collision.transform.position - transform.position;
            rb.AddForce(pos * 5f, ForceMode.Impulse);

            GameOver();
        }
    }

    void GameOver()
    {

    }

    void LevelSuccess()
    {
        fireworkParticle.Play();
    }
}
