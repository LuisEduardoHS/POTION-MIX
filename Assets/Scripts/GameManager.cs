using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int PuntosTotales { get { return puntosTotales; }}
    private int puntosTotales = 0;

    public static GameManager instance;

    public Player player;

    private Vector3 currentCheckpoint;
    private bool hasCheckpoint = false;

    public static event System.Action<int> OnPointsUpdated;

    public static event System.Action OnPlayerRespawn;

    [Header("Paneles de UI")]
    public GameObject gameOverPanel;
    public GameObject settingsPanel;
    public GameObject winPanel;

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

        if (winPanel != null)
            winPanel.SetActive(false);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<Player>();
        }
        else
        {
            Debug.LogError("GameManager no pudo encontrar al 'Player'. Asegúrate de que esté etiquetado.");
        }

        if (player != null)
        {
            currentCheckpoint = player.transform.position; // Guarda la pos inicial
            hasCheckpoint = false; // Aún no hemos tocado un checkpoint
        }
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

        // Si SÍ hemos tocado un checkpoint
        if (hasCheckpoint)
        {
            // --- Lógica de RESPAWN ---
            Debug.Log("Respawn en checkpoint...");

            gameOverActivo = false;

            // 1. Oculta el panel de Game Over
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            // 2. Dile al jugador que reaparezca
            if (player != null)
                player.Respawn(currentCheckpoint);

            OnPlayerRespawn?.Invoke();
        }
        else
        {
            // --- Lógica Antigua (Reiniciar nivel completo) ---
            // Si morimos antes del checkpoint, recargamos el nivel
            Debug.Log("Reiniciando escena completa...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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

    public void GastarPociones(int cantidadAGastar)
    {
        puntosTotales -= cantidadAGastar;

        // Avisamos a la UI (al HUD de puntos y al nuevo botón) que el total cambió
        OnPointsUpdated?.Invoke(puntosTotales);
    }

    public void SetCheckpoint(Vector3 newPosition)
    {
        hasCheckpoint = true;
        currentCheckpoint = newPosition;
    }

    public void NivelCompletado()
    {
        Debug.Log("¡NIVEL COMPLETADO!");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // Detiene el juego
        Time.timeScale = 0f;
        AudioManager.instance.PlayMusic(AudioManager.instance.musicVictoria);
    }
}
