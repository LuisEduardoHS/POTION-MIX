using JetBrains.Annotations;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public bool juegoPausado = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Reanudar()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        juegoPausado = false;
    }

    public void Pausar()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        juegoPausado = true;
    }

}
