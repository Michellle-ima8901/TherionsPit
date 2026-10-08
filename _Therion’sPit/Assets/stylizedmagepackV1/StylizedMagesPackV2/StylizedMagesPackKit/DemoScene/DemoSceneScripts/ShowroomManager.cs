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

        [Header("Weapon Prefab Cycle")]
        public GameObject[] weaponPrefabs;
        private int weaponIndex = 0;
        private GameObject currentWeapon;

        [Header("Upper Body Layer")]
        // Must match the layer index in the Animator Controller (usually 1)
        public int upperBodyLayerIndex = 1;
        public GameObject upperBodyAnimContainer;
        public GameObject buttonPrefab;
        public Transform upperBodyButtonContainer;
        public List<AnimationClip> upperBodyClips;
        private bool combatModeActive = false;
        private List<GameObject> generatedUpperBodyButtons = new List<GameObject>();


        void Start()
        {
            for (int i = 0; i < pieceButtons.Count; i++)
                if (pieceButtons[i] != null)
                    pieceButtons[i].color = colorOn;

            InitPrefabArrays(setupVariants.malePrefabs, 0);
            InitPrefabArrays(setupVariants.femalePrefabs, 0);

            UpdateDisplay();
            SpawnWeapon(0);
            GenerateUpperBodyButtons();

            if (upperBodyAnimContainer != null)
                upperBodyAnimContainer.SetActive(false);


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
            SpawnWeapon(weaponIndex);
            RefreshAnimator();
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

            SpawnWeapon(weaponIndex);
            RefreshAnimator();
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

        public void CycleWeapon()
        {
            if (weaponPrefabs == null || weaponPrefabs.Length == 0) return;
            weaponIndex = (weaponIndex + 1) % weaponPrefabs.Length;
            SpawnWeapon(weaponIndex);
        }

        private void SpawnWeapon(int index)
        {
            if (weaponPrefabs == null || weaponPrefabs.Length == 0) return;
            if (currentWeapon != null) Destroy(currentWeapon);

            GameObject active = ActiveSetupPrefab();
            if (active == null) return;

            Transform spawnPoint = null;
            foreach (Transform child in active.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag("WeaponSpawnPoint"))
                {
                    spawnPoint = child;
                    break;
                }
            }

            if (spawnPoint == null)
            {
                Debug.LogWarning("WeaponSpawnPoint nao encontrado no prefab ativo!");
                return;
            }

            if (weaponPrefabs[index] == null) return;

            currentWeapon = Instantiate(weaponPrefabs[index], spawnPoint);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }

        // Toggles the upper body layer on/off
        // Hook this up to your Combat Mode button OnClick
        public void ToggleCombatMode()
        {
            combatModeActive = !combatModeActive;

            if (upperBodyAnimContainer != null)
                upperBodyAnimContainer.SetActive(combatModeActive);

            GameObject active = ActiveSetupPrefab();
            if (active == null) return;

            Animator anim = active.GetComponent<Animator>();
            if (anim == null) return;

            anim.SetLayerWeight(upperBodyLayerIndex, combatModeActive ? 1f : 0f);
        }

        private void GenerateUpperBodyButtons()
        {
            if (buttonPrefab == null || upperBodyButtonContainer == null) return;

            foreach (var clip in upperBodyClips)
            {
                if (clip == null) continue;
                AnimationClip captured = clip;

                GameObject btnGO = Instantiate(buttonPrefab, upperBodyButtonContainer);
                generatedUpperBodyButtons.Add(btnGO);

                var tmp = btnGO.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null) tmp.text = clip.name;

                var btn = btnGO.GetComponent<Button>();
                if (btn != null)
                    btn.onClick.AddListener(() => PlayUpperBodyAnim(captured.name));
            }
        }

        // Syncs the active prefab Animator with the current combat mode state
        private void RefreshAnimator()
        {
            GameObject active = ActiveSetupPrefab();
            if (active == null) return;

            Animator anim = active.GetComponent<Animator>();
            if (anim == null) return;

            anim.SetLayerWeight(upperBodyLayerIndex, combatModeActive ? 1f : 0f);
        }

        private void PlayUpperBodyAnim(string stateName)
        {
            GameObject active = ActiveSetupPrefab();
            if (active == null) return;

            Animator anim = active.GetComponent<Animator>();
            if (anim == null) return;

            anim.CrossFadeInFixedTime(stateName, 0.2f, upperBodyLayerIndex);
        }
    }
}