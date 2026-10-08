using UnityEngine;

namespace StylizedMagePack
{

    public class SimpleLaserHitVFX : MonoBehaviour
    {
        public float duration = 0.6f;          // Total visible time
        public float maxDistance = 100f;
        public float damageInterval = 0.5f;    // How often damage is applied (in seconds)

        private LineRenderer lr;
        private float lastDamageTime = 0f;     // Timer for periodic damage

        void Awake()
        {
            lr = GetComponentInChildren<LineRenderer>();
        }

        void Start()
        {
            // Destroy the object after the duration
            Destroy(gameObject, duration);
        }

        void Update()
        {
            if (lr == null) return;

            // Raycast updated every frame (to follow player movement while parented)
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, hit.point);
                lr.enabled = true;

                //---------------------------------------------------
                // This part is only for demoScene, can be deleted
                // Check if we hit the Dummy and apply damage every damageInterval seconds
                if (hit.collider.CompareTag("Dummy") && Time.time - lastDamageTime >= damageInterval)
                {
                    DemoSceneDummy dummy = hit.collider.GetComponent<DemoSceneDummy>();
                    if (dummy != null)
                    {
                        dummy.TakeHit();
                        lastDamageTime = Time.time;
                        Debug.Log("Laser hit dummy!");
                    }
                }
                // -------------------------------------------------
            }
            else
            {
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, transform.position + transform.forward * maxDistance);
                lr.enabled = true;
            }
        }
    }
}