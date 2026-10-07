using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class SimplePlayerControllerSky : MonoBehaviour, IUpdraftable
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float sprintMultiplier = 1.7f;
    public float acceleration = 20f;
    public float deceleration = 35f;
    public float airControlMultiplier = 0.6f;

    [Header("Jump")]
    public float gravity = -30f;
    public float jumpHeight = 2.2f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2.5f;
    public AudioClip jumpSound;

    [Header("Footsteps")]
    public AudioClip rockStep;
    public AudioClip grassStep;
    public AudioClip woodStep;
    public float stepInterval = 0.5f;
    public float sprintStepInterval = 0.35f;

    [Header("Respawn")]
    public Transform respawnPoint;
    public GameObject respawnUI;
    public float uiDisplayTime = 3f;
    public float fallDeathY = -25f;

    [Header("Level Progression")]
    public string nextLevelName;

    public Transform cameraTransform;

    private CharacterController controller;
    private AudioSource audioSource;

    private Vector3 velocity;
    private Vector3 currentMoveVelocity;
    private Vector3 startPosition;

    private bool isGrounded;
    private float stepTimer;

    private float currentUpdraftSpeed = 0f;
    private float updraftLinger = 0f;
    public float updraftLingerDuration = 0.2f;

    Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        startPosition = transform.position;

        if (respawnUI != null)
            respawnUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Respawn();

        if (transform.position.y < fallDeathY)
            Respawn();

        HandleUpdraft();
        HandleMovement();
        HandleFootsteps();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0 && updraftLinger <= 0f)
            velocity.y = -2f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) ||
                           Input.GetKey(KeyCode.RightShift);

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * v + camRight * h;

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                15f * Time.deltaTime
            );
        }

        float baseSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        float controlMultiplier = isGrounded ? 1f : airControlMultiplier;

        Vector3 targetVelocity = move.normalized * baseSpeed * controlMultiplier;

        if (move.magnitude > 0.1f)
        {
            currentMoveVelocity = Vector3.Lerp(
                currentMoveVelocity,
                targetVelocity,
                acceleration * Time.deltaTime
            );
        }
        else
        {
            currentMoveVelocity = Vector3.Lerp(
                currentMoveVelocity,
                Vector3.zero,
                deceleration * Time.deltaTime
            );
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (jumpSound != null)
                audioSource.PlayOneShot(jumpSound);
        }

        if (velocity.y < 0)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else if (velocity.y > 0 && !Input.GetButton("Jump"))
            velocity.y += gravity * lowJumpMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        velocity.y = Mathf.Clamp(velocity.y, -50f, 10f);

        Vector3 finalMove = currentMoveVelocity + new Vector3(0, velocity.y, 0);
        controller.Move(finalMove * Time.deltaTime);
    }

    void HandleUpdraft()
    {
        if (updraftLinger <= 0f) return;

        updraftLinger -= Time.deltaTime;

        velocity.y += currentUpdraftSpeed * Time.deltaTime;
    }

    void HandleFootsteps()
    {
        if (!isGrounded || currentMoveVelocity.magnitude < 0.2f)
        {
            stepTimer = 0f;
            return;
        }

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) ||
                           Input.GetKey(KeyCode.RightShift);

        float interval = isSprinting ? sprintStepInterval : stepInterval;

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            PlayFootstepSound();
            stepTimer = interval;
        }
    }

    void PlayFootstepSound()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            if (hit.collider.CompareTag("Rock") && rockStep != null)
                audioSource.PlayOneShot(rockStep);
            else if (hit.collider.CompareTag("Grass") && grassStep != null)
                audioSource.PlayOneShot(grassStep);
            else if (hit.collider.CompareTag("Wood") && woodStep != null)
                audioSource.PlayOneShot(woodStep);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            Respawn();
            StartCoroutine(ShowRespawnUI());
        }

        if (other.CompareTag("Finish"))
        {
            LoadNextLevel();
        }
    }

    public void Respawn()
    {
        controller.enabled = false;

        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            transform.position = startPosition;

        velocity = Vector3.zero;
        currentMoveVelocity = Vector3.zero;

        controller.enabled = true;
    }

    IEnumerator ShowRespawnUI()
    {
        if (respawnUI == null)
            yield break;

        respawnUI.SetActive(true);
        yield return new WaitForSeconds(uiDisplayTime);
        respawnUI.SetActive(false);
    }

    void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
            SceneManager.LoadScene(nextLevelName);
        else
            Debug.LogWarning("Next level name not assigned.");
    }

    public void ApplyUpdraft(float speed)
    {
        if (velocity.y < 0f)
            velocity.y = 0f;

        currentUpdraftSpeed = speed;
        updraftLinger = updraftLingerDuration;
    }

    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
    }
}