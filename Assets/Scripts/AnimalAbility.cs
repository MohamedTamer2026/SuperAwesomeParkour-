using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AnimalAbility : MonoBehaviour
{
    public enum AnimalType { Dog, Cat, Bird }
    public AnimalType type;

    [Header("UI")]
    public Slider healthSlider;
    private Image healthFillImage;
    public Color healthColor = Color.green;

    public Slider cooldownSlider;
    private Image fillImage;
    public Color loadingColor;
    private float flashTimer = 0;

    public Image iconDisplay;
    public Sprite dogIcon;
    public Sprite catIcon;
    public Sprite birdIcon;

    private Animator anim;
    private CharacterController controller;
    private SimplePlayerController playerController;

    public float maxHealth = 100f;
    public float currentHealth;

      [Header("Ability Cooldown")]
    // for ability cooldown
    public float cooldownTime = 0.18f;
    private float cooldownTimer = 0;

    [Header("Dog")]
    // for Dog's ability
    public GameObject barkPrefab;
    private Transform barkSpawnPoint;
    public AudioClip barkSound;
    private AudioSource audioSource;

    // for Cat's ability
    private float dashTimer = 0;


    // for Bird's ability
    private float birdFlapBoostTimer = 0;
    private float birdUpwardForce = 0;
    [Header("Bird")]
    public float gravity = -5f;
    public float initialFlapForce = 2f;
    private float currentFlapForce;
    public float glideGravityMultiplier = 0.35f;

    public int maxFlaps = 2;
    private int currentFlaps = 0;
    private bool isTired = false;

    private float invulnerabilityTimer = 0f;
    public float invulnerabilityDuration = 2f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponentInParent<CharacterController>();
        audioSource = GetComponentInParent<AudioSource>();
        playerController = GetComponentInParent<SimplePlayerController>();

        // Debug check for Animator
        if (anim == null)
        {
            Debug.LogError($"Animator not found on {gameObject.name} or its children! Trying GetComponent instead...");
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogError($"Still no Animator found on {gameObject.name}. Please add an Animator component!");
            }
        }
        else
        {
            Debug.Log($"Animator found on {anim.gameObject.name}");
        }

        currentHealth = maxHealth;
        currentFlapForce = initialFlapForce;

        if (healthSlider == null)
        {
            GameObject foundUI = GameObject.Find("HealthBar");
            if (foundUI != null)
            {
                healthSlider = foundUI.GetComponent<Slider>();
                healthSlider.value = currentHealth / maxHealth;
            }
        }

        if (cooldownSlider == null)
        {
            GameObject foundUI = GameObject.Find("CooldownBar");
            if (foundUI != null)
            {
                cooldownSlider = foundUI.GetComponent<Slider>();
            }
        }

        if (iconDisplay == null)
        {
            GameObject foundIcon = GameObject.Find("AnimalIcon");
            if (foundIcon != null)
            {
                iconDisplay = foundIcon.GetComponent<Image>();
            }
        }

        if (cooldownSlider != null && fillImage == null)
        {
            fillImage = cooldownSlider.fillRect.GetComponent<Image>();
        }

        if (iconDisplay != null)
        {
            if (type == AnimalType.Dog) iconDisplay.sprite = dogIcon;
            else if (type == AnimalType.Cat) iconDisplay.sprite = catIcon;
            else if (type == AnimalType.Bird) iconDisplay.sprite = birdIcon;
        }

        if (fillImage != null)
        {
            ColorUtility.TryParseHtmlString("#3F91C8", out loadingColor);
            fillImage.color = loadingColor;
        }

        if (type == AnimalType.Dog)
        {
            GameObject mouth = transform.Find("Mouth")?.gameObject;

            if (mouth != null)
            {
                barkSpawnPoint = mouth.transform;
            }
            else
            {
                barkSpawnPoint = this.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float previousTimer = cooldownTimer;

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }

        if (previousTimer > 0 && cooldownTimer <= 0)
        {
            flashTimer = 0.2f;
        }

        if (cooldownSlider != null)
        {
            float progress = 1f - (cooldownTimer / cooldownTime);
            cooldownSlider.value = Mathf.Clamp01(progress);

            if (fillImage != null)
            {
                if (flashTimer > 0)
                {
                    fillImage.color = Color.white;
                    flashTimer -= Time.deltaTime;
                }
                else
                {
                    fillImage.color = loadingColor;
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && cooldownTimer <= 0)
        {
            ExecuteAbility();
        }

        HandlePhysics();
    }

    void HandlePhysics()
    {
        if (type == AnimalType.Cat && dashTimer > 0)
        {
            controller.Move(transform.forward * 20f * Time.deltaTime);
            dashTimer -= Time.deltaTime;
        }
        else if (type == AnimalType.Bird && birdFlapBoostTimer > 0)
        {
            // Apply upward boost that gradually diminishes
            controller.Move(Vector3.up * birdUpwardForce * Time.deltaTime);
            birdFlapBoostTimer -= Time.deltaTime;
            birdUpwardForce *= 0.99f; // Gradually reduce the boost for smoother glide
            
            // Update animator with grounded state
            if (anim != null && controller != null)
            {
                anim.SetBool("isGrounded", controller.isGrounded);
            }
            
            // Reset flaps when grounded
            if (controller != null && controller.isGrounded)
            {
                currentFlaps = 0;
                isTired = false;
                currentFlapForce = initialFlapForce;
                birdFlapBoostTimer = 0;
            }
        }
        else if (type == AnimalType.Bird)
        {
            // Update animator with grounded state when not actively flapping
            if (anim != null && controller != null)
            {
                anim.SetBool("isGrounded", controller.isGrounded);
            }
            
            // Reset flaps when grounded
            if (controller != null && controller.isGrounded)
            {
                currentFlaps = 0;
                isTired = false;
                currentFlapForce = initialFlapForce;
            }
        }
    }
    void ExecuteAbility()
    {
        if (type == AnimalType.Dog)
        {
            cooldownTime = 1.0f;
            cooldownTimer = cooldownTime;
            if (anim != null) anim.SetTrigger("BarkTrigger");

            if (audioSource != null && barkSound != null)
            {
                audioSource.PlayOneShot(barkSound);
            }

            if (barkPrefab != null && barkSpawnPoint != null && type == AnimalType.Dog)
            {
                Debug.Log("Dog is barking!");
                Instantiate(barkPrefab, barkSpawnPoint.position, barkSpawnPoint.rotation);
            }
        }
        else if (type == AnimalType.Cat)
        {
            Debug.Log("Cat is dashing!");
            
            cooldownTime = 1.0f;
            cooldownTimer = cooldownTime;
            if (anim != null) anim.SetTrigger("DashTrigger");
            dashTimer = 0.2f;
        }
        else if (type == AnimalType.Bird)
        {
            if (isTired) return;

            cooldownTime = 0.5f;
            cooldownTimer = cooldownTime;
            if (anim != null) anim.SetTrigger("FlyTrigger");

            currentFlapForce *= 0.3f;
            
            // Calculate flap boost with level modifier
            float flapBoost = currentFlapForce;
            if (SceneManager.GetActiveScene().buildIndex != 4)
            {
                flapBoost *= 1.3f; // increase height on all levels except level 4
            }

            // Apply upward force for a brief duration
            birdUpwardForce = flapBoost * 6f; // Higher multiplier for more height
            birdFlapBoostTimer = 0.5f; // Apply boost for longer glide duration

            currentFlaps++;

            if (currentFlaps >= maxFlaps) { isTired = true; }
        }
    }
    public void PlayerTakeDamage(float amount)
    {
        if (invulnerabilityTimer > 0)
            return; // Don't take damage if invulnerable

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        SimplePlayerController playerController = GetComponentInParent<SimplePlayerController>();

        if (playerController != null)
        {
            playerController.Respawn();
        }

        currentHealth = maxHealth;
        invulnerabilityTimer = invulnerabilityDuration;
    }
}
