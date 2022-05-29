using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Transform ballTransform;

    bool canWalk = true;
    bool gameOver = false;

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
    [SerializeField] ParticleSystem hitParticle;

    [Header("Sound Components")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip goalSound;
    [SerializeField] AudioClip collideSound;
    [SerializeField] AudioClip pickUpSound;
    [SerializeField] AudioClip shootSound;

    [Header("UI Components")]
    [SerializeField] TextMeshProUGUI goalText;
    [SerializeField] TextMeshProUGUI failText;

    bool isCorouitineActive = false;

    float timeCounter = 0f;
    bool hasShooted = false;

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
        if (hasShooted) timeCounter += Time.deltaTime;

        if (timeCounter > 5f) GameOver();

        if (transform.position.y < -2f) GameOver();

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
            audioSource.PlayOneShot(shootSound, 1f);
            canWalk = false;
            hasShooted = true;
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

                if (isCorouitineActive)
                {
                    StopAllCoroutines();
                    isCorouitineActive = false;
                }

                StartCoroutine(DeactiveSpeedEffect());
            }
            else if (type == PowerUpType.SpeedDown)
            {
                moveSpeed = moveSpeedAtStart - 5;

                if (isCorouitineActive)
                {
                    StopAllCoroutines();
                    isCorouitineActive = false;
                }

                StartCoroutine(DeactiveSpeedEffect());
            }
            else if (type == PowerUpType.ExtraShoot)
            {
                //
            }

            audioSource.PlayOneShot(pickUpSound, 1f);

            Destroy(other.gameObject);
        }

        if(other.gameObject.CompareTag("GoalTrigger"))
        {
            Destroy(other.gameObject);
            LevelSuccess();
        }
    }

    IEnumerator DeactiveSpeedEffect()
    {
        isCorouitineActive = true;
        yield return new WaitForSeconds(powerUpEffectTime);
        moveSpeed = moveSpeedAtStart;
        isCorouitineActive = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            canWalk = false;

            Vector3 pos = collision.transform.position - transform.position;
            rb.AddForce(pos * 5f, ForceMode.Impulse);
            hitParticle.gameObject.SetActive(true);

            if(!gameOver)
            {
                audioSource.PlayOneShot(hitSound, 1f);
            }

            GameOver();
        }
    }

    void GameOver()
    {
        if (gameOver) return;

        Invoke("FailTextAnimation", 0.05f);
        gameOver = true;

        FindObjectOfType<GameSceneManager>().LoadCurrentLevel();
    }

    private void FailTextAnimation()
    {
        failText.gameObject.SetActive(true);
    }

    void LevelSuccess()
    {
        if(gameOver) return;

        fireworkParticle.Play();
        audioSource.PlayOneShot(goalSound, 1f);
        Invoke("GoalTextAnimation", 0.1f);

        gameOver = true;

        FindObjectOfType<GameSceneManager>().LoadNextLevel();
    }

    private void GoalTextAnimation()
    {
        goalText.gameObject.SetActive(true);
        goalText.transform.DORewind();
        goalText.transform.DOPunchScale(new Vector3(1f, 1f, 1f), 0.5f);
    }
}
