using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour{

    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private Transform CameraFollowPoint;
    [SerializeField] private MyCharacterController characterController;

    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference scrollAction;

    private Vector3 lookInputVector; 



    private void OnEnable()
    {
        lookAction.action.Enable();
        scrollAction.action.Enable(); 
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
        scrollAction.action.Disable();
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

    // private void handleCameraInput(){
    //     float mouseUp = Input.GetAxisRaw("Mouse Y");
    //     float mouseRight = Input.GetAxisRaw("Mouse X");


    //     lookInputVector = new Vector3(mouseRight , mouseUp , 0f);

    //     float scrollInput = -Input.GetAxis("Mouse ScrollWheel");
    //     playerCamera.updateWithInput(Time.deltaTime,scrollInput,lookInputVector);


    // }

    private void LateUpdate(){
        handleCameraInput();
    }

      



      

}


























