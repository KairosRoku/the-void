// CHANGE LOG
// 
// CHANGES || version 1.0.1
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;

    #region Audio Variables
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepSounds;
    public float footstepInterval = 0.5f;
    private float footstepTimer;
    #endregion

    #region Camera Movement Variables

    public Camera playerCamera;
    public Transform Head;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    // Internal Variables
    private bool isWalking = false;

    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    // Internal Variables
    private CanvasGroup sprintBarCG;
    private bool isSprinting = false;
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion

    #region Jump

    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;

    // Internal Variables
    private bool isGrounded = false;

    #endregion

    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    // Internal Variables
    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        crosshairObject = GetComponentInChildren<Image>();

        // Set internal variables
        playerCamera.fieldOfView = fov;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }

        // Initialize AudioSource
        if (footstepAudioSource == null)
        {
            footstepAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else
        {
            crosshairObject.gameObject.SetActive(false);
        }

        #region Sprint Bar

        sprintBarCG = GetComponentInChildren<CanvasGroup>();

        if (useSprintBar)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            sprintBarWidth = screenWidth * sprintBarWidthPercent;
            sprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

            if (hideBarWhenFull)
            {
                sprintBarCG.alpha = 0;
            }
        }
        else
        {
            sprintBarBG.gameObject.SetActive(false);
            sprintBar.gameObject.SetActive(false);
        }

        #endregion
    }

    private void Update()
    {
        #region Camera

        // Control camera movement
        if (cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!invertCamera)
            {
                pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                // Inverted Y
                pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
            }

            // Clamp pitch between lookAngle
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        #region Camera Zoom

        if (enableZoom)
        {
            // Changes isZoomed when key is pressed
            // Behavior for toggle zoom
            if (Input.GetKeyDown(zoomKey) && !holdToZoom && !isSprinting)
            {
                isZoomed = !isZoomed;
            }

            // Changes isZoomed when key is pressed
            // Behavior for hold to zoom
            if (holdToZoom && !isSprinting)
            {
                if (Input.GetKeyDown(zoomKey))
                {
                    isZoomed = true;
                }
                else if (Input.GetKeyUp(zoomKey))
                {
                    isZoomed = false;
                }
            }

            // Lerps camera.fieldOfView to allow for a smooth transition
            if (isZoomed)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
            }
            else if (!isZoomed && !isSprinting)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
            }
        }

        #endregion
        #endregion

        #region Sprint

        if (enableSprint)
        {
            if (isSprinting)
            {
                isZoomed = false;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                // Drain sprint remaining while sprinting
                if (!unlimitedSprint)
                {
                    sprintRemaining -= 1 * Time.deltaTime;
                    if (sprintRemaining <= 0)
                    {
                        isSprinting = false;
                        isSprintCooldown = true;
                    }
                }
            }
            else
            {
                // Regain sprint while not sprinting
                sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
            }

            // Handles sprint cooldown 
            // When sprint remaining == 0, isSprintCooldown = true
            // After the cooldown is complete, isSprintCooldown = false and player can sprint again
            if (isSprintCooldown)
            {
                sprintCooldown -= 1 * Time.deltaTime;
                if (sprintCooldown <= 0)
                {
                    isSprintCooldown = false;
                    sprintCooldown = sprintCooldownReset;
                }
            }
        }

        #endregion

        #region Jump

        if (enableJump)
        {
            if (Input.GetKeyDown(jumpKey) && isGrounded)
            {
                Jump();
            }
        }

        #endregion

        #region Crouch

        if (enableCrouch)
        {
            if (Input.GetKeyDown(crouchKey) && !holdToCrouch)
            {
                isCrouched = !isCrouched;
                Crouch();
            }
            else if (Input.GetKeyDown(crouchKey) && holdToCrouch)
            {
                isCrouched = !isCrouched;
                Crouch();
            }

            if (holdToCrouch && isCrouched && Input.GetKeyUp(crouchKey))
            {
                isCrouched = !isCrouched;
                Crouch();
            }
        }

        #endregion

        if (enableHeadBob)
        {
            HeadBob();
        }

        PlayFootstepSounds();

        // Sprint Bar display
        if (useSprintBar && hideBarWhenFull && !unlimitedSprint)
        {
            if (sprintRemaining == sprintDuration && sprintBarCG.alpha != 0)
            {
                StartCoroutine(FadeSprintBar());
            }
            else if (sprintRemaining != sprintDuration && sprintBarCG.alpha != 1)
            {
                StartCoroutine(FadeSprintBar());
            }
        }
    }

    private void FixedUpdate()
    {
        #region Movement

        if (playerCanMove)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 targetVelocity = (transform.forward * z + transform.right * x) * (isSprinting ? sprintSpeed : walkSpeed);
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            if (enableSprint)
            {
                isWalking = x != 0 || z != 0;
                isSprinting = isWalking && Input.GetKey(sprintKey) && sprintRemaining > 0 && !isSprintCooldown;
            }
        }

        #endregion
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void Crouch()
    {
        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed *= speedReduction;
            if (enableSprint)
            {
                sprintSpeed *= speedReduction;
            }
        }
        else
        {
            transform.localScale = originalScale;
            walkSpeed /= speedReduction;
            if (enableSprint)
            {
                sprintSpeed /= speedReduction;
            }
        }
    }

    private void HeadBob()
    {
        if (Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.z) > 0.1f)
        {
            //Player is moving
            timer += Time.deltaTime * bobSpeed;
            joint.localPosition = jointOriginalPos + new Vector3(Mathf.Cos(timer) * bobAmount.x, Mathf.Sin(timer) * bobAmount.y, 0);
        }
        else
        {
            //Idle
            timer = 0;
            joint.localPosition = Vector3.Lerp(joint.localPosition, jointOriginalPos, Time.deltaTime * bobSpeed);
        }
    }

    private void PlayFootstepSounds()
    {
        Debug.Log("PlayFootstepSounds called"); // Debug log
        if (isWalking && footstepSounds.Length > 0)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                int randomIndex = Random.Range(0, footstepSounds.Length);
                footstepAudioSource.clip = footstepSounds[randomIndex];
                footstepAudioSource.Play();
                Debug.Log("Playing footstep sound: " + footstepSounds[randomIndex].name); // Debug log
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            Debug.Log("Not walking or no footstep sounds available"); // Debug log
        }
    }

    private IEnumerator FadeSprintBar()
    {
        float targetAlpha = sprintBarCG.alpha == 1 ? 0 : 1;
        while (!Mathf.Approximately(sprintBarCG.alpha, targetAlpha))
        {
            sprintBarCG.alpha = Mathf.MoveTowards(sprintBarCG.alpha, targetAlpha, 2 * Time.deltaTime);
            yield return null;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FirstPersonController))]
