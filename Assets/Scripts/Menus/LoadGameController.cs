using static utils.Constants;
using static utils.Utils;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadGameController : MonoBehaviour {
    public Text[] matchDescriptions;
    public Button[] mainButtons;
    public Button[] deleteButtons;
    public AudioSource uISound;
    private string[] files;

    public void Start() {
        PlayUIAudioIfEnabled(uISound);
        /* Iterates through all the saved games and shows them on the UI */
        files = System.IO.Directory.GetFiles(Application.persistentDataPath);
        for(int i = 0; i < matchDescriptions.Length && i < files.Length; i++) {
            matchDescriptions[i].text = files[i].Substring(files[i].LastIndexOf("/") + 1);
            mainButtons[i].GetComponent<Button>().interactable = true;
            deleteButtons[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Loads the match scene.
    /// Sets PlayerPrefs('matchToLoad') as the match identifier so that the game controller can read it and load the saved match.
    /// </summary>
    /// <param name="buttonId">Id of the pressed button</param>
    /// <remarks>
    /// Handler for button (set up via inspector).
    /// </remarks>
    public void LoadGame(int buttonId) {
        PlayerPrefs.SetString("matchToLoad", matchDescriptions[buttonId].text);
        SceneManager.LoadScene(MatchScene);
    }

    /// <summary>
    /// Deletes a saved match from storage and reloads the scene.
    /// </summary>
    /// <param name="buttonId">Id of the pressed button</param>
    /// <remarks>
    /// Handler for button (set up via inspector).
    /// </remarks>
    public void DeleteSave(int buttonId) {
        File.Delete(files[buttonId]);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reloads the scene
    }

}