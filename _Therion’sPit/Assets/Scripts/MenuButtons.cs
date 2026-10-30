using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;



public class MenuButtons : MonoBehaviour
{

    private CharacterButton currentSelected;
    private int selectedRole = 0;
    private bool hasLockedIn = false;



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

    
    
    public void SelectCharacter(CharacterButton clickedButton){
        

        if (hasLockedIn) return;
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


    public void OnConfirmButtonClicked()
    {
        Debug.Log("confirm  button clicked ");
        if (hasLockedIn) return;
        if (currentSelected == null)
        {
            Debug.LogWarning("Please select a character first!");
            return;
        }

        // Start the coroutine to wait for the PartyManager to be ready
        StartCoroutine(LockInWhenReady());
    }

    private IEnumerator LockInWhenReady()
    {
        PartyManager partyManager = null;

        // Keep searching until we find it
        while (partyManager == null)
        {
            partyManager = FindObjectOfType<PartyManager>();
            yield return null; // Wait one frame before trying again
        }

        if (partyManager == null)
        {
            Debug.LogError("PartyManager still not found after waiting!");
            yield break;
        }

        // Wait for it to be spawned on the network
        while (!partyManager.IsSpawned)
        {
            yield return null;
        }

        // Now it is safe to send the ServerRpc!
        partyManager.LockInRoleServerRpc(selectedRole);
        
        hasLockedIn = true;
        if (currentSelected != null && currentSelected.rawImage != null)
        {
            currentSelected.rawImage.color = Color.green;
        }
    }


    
    public void OnStartGameClicked()
    {
        PartyManager partyManager = FindObjectOfType<PartyManager>();
        if (partyManager != null)
        {
            partyManager.StartGameServerRpc();
        }
    }

    
    public void ShowError(string message)
    {
        hasLockedIn = false;
        if (currentSelected != null) currentSelected.rawImage.color = Color.white; // Reset color
        Debug.LogError("Server Error: " + message);
        // add a UI Text element to display this message on screen!
    }

    // Call this from a "Host Game" UI Button
    public void StartHost(){
        NetworkManager.Singleton.StartHost();
    }

    // Call this from a "Join Game" UI Button
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }


  
}