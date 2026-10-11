using UnityEngine;
using KinematicCharacterController;
using Unity.Netcode;

public class MagePlayerAnimations : NetworkBehaviour
{
    private Animator animator;
    private KinematicCharacterMotor motor;

    void Start()
    {
        animator = GetComponent<Animator>();
        //motor = GetComponent<KinematicCharacterMotor>();
        motor = GetComponentInParent<KinematicCharacterMotor>();
    }
    

    // Update is called once per frame
    void Update()
    {
        if (animator == null || motor == null){
            return ;
        }

        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(motor.Velocity, Vector3.up);
        animator.SetFloat("Speed", horizontalVelocity.magnitude);
        animator.SetBool("IsGrounded", motor.GroundingStatus.IsStableOnGround);
    }

    public void OnJump(){
        animator.SetTrigger("Jump");
    }

   
}
