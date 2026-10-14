using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Player : NetworkBehaviour{

    
    private PlayerCamera playerCamera;
    [SerializeField] private Transform CameraFollowPoint;
    [SerializeField] private PlayerCharacterController characterController;
    [SerializeField] private MagePlayerAnimations magePlayerAnimations;

    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference scrollAction;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;
    
    private Vector3 lookInputVector; 



    private void OnEnable()
    {
        lookAction.action.Enable();
        scrollAction.action.Enable();
        jumpAction.action.Enable(); 
        sprintAction.action.Enable();
    }
    

    private void OnDisable()
    {
        lookAction.action.Disable();
        scrollAction.action.Disable();
        jumpAction.action.Disable(); 
        sprintAction.action.Disable();
    }


    


    private void handleCameraInput()
    {
        Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();
         
        float mouseUp    = mouseDelta.y;
        float mouseRight = mouseDelta.x;

        lookInputVector = new Vector3(mouseRight, mouseUp, 0f);

        float scrollInput = -scrollAction.action.ReadValue<float>();
        playerCamera.updateWithInput(Time.deltaTime, scrollInput, lookInputVector);
    }

    

    public void handleCharacterInputs(){
      

        Debug.Log("handleCharacterInputs called");
        PlayerInputs inputs = new PlayerInputs();

        Vector2 move = moveAction.action.ReadValue<Vector2>();
        inputs.moveAxisRight   = move.x;   // A/D
        inputs.moveAxisForward = move.y;   // W/S

        inputs.cameraRotation = playerCamera.transform.rotation;
        if (jumpAction.action.WasPressedThisFrame()){
            inputs.jumpPressed = true;
            if (magePlayerAnimations != null)
            {
                magePlayerAnimations.OnJump();
            }
            

        }
        inputs.sprintHeld = sprintAction.action.IsPressed();           
       

        characterController.setInputs(ref inputs);
    }

    
    private void LateUpdate(){
        if (!IsOwner){ 
            return;
        }

        handleCameraInput();
        handleCharacterInputs();
    }

    

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        Cursor.lockState = CursorLockMode.Locked;

        // Find the scene's camera
        playerCamera = Camera.main.GetComponent<PlayerCamera>();

        if (playerCamera != null)
        {
            playerCamera.setFollowTransform(CameraFollowPoint);
        }
        else
        {
            Debug.LogError("Main Camera doesn't have a PlayerCamera script!");
        }

        // Also find the mage animations on a child
        magePlayerAnimations = GetComponentInChildren<MagePlayerAnimations>();
    }

      



      

}


























