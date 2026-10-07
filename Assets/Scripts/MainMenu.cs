using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void OpenMenu()
    {
        SceneManager.LoadSceneAsync(1);
        Time.timeScale = 1;
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void LoadLevel2()
    {
        SceneManager.LoadSceneAsync(4);
    }

    public void LoadLevel3()
    {
        SceneManager.LoadSceneAsync(5);
    }

    public void LoadLevel4()
    {
        SceneManager.LoadSceneAsync(5);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    [SerializeField] GameObject pauseMenu;
    bool gamePaused = false;
    void Update()
    {
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame && gamePaused == false)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Time.timeScale = 0;
            gamePaused = true;
            pauseMenu.SetActive(true);
        }
        else if (Keyboard.current.escapeKey.wasPressedThisFrame && gamePaused == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
            gamePaused = false;
            pauseMenu.SetActive(false);
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        gamePaused = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1; // Reset time scale before quitting
        Application.Quit();
    }
}
