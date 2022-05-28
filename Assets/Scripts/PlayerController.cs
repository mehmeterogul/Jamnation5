using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Animator animator;
    Rigidbody rb;

    [Header("Speed Values")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;

    [Header("Movement Vector Values")]
    Vector3 lastMousePosition;
    Vector3 delta;
    Vector3 normalizedDelta;

    bool canWalk = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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

            if (normalizedDelta.x > 0.01f || normalizedDelta.x < -0.01f || normalizedDelta.z > 0.01f || normalizedDelta.z < -0.01f)
            {
                animator.SetBool("isMoving", true);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            SetMovementVectorsZero();

            animator.SetBool("isMoving", false);
            
            rb.AddForce(transform.forward * 25f, ForceMode.Impulse);
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
}
