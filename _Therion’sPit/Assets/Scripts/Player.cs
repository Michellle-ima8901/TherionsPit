using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour{

    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private Transform CameraFollowPoint;
    [SerializeField] private CharacterController characterController;

    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference scrollAction;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    
    private Vector3 lookInputVector; 



    private void OnEnable()
    {
        lookAction.action.Enable();
        scrollAction.action.Enable();
        jumpAction.action.Enable();  
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
        scrollAction.action.Disable();
        jumpAction.action.Disable(); 
    }


    private void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera.setFollowTransform(CameraFollowPoint); 
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
        inputs.jumpPressed     = jumpAction.action.WasPressedThisFrame();

        characterController.setInputs(ref inputs);
    }

    
    private void LateUpdate(){
        handleCameraInput();
        handleCharacterInputs();
    }

      



      

}


























