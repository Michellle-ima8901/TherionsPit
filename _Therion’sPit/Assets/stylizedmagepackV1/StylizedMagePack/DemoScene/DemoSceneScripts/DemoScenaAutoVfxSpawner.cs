using UnityEngine;

namespace StylizedMagePack
{
    public class DemoScenaAutoVfxSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] VfxPrefabs;   // Array containing the VFX variants (same as used in the Mage)
        [SerializeField] private int selectedType = 0;      // Index of the variant to use for this specific demo spawner

        [Header("Auto Spawn Settings")]
        [SerializeField] private float spawnInterval = 2.5f; // Delay between each spawn in seconds
        [SerializeField] private bool randomizeType = false; // If true, picks a random prefab from the array every time
        [SerializeField] Transform FirePoint; // Muzzle point

        private float nextSpawnTime;

        private void Start()
        {
            // Set the timer for the first spawn immediately upon starting
            nextSpawnTime = Time.time + spawnInterval;
        }

        private void Update()
        {
            // Check if enough time has passed to spawn the next VFX
            if (Time.time >= nextSpawnTime)
            {
                // Reset the timer for the next cycle
                nextSpawnTime = Time.time + spawnInterval;

                SpawnFireball();
            }
        }

        private void SpawnFireball()
        {
            int typeIndex = selectedType;

            // Pick a random index if randomization is enabled
            if (randomizeType)
            {
                typeIndex = Random.Range(0, VfxPrefabs.Length);
            }

            // Basic safety check: ensure the array index is valid and the prefab exists
            if (typeIndex < 0 || typeIndex >= VfxPrefabs.Length || VfxPrefabs[typeIndex] == null)
                return;

            var prefab = VfxPrefabs[typeIndex];

            // Instantiate the VFX at the spawner's current position and rotation
            var instance = Instantiate(prefab, FirePoint.position, Quaternion.identity);

            // Try to get the VfxMover component to launch the projectile
            var mover = instance.GetComponent<SimpleProjectileHitVFX>();
            if (mover != null)
            {
                // Launch the VFX in the forward direction of this spawner object
                mover.GoForward();
            }
        }
    }
}
