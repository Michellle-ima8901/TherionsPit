
using UnityEngine;
using KinematicCharacterController;
using Unity.Netcode;



public struct playerInput{
    public float moveAxisForward;
    public float moveAxisRight;
    public Quaternion cameraRotation ;
    
}

public class PlayerCharacterController:  NetworkBehaviour, ICharacterController
{

    [SerializeField]private KinematicCharacterMotor motor;
    private Vector3 moveInputVector ;
    private Vector3 lookInputVector ;
    private PlayerInputs currentInputs;

   
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField]private float stableMovementSharpness = 15f ;
    [SerializeField]private float orientationSharpness = 10f ;
    [SerializeField] private float jumpSpeed = 8f;
    [SerializeField] private Vector3 gravity = new Vector3(0f, -30f, 0f); 
    private bool jumpRequested;
    private bool isSprinting;
    



    public void Awake(){
        motor.CharacterController = this;
    }
    private void Start(){
        
    }

    public void setInputs(ref PlayerInputs inputs)
    {

        // Debug.Log($"setInputs | fwd={inputs.moveAxisForward} right={inputs.moveAxisRight} | camRot={(inputs.cameraRotation == default ? "NULL" : "OK")}");
        // Debug.Log($"setInputs: forward={inputs.moveAxisForward}, right={inputs.moveAxisRight}");
        isSprinting = inputs.sprintHeld;
        if (inputs.jumpPressed){
            jumpRequested = true;
        

        }

        currentInputs = inputs;

        // Compute move input in camera space
        Vector3 rawInput = new Vector3(inputs.moveAxisRight, 0f, inputs.moveAxisForward);
        rawInput = Vector3.ClampMagnitude(rawInput, 1f);

        Vector3 cameraForward = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.forward, motor.CharacterUp).normalized;
        if (cameraForward.sqrMagnitude == 0f)
            cameraForward = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.up, motor.CharacterUp);

        Quaternion cameraForwardRotation = Quaternion.LookRotation(cameraForward, motor.CharacterUp);

      
        moveInputVector = cameraForwardRotation * rawInput;
        lookInputVector = moveInputVector.normalized;
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        // This is called before the motor does anything
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
     
        if(lookInputVector.sqrMagnitude>0f &&  orientationSharpness > 0f){
            Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward,lookInputVector,1 - Mathf.Exp(-orientationSharpness*deltaTime)).normalized;

            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
        }

    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime){
        
        if (jumpRequested && motor.GroundingStatus.IsStableOnGround)
        {
            currentVelocity += motor.CharacterUp * jumpSpeed;
            motor.ForceUnground(0.1f);
        }
        jumpRequested = false;

        if (motor.GroundingStatus.IsStableOnGround)
        {
            float vertical = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (vertical < 0f)
            {
                currentVelocity -= motor.CharacterUp * vertical;  // strip downward only
            }
        }
        else
        {
            // In air  apply gravity
            currentVelocity += gravity * deltaTime;
        }

        
        Vector3 moveInputVector = new Vector3(currentInputs.moveAxisRight, 0f, currentInputs.moveAxisForward);
        moveInputVector = currentInputs.cameraRotation * moveInputVector;
        moveInputVector.y = 0f;

        if (moveInputVector.sqrMagnitude > 0.01f)
        {
            moveInputVector.Normalize();
        }


        Vector3 effectiveGroundNormal = motor.GroundingStatus.GroundNormal;
        Vector3 inputRight = Vector3.Cross(moveInputVector, motor.CharacterUp);
        Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * moveInputVector.magnitude;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 targetMovementVelocity = reorientedInput * currentSpeed;
        //Vector3 targetMovementVelocity = reorientedInput * maxStableMoveSpeed;

        Vector3 currentHorizontal = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
        Vector3 newHorizontal = Vector3.Lerp(
            currentHorizontal,
            targetMovementVelocity,
            1f - Mathf.Exp(-stableMovementSharpness * deltaTime)
        );
        float verticalComponent = Vector3.Dot(currentVelocity, motor.CharacterUp);
        currentVelocity = newHorizontal + motor.CharacterUp * verticalComponent;
       
    }


    public void AfterCharacterUpdate(float deltaTime)
    {
        // This is called after the motor has finished everything in its update
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        // This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        // This is called when the motor's ground probing detects a ground hit
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        // This is called when the motor's movement logic detects a hit
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        // This is called after every hit detected in the motor, to give you a chance to modify the HitStabilityReport any way you want
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        // This is called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
    }
    
}
 
