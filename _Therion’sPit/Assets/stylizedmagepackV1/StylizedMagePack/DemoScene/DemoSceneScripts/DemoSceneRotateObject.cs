using UnityEngine;


namespace StylizedMagePack
{
    public class DemoSceneRotateObject : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 50, 0); // Degrees per second on X, Y, and Z
        [SerializeField] private bool useLocalAxis = true;                      // Rotate relative to its own orientation

        private void Update()
        {
            // Calculate the rotation amount for this frame
            // Multiply by Time.deltaTime to ensure smooth motion regardless of frame rate
            Vector3 rotationDelta = rotationSpeed * Time.deltaTime;

            if (useLocalAxis)
            {
                // Rotates the object based on its current local rotation
                transform.Rotate(rotationDelta, Space.Self);
            }
            else
            {
                // Rotates the object based on the world's coordinates
                transform.Rotate(rotationDelta, Space.World);
            }
        }
    }
}
