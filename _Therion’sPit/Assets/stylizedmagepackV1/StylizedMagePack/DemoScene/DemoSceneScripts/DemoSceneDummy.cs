using UnityEngine;
using System.Collections;

namespace StylizedMagePack
{
    public class DemoSceneDummy : MonoBehaviour
    {
        private Animator animator;
        private Collider dummyCollider;

        [Header("Settings")]
        [SerializeField] private int hitsToDie = 7;
        [SerializeField] private float respawnTime = 3f; // Seconds before he comes back
        private int currentHits;

        private void Start()
        {
            animator = GetComponent<Animator>();
            dummyCollider = GetComponent<Collider>();
            currentHits = hitsToDie;

        }

        // Simple take hit method
        public void TakeHit()
        {
            currentHits--;

            if (currentHits <= 0)
            {
                // Start the death and respawn sequence
                StartCoroutine(HandleDeathAndRespawn());
            }
            else
            {
                // Play hit reaction
                animator.CrossFade("m_gethit", 0.1f);
                Debug.Log($"Hit! {currentHits} left.");
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            TakeHit();
        }

        private IEnumerator HandleDeathAndRespawn()
        {
            // 1. Play death animation
            animator.CrossFade("m_death", 0.1f);

            // 2. Disable collider so he doesn't take hits while dead
            dummyCollider.enabled = false;
            Debug.Log("Dummy defeated! Respawning soon...");

            // 3. Wait for the specified time
            yield return new WaitForSeconds(respawnTime);

            // 4. Reset stats and play idle animation
            currentHits = hitsToDie;
            dummyCollider.enabled = true;
            animator.CrossFade("m_idle_loop", 0.2f);

            Debug.Log("Dummy is back!");
        }
    }
}