public class FirstPersonControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FirstPersonController script = (FirstPersonController)target;
        if (GUILayout.Button("Reset To Defaults"))
        {
            script.fov = 60f;
            script.invertCamera = false;
            script.cameraCanMove = true;
            script.mouseSensitivity = 2f;
            script.maxLookAngle = 50f;
            script.lockCursor = true;
            script.crosshair = true;
            script.crosshairImage = null;
            script.crosshairColor = Color.white;
            script.enableZoom = true;
            script.holdToZoom = false;
            script.zoomKey = KeyCode.Mouse1;
            script.zoomFOV = 30f;
            script.zoomStepTime = 5f;
            script.playerCanMove = true;
            script.walkSpeed = 5f;
            script.maxVelocityChange = 10f;
            script.enableSprint = true;
            script.unlimitedSprint = false;
            script.sprintKey = KeyCode.LeftShift;
            script.sprintSpeed = 7f;
            script.sprintDuration = 5f;
            script.sprintCooldown = .5f;
            script.sprintFOV = 80f;
            script.sprintFOVStepTime = 10f;
            script.useSprintBar = true;
            script.hideBarWhenFull = true;
            script.sprintBarBG = null;
            script.sprintBar = null;
            script.sprintBarWidthPercent = .3f;
            script.sprintBarHeightPercent = .015f;
            script.enableJump = true;
            script.jumpKey = KeyCode.Space;
            script.jumpPower = 5f;
            script.enableCrouch = true;
            script.holdToCrouch = true;
            script.crouchKey = KeyCode.LeftControl;
            script.crouchHeight = .75f;
            script.speedReduction = .5f;
            script.enableHeadBob = true;
            script.joint = null;
            script.bobSpeed = 10f;
            script.bobAmount = new Vector3(.15f, .05f, 0f);
        }

        DrawDefaultInspector();
    }
}
#endif
