using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

  [SerializeField] private float defaultDistance= 6f;
  [SerializeField] private float minDistance = 3f ;
  [SerializeField] private float maxDistance = 10f ; 
  [SerializeField] private float distanceMovementSpeed = 5f;
  [SerializeField] private float distanceMovementSharpness = 10f ;
  [SerializeField] private float rotationSpeed = 10f ;
  [SerializeField] private float rotationSharpness = 10000f ;
  [SerializeField] private float followSharpness =  10000f ;
  [SerializeField] private float minVerticalAngle = - 90f ;
  [SerializeField] private float maxVerticalAngle =  90f ;
  [SerializeField] private float defaultVerticalAngle = 20f ;


   private Transform followTransform;
   private Vector3  currentFollowPosition,forwardDirection;
   private float targetVerticalAngle;

   private float currentDistance,targetDistance ;


   private void Awake (){
    currentDistance = defaultDistance ;
    targetDistance = currentDistance ;
    targetVerticalAngle = 0f ;  
    forwardDirection = Vector3.forward;
   }

   public void setFollowTransform(Transform t){
    followTransform = t  ;
    currentFollowPosition = t.position ;
    forwardDirection = t.forward ;

   }

   private void onValidate(){
    defaultDistance = Mathf.Clamp(defaultDistance , minDistance ,maxDistance);
    defaultVerticalAngle = Mathf.Clamp(defaultVerticalAngle , minVerticalAngle, maxVerticalAngle);

   }

   private void handleRotationInput(float deltaTime, Vector3 rotationInput,out Quaternion targetRotation){
    Quaternion rotationFromInput = Quaternion.Euler(followTransform.up * (rotationInput.x *rotationSpeed));
    forwardDirection  = rotationFromInput * forwardDirection ;
    Quaternion forwardRot = Quaternion.LookRotation(forwardDirection, followTransform.up);

    targetVerticalAngle -= (rotationInput.y * rotationSpeed);
    targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, minVerticalAngle,maxVerticalAngle);
    Quaternion verticalRot = Quaternion.Euler(targetVerticalAngle,0,0);

    targetRotation = Quaternion.Slerp(transform.rotation , forwardRot*verticalRot, rotationSharpness*deltaTime);

    transform.rotation = targetRotation;

   }

   private void handlePosition(float deltaTime , float zoomInput, Quaternion targetRotation){

    targetDistance += zoomInput * distanceMovementSpeed ;
    targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);

    currentFollowPosition = Vector3.Lerp(currentFollowPosition,followTransform.position,1f-Mathf.Exp(-followSharpness*deltaTime));
    Vector3 targetPosition =currentFollowPosition - ((targetRotation * Vector3.forward)* currentDistance);

    currentDistance = Mathf.Lerp(currentDistance,targetDistance,1-Mathf.Exp(-distanceMovementSharpness * deltaTime));
    transform.position = targetPosition;


   }

   public void  updateWithInput(float deltaTime , float zoomInput , Vector3 rotationInput){

    if(followTransform){
        handleRotationInput(deltaTime,rotationInput ,out Quaternion targetRotation );
        handlePosition(deltaTime,zoomInput,targetRotation);
         
    }

   }









}
