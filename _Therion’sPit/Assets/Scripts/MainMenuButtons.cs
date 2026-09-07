using UnityEngine;
using System.Collections;


public class MainMenuButtons : MonoBehaviour
{
    private Animator animator;
    
    void Start(){
        animator = GetComponent<Animator>();
        if (animator == null){
            animator = GetComponentInParent<Animator>();
            Debug.LogWarning("No Animator found on ");
        }  
        
    }

    public void OnCampaignButtonClicked(){
        Debug.Log("Campaign button clicked ");
        //load part stage 
    }

    public void OnSetiingsButtonClicked(){
        Debug.Log("Settings Button Clicked ");
        //load settingd menu
    }
    
    public void OnQuitButtonClicked(){//fix later 
        Debug.Log("Quit button clicked ");
        //Application.Quit();
    }

    
  
}