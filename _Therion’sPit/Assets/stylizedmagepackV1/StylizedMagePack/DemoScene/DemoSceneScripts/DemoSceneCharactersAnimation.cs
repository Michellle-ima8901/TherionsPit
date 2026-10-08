using UnityEngine;

namespace StylizedMagePack
{
    public class DemoSceneCharactersAnimation : MonoBehaviour
    {
        // Reference to the Animator component
        private Animator animator;

        // Animations to be played
        public bool playTalkAnimation = false;
        public bool playVictoryAnimation = false;
        public bool playDeathAnimation = false;
        public bool playGetHitAnimation = false;
        public bool playInteractAnimation = false;
        public bool playSpellCastingAnimation = false;
        public bool playSpellLightAnimation = false;
        public bool playSpellHeavyAnimation = false;

        // Timer variables
        private float timer = 0f;
        private float switchTime = 3f;  // Time to switch the boolean values (4 seconds)

        // Start is called before the first frame update
        void Start()
        {
            // Get the Animator component attached to the object
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            // Increment the timer by the time passed since the last frame
            timer += Time.deltaTime;

            // If the timer exceeds the switch time, toggle the boolean values
            if (timer >= switchTime)
            {
                // Reset timer
                timer = 0f;

                // Switch the booleans on and off (turn off after 4 seconds)
                playTalkAnimation = !playTalkAnimation;
                playVictoryAnimation = !playVictoryAnimation;
                playDeathAnimation = !playDeathAnimation;
                playGetHitAnimation = !playGetHitAnimation;
                playInteractAnimation = !playInteractAnimation;
                playSpellCastingAnimation = !playSpellCastingAnimation;
                playSpellLightAnimation = !playSpellLightAnimation;
                playSpellHeavyAnimation = !playSpellHeavyAnimation;
            }

            // Play the selected animation based on the active boolean
            if (playTalkAnimation)
            {
                PlayLoopingAnimation("m_talk");
            }
            else if (playVictoryAnimation)
            {
                PlayLoopingAnimation("m_victory");
            }
            else if (playDeathAnimation)
            {
                PlayLoopingAnimation("m_death");
            }
            else if (playGetHitAnimation)
            {
                PlayLoopingAnimation("m_gethit");
            }
            else if (playInteractAnimation)
            {
                PlayLoopingAnimation("m_interact");
            }
            else if (playSpellCastingAnimation)
            {
                PlayLoopingAnimation("m_spell_casting");
            }
            else if (playSpellLightAnimation)
            {
                PlayLoopingAnimation("m_spell_light");
            }
            else if (playSpellHeavyAnimation)
            {
                PlayLoopingAnimation("m_spell_heavy");
            }
        }

        // This method plays the animation and forces it to loop continuously
        private void PlayLoopingAnimation(string animationName)
        {
            // Check if the animation is not already playing
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName(animationName))
            {
                animator.Play(animationName);
            }
        }
    }
}