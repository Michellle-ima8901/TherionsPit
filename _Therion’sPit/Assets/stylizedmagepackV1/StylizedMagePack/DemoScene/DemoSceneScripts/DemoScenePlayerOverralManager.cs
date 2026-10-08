using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace StylizedMagePack
{
    public class DemoScenePlayerOverralManager : MonoBehaviour
    {
        // Core components
        private CharacterController characterController;
        private Animator animator;

        [Header("Player Movement")]
        private Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        private float moveAmount;

        [Header("Camera Movement")]
        private Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] private float walkingSpeed = 2f;
        [SerializeField] private float runningSpeed = 5f;
        [SerializeField] private float rotationSpeed = 15f;

        [Header("Cameras")]
        public Camera isometricCamera;
        public Camera thirdPersonCamera;
        public bool isIsometricActive = true;           // true = isometric, false = third person

        [Header("Third Person Camera Settings")]
        [SerializeField] private float thirdPersonDistance = 6f;
        [SerializeField] private float lookHeight = 1.7f;           // Height to look at (usually head height)
        [SerializeField] private float camPitchClampMin = -35f;
        [SerializeField] private float camPitchClampMax = 60f;
        [SerializeField] private float camSensitivity = 2f;

        private float currentCamPitch = 20f;
        private float currentCamYaw = 0f;

        [Header("Animation Flags")]
        public bool sprintInput = false;
        public bool isSprinting = false;

        [Header("Ground & Jump")]
        [SerializeField] private float gravityForce = -9.81f;
        [SerializeField] private float jumpHeight = 4f;
        [SerializeField] private float groundedDownForce = -3.0f;
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] private float jumpCooldown = 0.25f;

        private Vector3 yVelocity;
        private float coyoteTimeCounter;
        private float jumpCooldownTimer;
        public bool isGrounded;
        private bool jumpPressedThisFrame;

        [Header("Attack Prefabs")]
        [SerializeField] private GameObject[] lightAttackPrefabs = new GameObject[4];
        [SerializeField] private GameObject[] heavyAttackPrefabs = new GameObject[4];
        [SerializeField] private GameObject[] aoePrefabs = new GameObject[3];

        [Header("Attack Timings")]
        [SerializeField] private float attackDelay = 0.7f;              // Cooldown between attacks
        [SerializeField] private float lightSpawnDelay = 0.6f;
        [SerializeField] private float heavySpawnDelay = 0.6f;
        [SerializeField] private float aoeSpawnDelay = 0.6f;
        [SerializeField] private float heavyFollowDuration = 2f;        // How long heavy (laser) follows player

        private float lastAttackTime = -Mathf.Infinity;
        public int selectedLightIndex = 0;
        public int selectedHeavyIndex = 0;

        [Header("Spawn Positions")]
        [SerializeField] private Transform projectileSpawnPoint;

        [Header("Lock-On System")]
        [SerializeField] private Transform lockOnTarget;
        [SerializeField] private float strafeSpeed = 4f;
        public bool isLockedOn = false;

        private void Start()
        {
            // Get required components
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            // Setup cameras - start with isometric active
            if (isometricCamera == null) Debug.LogWarning("Isometric Camera not found!");
            else isometricCamera.gameObject.SetActive(true);

            if (thirdPersonCamera == null) Debug.LogWarning("Third Person Camera not found!");
            else thirdPersonCamera.gameObject.SetActive(false);

            // Fallback spawn point
            if (projectileSpawnPoint == null) projectileSpawnPoint = transform;

            // Initialize third person yaw to match player facing
            currentCamYaw = transform.eulerAngles.y;
        }

        private void Update()
        {
            // Read all inputs first
            CaptureInputs();

            // Handle camera toggle (C key)
            HandleCameraSwitch();

            // Lock-on toggle (F key)
            HandleLockOnToggle();

            // Spell selection (1-8 keys)
            HandleSelectionInputs();

            // Movement logic depending on lock-on state
            if (isLockedOn && lockOnTarget != null)
            {
                HandleLockOnMovementLogic();        // Strafe around target
            }
            else
            {
                handlePlayerMovementInput();
                HandleRotation();                   // Rotate to face movement direction
            }

            // Camera mouse input
            HandleCameraMovementInput();

            // Sprint check
            HandleSprinting();

            // Attack inputs and logic
            HandleAttacks();

            // Apply final movement + gravity
            CalculateMoveDirectionAndApplyMovement();

            // Ground check + gravity
            UpdateGroundAndGravity();

            // Jump logic
            HandleJumping();
        }

        // Smooth camera follow - only for third person mode
        private void LateUpdate()
        {
            if (!isIsometricActive && thirdPersonCamera != null)
            {
                UpdateThirdPersonCamera();
            }
        }

        // Toggle between isometric and third person camera with C key
        private void HandleCameraSwitch()
        {
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                isIsometricActive = !isIsometricActive;
                isometricCamera.gameObject.SetActive(isIsometricActive);
                thirdPersonCamera.gameObject.SetActive(!isIsometricActive);

                if (!isIsometricActive)
                {
                    // Reset third person camera behind player on switch
                    currentCamYaw = transform.eulerAngles.y;
                    currentCamPitch = 20f;
                    Debug.Log("Switched to Third Person!");
                }
                else
                {
                    Debug.Log("Switched to Isometric!");
                }
            }
        }

        // Basic orbit camera for third person (mouse controls yaw/pitch)
        private void UpdateThirdPersonCamera()
        {
            // Update yaw and pitch from mouse delta
            currentCamYaw += cameraHorizontalInput * camSensitivity;
            currentCamPitch -= cameraVerticalInput * camSensitivity;

            // Clamp vertical angle to avoid flipping
            currentCamPitch = Mathf.Clamp(currentCamPitch, camPitchClampMin, camPitchClampMax);

            // Calculate rotation and position offset
            Quaternion rotation = Quaternion.Euler(currentCamPitch, currentCamYaw, 0f);
            Vector3 offset = rotation * new Vector3(0f, 0f, -thirdPersonDistance);

            Vector3 targetPos = transform.position + Vector3.up * lookHeight;
            thirdPersonCamera.transform.position = targetPos + offset;
            thirdPersonCamera.transform.LookAt(targetPos);
        }

        // Toggle lock-on mode with F key
        private void HandleLockOnToggle()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                isLockedOn = !isLockedOn && lockOnTarget != null;
                animator.SetBool("LockOn", isLockedOn);
            }
        }

        // Strafe movement when locked on (faces target, moves sideways/forward relative)
        private void HandleLockOnMovementLogic()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            // Face the lock-on target
            Vector3 dirToTarget = (lockOnTarget.position - transform.position);
            dirToTarget.y = 0;
            if (dirToTarget.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            // Calculate strafe directions relative to target
            Vector3 forward = dirToTarget.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            moveDirection = (forward * verticalInput + right * horizontalInput).normalized * strafeSpeed;

            // Set animator strafe parameters
            animator.SetFloat("StrafeHorizontal", horizontalInput, 0.1f, Time.deltaTime);
            animator.SetFloat("StrafeVertical", verticalInput, 0.1f, Time.deltaTime);
        }

        // Capture WASD, mouse, sprint and jump inputs (raw keyboard/mouse)
        private void CaptureInputs()
        {
            float x = 0f;
            float y = 0f;

            if (Keyboard.current.wKey.isPressed) y += 1;
            if (Keyboard.current.sKey.isPressed) y -= 1;
            if (Keyboard.current.dKey.isPressed) x += 1;
            if (Keyboard.current.aKey.isPressed) x -= 1;

            movementInput = new Vector2(x, y).normalized;

            if (Mouse.current != null)
            {
                cameraInput = Mouse.current.delta.ReadValue();
                cameraInput *= 0.1f;
            }

            sprintInput = Keyboard.current.leftShiftKey.isPressed;
            jumpPressedThisFrame = Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        // Select spells with number keys (1-4 light, 5-8 heavy)
        private void HandleSelectionInputs()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) selectedLightIndex = 0;
            if (Keyboard.current.digit2Key.wasPressedThisFrame) selectedLightIndex = 1;
            if (Keyboard.current.digit3Key.wasPressedThisFrame) selectedLightIndex = 2;
            if (Keyboard.current.digit4Key.wasPressedThisFrame) selectedLightIndex = 3;

            if (Keyboard.current.digit5Key.wasPressedThisFrame) selectedHeavyIndex = 0;
            if (Keyboard.current.digit6Key.wasPressedThisFrame) selectedHeavyIndex = 1;
            if (Keyboard.current.digit7Key.wasPressedThisFrame) selectedHeavyIndex = 2;
            if (Keyboard.current.digit8Key.wasPressedThisFrame) selectedHeavyIndex = 3;
        }

        // Handle all attack inputs (light, heavy, AoE)
        private void HandleAttacks()
        {
            if (Time.time - lastAttackTime < attackDelay) return;

            // Light attack (right mouse)
            if (Mouse.current.rightButton.wasPressedThisFrame && lightAttackPrefabs[selectedLightIndex] != null)
            {
                animator.SetTrigger("SpellLight");
                StartCoroutine(SpawnLightAfterDelay(selectedLightIndex));
                lastAttackTime = Time.time;
            }

            // Heavy attack (left mouse)
            if (Mouse.current.leftButton.wasPressedThisFrame && heavyAttackPrefabs[selectedHeavyIndex] != null)
            {
                animator.SetTrigger("SpellHeavy");
                StartCoroutine(SpawnHeavyAfterDelay(selectedHeavyIndex));
                lastAttackTime = Time.time;
            }

            // AoE spells (H, J, K keys)
            int aoeIndex = -1;
            if (Keyboard.current.hKey.wasPressedThisFrame) aoeIndex = 0;
            else if (Keyboard.current.jKey.wasPressedThisFrame) aoeIndex = 1;
            else if (Keyboard.current.kKey.wasPressedThisFrame) aoeIndex = 2;

            if (aoeIndex >= 0 && aoePrefabs[aoeIndex] != null)
            {
                // Disable lock-on and strafe layer to prevent animation conflict
                isLockedOn = false;
                animator.SetBool("LockOn", false);
                animator.SetLayerWeight(4, 0f);

                animator.SetTrigger("SpellCasting");

                StartCoroutine(ResetLayerWeightAfterAttack(4, 1.5f));

                StartCoroutine(SpawnAoEAfterDelay(aoeIndex));
                lastAttackTime = Time.time;
            }
        }

        // Spawns light projectile after animation delay
        private IEnumerator SpawnLightAfterDelay(int index)
        {
            yield return new WaitForSeconds(lightSpawnDelay);
            Instantiate(lightAttackPrefabs[index], projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        }

        // Spawns heavy (laser) that follows player for a duration
        private IEnumerator SpawnHeavyAfterDelay(int index)
        {
            yield return new WaitForSeconds(heavySpawnDelay);
            GameObject laser = Instantiate(heavyAttackPrefabs[index], projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            laser.transform.SetParent(transform);
            yield return new WaitForSeconds(heavyFollowDuration);
            if (laser != null) laser.transform.SetParent(null);
        }

        // Spawns AoE effect at player position
        private IEnumerator SpawnAoEAfterDelay(int index)
        {
            yield return new WaitForSeconds(aoeSpawnDelay);
            Vector3 pos = transform.position;
            Instantiate(aoePrefabs[index], pos, Quaternion.identity);
        }

        // Calculates final move direction (isometric or third-person relative) and applies movement
        private void CalculateMoveDirectionAndApplyMovement()
        {
            float currentSpeed = isSprinting ? runningSpeed : walkingSpeed;

            if (!isLockedOn)
            {
                if (isIsometricActive)
                {
                    // Isometric: world-space movement
                    moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized * currentSpeed;
                }
                else if (thirdPersonCamera != null)
                {
                    // Third person: movement relative to camera facing direction
                    Vector3 camForward = thirdPersonCamera.transform.forward;
                    camForward.y = 0f; camForward.Normalize();
                    Vector3 camRight = thirdPersonCamera.transform.right;
                    camRight.y = 0f; camRight.Normalize();
                    moveDirection = (camForward * verticalInput + camRight * horizontalInput).normalized * currentSpeed;
                }
            }

            Vector3 finalMove = moveDirection + yVelocity;
            characterController.Move(finalMove * Time.deltaTime);
        }

        // Check if grounded, apply gravity, reset coyote time
        private void UpdateGroundAndGravity()
        {
            isGrounded = characterController.isGrounded;
            if (isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
                yVelocity.y = groundedDownForce;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
                yVelocity.y += gravityForce * Time.deltaTime;
            }

            if (jumpCooldownTimer > 0f) jumpCooldownTimer -= Time.deltaTime;
            animator.SetBool("isGrounded", isGrounded);
        }

        // Convert raw input to animator-friendly values
        private void handlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;
            float inputMagnitude = movementInput.magnitude;

            if (inputMagnitude > 0.01f)
            {
                moveAmount = isSprinting ? 1f : 0.5f;
                UpdateAnimatorMovementParameters(moveAmount);
            }
            else
            {
                moveAmount = 0f;
                UpdateAnimatorMovementParameters(0f);
            }
        }

        // Store mouse delta for camera control
        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }

        // Rotate player to face movement direction (adapts to camera mode)
        private void HandleRotation()
        {
            targetRotationDirection = Vector3.zero;
            float inputMag = movementInput.magnitude;

            if (inputMag > 0.01f)
            {
                if (isIsometricActive)
                {
                    targetRotationDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
                }
                else if (thirdPersonCamera != null)
                {
                    Vector3 camForward = thirdPersonCamera.transform.forward;
                    camForward.y = 0f; camForward.Normalize();
                    Vector3 camRight = thirdPersonCamera.transform.right;
                    camRight.y = 0f; camRight.Normalize();
                    targetRotationDirection = (camForward * verticalInput + camRight * horizontalInput).normalized;
                }
            }
            else
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(targetRotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Simple sprint flag
        private void HandleSprinting()
        {
            isSprinting = sprintInput;
        }

        // Jump with coyote time and cooldown
        private void HandleJumping()
        {
            if (jumpCooldownTimer > 0f) return;

            if (jumpPressedThisFrame && coyoteTimeCounter > 0f)
            {
                animator.CrossFade("m_jump_enter", 0.2f);
                yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
                coyoteTimeCounter = 0f;
                jumpCooldownTimer = jumpCooldown;
            }
        }

        // Update animator vertical speed parameter smoothly
        public void UpdateAnimatorMovementParameters(float speed)
        {
            animator.SetFloat("Vertical", speed, 0.1f, Time.deltaTime);
        }

        // Restore layer weight after AoE animation (prevents leg animation issues)
        private IEnumerator ResetLayerWeightAfterAttack(int layerIndex, float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.SetLayerWeight(layerIndex, 1f);
        }
    }
}