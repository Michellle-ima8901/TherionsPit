using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace yohansouls.showroom
{
    public class ShowroomAnimationManager : MonoBehaviour
    {
        [Header("References")]
        public ShowroomManager mainManager;

        [Header("Button Setup")]
        public GameObject buttonPrefab;
        public Transform buttonContainer;

        [Header("Button Colors")]
        public Color activeColor = Color.yellow;
        public Color defaultColor = Color.white;

        [Header("Animation List")]
        public List<AnimationClip> animationClips;

        private List<Image> spawnedButtons = new List<Image>();

        void Start()
        {
            if (animationClips.Count > 0)
                CreateButtons();
        }

        void CreateButtons()
        {
            foreach (AnimationClip clip in animationClips)
            {
                if (clip == null) continue;

                GameObject newBtn = Instantiate(buttonPrefab, buttonContainer);
                newBtn.name = "Btn_" + clip.name;
                newBtn.SetActive(true);
                newBtn.GetComponentInChildren<TextMeshProUGUI>().text = clip.name;

                Image btnImg = newBtn.GetComponent<Image>();
                spawnedButtons.Add(btnImg);

                string clipName = clip.name;
                newBtn.GetComponent<Button>().onClick.AddListener(() => PlayAnim(clipName, btnImg));
            }
        }

        public void PlayAnim(string stateName, Image activeImg)
        {
            GameObject activeSetup = mainManager.ActiveSetupPrefab();
            if (activeSetup == null) return;

            Animator activeAnim = activeSetup.GetComponent<Animator>();

            if (activeAnim == null)
                activeAnim = activeSetup.GetComponentInChildren<Animator>(false);

            if (activeAnim != null)
                activeAnim.CrossFadeInFixedTime(stateName, 0.2f);

            foreach (var img in spawnedButtons)
                img.color = (img == activeImg) ? activeColor : defaultColor;
        }
    }
}