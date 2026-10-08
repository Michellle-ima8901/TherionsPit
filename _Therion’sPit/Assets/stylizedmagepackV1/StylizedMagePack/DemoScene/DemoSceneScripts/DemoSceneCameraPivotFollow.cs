using UnityEngine;

namespace StylizedMagePack
{
    public class CameraPivotFollow : MonoBehaviour
    {
        [SerializeField] private Transform playerTarget;  // drag the PLAYER here
        [SerializeField] private float followSpeed = 8f;  // smooth (between 5-15)


        private void LateUpdate()
        {
            // Follow Player 
            if (playerTarget != null)
            {
                transform.position = Vector3.Lerp(transform.position, playerTarget.position, followSpeed * Time.deltaTime);
            }
            else
            {
                Debug.Log("Missing PlayerTransform in CameraPivot");
            }
        }
    }
}
