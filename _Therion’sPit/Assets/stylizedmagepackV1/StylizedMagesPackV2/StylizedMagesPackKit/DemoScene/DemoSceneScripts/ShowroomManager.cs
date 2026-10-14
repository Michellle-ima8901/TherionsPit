using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace yohansouls.showroom
{
    public class ShowroomManager : MonoBehaviour
    {
        [System.Serializable]
        public class SetupVariants
        {
            public GameObject[] malePrefabs;
            public GameObject[] femalePrefabs;
        }


        [Header("Character References")]
        public GameObject maleObject;
        public GameObject femaleObject;
        public TextMeshProUGUI genreText;
        public bool isMale = true;

        [Header("Toggle UI Colors")]
        public Color colorOn = Color.green;
        public Color colorOff = Color.red;

        [Header("Piece Toggle Buttons")]
        public List<Image> pieceButtons;

        [Header("Setup Prefab Cycle")]
        public SetupVariants setupVariants;
        private int setupIndex = 0;

        [Header("Weapon - Sword (Right Hand)")]
        public GameObject swordPrefab;
        private GameObject currentSword;

        [Header("Weapon - Shield Variants (Left Hand)")]
        public GameObject[] shieldPrefabs;
        private int shieldIndex = 0;
        private GameObject currentShield;


        void Start()
        {
            for (int i = 0; i < pieceButtons.Count; i++)
                if (pieceButtons[i] != null)
                    pieceButtons[i].color = colorOn;

            InitPrefabArrays(setupVariants.malePrefabs, 0);
            InitPrefabArrays(setupVariants.femalePrefabs, 0);

            UpdateDisplay();
            SpawnWeapons();
        }

        private void InitPrefabArrays(GameObject[] arr, int activeIndex)
        {
            if (arr == null) return;
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] != null) arr[i].SetActive(i == activeIndex);
        }

        public GameObject ActiveSetupPrefab()
        {
            var arr = isMale ? setupVariants.malePrefabs : setupVariants.femalePrefabs;
            if (arr == null || arr.Length == 0) return null;
            return arr[setupIndex];
        }


        public void ToggleGenre()
        {
            isMale = !isMale;
            UpdateDisplay();
            SpawnWeapons();
        }

        private void UpdateDisplay()
        {
            if (maleObject == null || femaleObject == null) return;
            maleObject.SetActive(isMale);
            femaleObject.SetActive(!isMale);
            if (genreText != null)
                genreText.text = isMale ? "MALE" : "FEMALE";

            SyncSetupVisibility();
        }


        private void SyncSetupVisibility()
        {
            var inactive = isMale ? setupVariants.femalePrefabs : setupVariants.malePrefabs;
            if (inactive != null)
                foreach (var go in inactive)
                    if (go != null) go.SetActive(false);

            var active = isMale ? setupVariants.malePrefabs : setupVariants.femalePrefabs;
            if (active != null)
                for (int i = 0; i < active.Length; i++)
                    if (active[i] != null) active[i].SetActive(i == setupIndex);
        }

        public void CycleSetup()
        {
            var arr = isMale ? setupVariants.malePrefabs : setupVariants.femalePrefabs;
            if (arr == null || arr.Length == 0) return;

            if (arr[setupIndex] != null) arr[setupIndex].SetActive(false);
            setupIndex = (setupIndex + 1) % arr.Length;
            if (arr[setupIndex] != null) arr[setupIndex].SetActive(true);

            SpawnWeapons();
        }

        public void TogglePieceByTag(string tag)
        {
            GameObject active = ActiveSetupPrefab();
            if (active == null) return;

            foreach (Transform child in active.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag(tag))
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }

        public void ChangeButtonColor(Image btnImage)
        {
            if (btnImage == null) return;
            btnImage.color = (btnImage.color == colorOn) ? colorOff : colorOn;
        }

        public void CycleShield()
        {
            if (shieldPrefabs == null || shieldPrefabs.Length == 0) return;
            shieldIndex = (shieldIndex + 1) % shieldPrefabs.Length;
            SpawnWeapons();
        }

        private void SpawnWeapons()
        {
            // Destroy current weapons
            if (currentSword != null) Destroy(currentSword);
            if (currentShield != null) Destroy(currentShield);

            GameObject active = ActiveSetupPrefab();
            if (active == null) return;

            Transform rightHandPoint = null;
            Transform leftHandPoint = null;

            foreach (Transform child in active.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag("RightHandSpawnPoint ")) rightHandPoint = child;
                if (child.CompareTag("LeftHandSpawnPoint ")) leftHandPoint = child;
                if (rightHandPoint != null && leftHandPoint != null) break;
            }

            // Spawn Sword
            if (swordPrefab != null && rightHandPoint != null)
            {
                currentSword = Instantiate(swordPrefab, rightHandPoint);
                currentSword.transform.localPosition = Vector3.zero;
                currentSword.transform.localRotation = Quaternion.identity;
            }
            else if (rightHandPoint == null)
            {
                Debug.LogWarning("RightHandSpawnPoint nao encontrado no prefab ativo!");
            }

            // Spawn Shield
            if (shieldPrefabs != null && shieldPrefabs.Length > 0 && leftHandPoint != null)
            {
                if (shieldPrefabs[shieldIndex] != null)
                {
                    currentShield = Instantiate(shieldPrefabs[shieldIndex], leftHandPoint);
                    currentShield.transform.localPosition = Vector3.zero;
                    currentShield.transform.localRotation = Quaternion.identity;
                }
            }
            else if (leftHandPoint == null)
            {
                Debug.LogWarning("LeftHandSpawnPoint nao encontrado no prefab ativo!");
            }
        }
    }
}