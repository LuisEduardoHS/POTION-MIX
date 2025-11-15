using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject mainPausePanel;
    public GameObject settingsPanel;

    private bool isPaused = false;

    private void Start()
    {
        // Ocultar paneles al inicio
        if (mainPausePanel != null) mainPausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        SettingsMenu.OnSettingsClosed += ReShowPausePanel;
    }

    private void OnDestroy()
    {
        SettingsMenu.OnSettingsClosed -= ReShowPausePanel;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!settingsPanel.activeSelf)
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        mainPausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f; // Asegura que el tiempo se reanuda al volver al menú
        SceneManager.LoadScene("Menu");
    }

    public void AbrirAjustes()
    {
        mainPausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void ReShowPausePanel()
    {
        if (isPaused) // Si estamos en pausa, muestra el panel de pausa principal
        {
            mainPausePanel.SetActive(true);
        }
    }

}