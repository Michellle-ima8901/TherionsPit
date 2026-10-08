using UnityEngine;
using KinematicCharacterController;

public class MagePlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private KinematicCharacterMotor motor;

    void Start()
    {
        animator = GetComponent<Animator>();
        motor = GetComponent<KinematicCharacterMotor>();
    }
    

    // Update is called once per frame
    void Update()
    {
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(motor.Velocity, Vector3.up);
        animator.SetFloat("Speed", horizontalVelocity.magnitude);
    }

    public void OnJump(){
        animator.SetTrigger("Jump");
    }


}
