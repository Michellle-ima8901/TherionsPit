⚠ IMPORTANT NOTE


* DO NOT use MAGE_ANIMATIONSV2 as the player character.

Use the models inside the Models folder or the prefabs that are already provided.


   * The model included in MAGE_ANIMATIONSV2 is only a reference for hierarchy naming. It is not meant to be used as a playable character and does not affect gameplay mechanics or visuals of the asset.


   * create a new Animator Controller. If your character is Humanoid, simply create a new state in the Animator and assign the animation clip.

   *Animation Setup Notes 
For lock-on locomotion, use the 8-direction locomotion set together with the lock-on attack 
animations. 
For free movement and free aiming, use the standard walk cycle with manual character 
rotation and the free-aim attack animations, as they were authored with a straight spine. 

*Animation Import & Setup 
Any animations labeled UpperBody require an Avatar Mask (or upper-body mask) to work 
correctly. Be sure to create and assign the appropriate mask before using them.


   * If the asset receives animation updates, only the MAGE_ANIMATION FBX will be affected.
Model updates will affect only the files inside the Models folder
.
      For more informations Read the Instructions-Guideline