using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int PuntosTotales { get { return puntosTotales; }}
    private int puntosTotales = 0;

    public static GameManager instance;

    public static event System.Action<int> OnPointsUpdated;

    [Header("Paneles de UI")]
    public GameObject gameOverPanel;
    public GameObject settingsPanel;

    public TextMeshProUGUI gameOverText;
    public Button reiniciarButton;
    public Button menuButton;

    private bool gameOverActivo = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if(gameOverPanel != null) 
            gameOverPanel.SetActive(false);

        if (reiniciarButton != null)
            reiniciarButton.onClick.AddListener(ReiniciarEscena);

        if(menuButton != null)
            menuButton.onClick.AddListener(IrAlMenu);

        SettingsMenu.OnSettingsClosed += ReShowGameOverPanel;
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento para evitar referencias colgantes
        SettingsMenu.OnSettingsClosed -= ReShowGameOverPanel;
    }

    private void Update()
    {
        if (gameOverActivo)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReiniciarEscena();
            }

            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
            {
                IrAlMenu();
            }
        }
    }

    public void GameOver()
    {
        if(gameOverActivo) return;

        gameOverActivo = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText != null)
        {
            gameOverText.text = "Game Over\n\nPresiona 'R' para reiniciar\nPresiona 'M' para ir al menú";
        }
    }

    public void ReiniciarEscena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void SumarPuntos(int puntosASumar)
    {
        puntosTotales += puntosASumar;
        Debug.Log("Puntos Totales: " + puntosASumar);

        OnPointsUpdated?.Invoke(puntosTotales);
    }

    public void AbrirAjustes_GameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    private void ReShowGameOverPanel()
    {
        // Si el juego terminó, re-muestra el panel de Game Over
        if (gameOverActivo)
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }
    }
}
