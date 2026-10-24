using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuButtons : MonoBehaviour
{

    private CharacterButton currentSelected;
    private int selectedRole = 0;



    public void OnCampaignButtonClicked(){
        Debug.Log("Campaign button clicked ");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

    public void OnSettingsButtonClicked(){
        Debug.Log("Settings Button Clicked ");
        
    }
    
    public void OnQuitButtonClicked(){//fix later 
        Debug.Log("Quit button clicked ");
        Application.Quit();
        
    }

    public void OnCreateButtonClicked(){
        Debug.Log("Create button clicked ");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

    public void OnJoinButtonClicked(){
        Debug.Log("Join button clicked ");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
       
    }

    public void OnCreate02ButtonClicked(){
        Debug.Log("Create02 button clicked ");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
       
    }

    public void OnJoin02ButtonClicked(){
        Debug.Log("Join02 button clicked ");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
         
    }

    
    
    public void SelectCharacter(CharacterButton clickedButton)
    {
        // 1. Reset the old button to white
        if (currentSelected != null)
        {
            currentSelected.rawImage.color = Color.white;
        }

        // 2. Highlight the new button to yellow
        clickedButton.rawImage.color = Color.yellow;

        // 3. Save the selection
        currentSelected = clickedButton;
        selectedRole = clickedButton.roleIndex;
        
        Debug.Log("Selected Role: " + selectedRole);
    }

    public int GetSelectedRole()
    {
        return selectedRole;
    }


  
}