using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("ruang kelas"); 
        // pastikan nama persis sama dengan scene yang kamu buat
    }

    public void QuitGame()
    {
        Debug.Log("Keluar game...");
        Application.Quit();
    }
}
