using UnityEngine;
using Unity.Netcode;
using KinematicCharacterController;

public class NinjaPlayerAnimations : NetworkBehaviour, IPlayerAnimations
{
    private Animator animator;
    private KinematicCharacterMotor motor;

    void Start()
    {
        animator = GetComponent<Animator>();
        motor = GetComponentInParent<KinematicCharacterMotor>();
    }

    void Update()
    {
        
        if (!IsOwner) return; 

        if (animator == null || motor == null) return;

        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(motor.Velocity, Vector3.up);
        animator.SetFloat("Speed", horizontalVelocity.magnitude);
        animator.SetBool("IsGrounded", motor.GroundingStatus.IsStableOnGround);
    }

    public void OnJump(){
        animator.SetTrigger("Jump");
    }
}