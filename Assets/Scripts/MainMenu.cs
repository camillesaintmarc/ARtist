using UnityEngine;
using UnityEngine.SceneManagement; // INDISPENSABLE pour changer de scène

public class MainMenu : MonoBehaviour
{
    // Fonction appelée par le bouton
    public void PlayGame()
    {
        // Remplacez "SampleScene" par le nom EXACT de votre scène de jeu AR
        SceneManager.LoadScene("ARtist"); 
    }

    public void QuitGame()
    {
        Debug.Log("Quit the game");
        Application.Quit();
    }
}