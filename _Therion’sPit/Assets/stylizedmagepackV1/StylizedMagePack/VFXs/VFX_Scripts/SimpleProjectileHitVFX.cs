using UnityEngine;
namespace StylizedMagePack
{
    public class SimpleProjectileHitVFX : MonoBehaviour
    {
        public GameObject impactPrefab;     // Prefab to spawn on impact (VFX, particles, decal, etc.)
        public float lifetime = 5f;         // How long the projectile lives before auto-destroy
        public float speed = 20f;           // Movement speed of the projectile

        void Start()
        {
            // Automatically destroy the projectile after its lifetime
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            GoForward();
        }

        public void GoForward()
        {
            // Move forward in local space every frame
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (impactPrefab != null)
            {
                // Get the first contact point and surface normal
                ContactPoint contact = collision.contacts[0];
                Quaternion rot = Quaternion.LookRotation(contact.normal);

                // Spawn impact effect at contact point, aligned with surface normal
                GameObject impactInstance = Instantiate(impactPrefab, contact.point, rot);
                Destroy(impactInstance, 1f);
            }

            // Destroy the projectile immediately after impact
            Destroy(gameObject);
        }
    }
}