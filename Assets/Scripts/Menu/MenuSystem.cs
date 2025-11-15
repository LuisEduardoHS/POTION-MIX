using UnityEngine;
using UnityEngine.SceneManagement; // ¡Necesario para cambiar de escena!

public class MenuSystem : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject creditsPanel; // Arrastra tu Panel_Creditos aquí

    [Header("Configuración de Escena")]
    public string gameSceneName = "SampleScene"; // Asegúrate de que este es el nombre de tu nivel

    void Start()
    {
        // Ocultar paneles al inicio
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    // --- Funciones para Botones del Menú Principal ---

    public void Jugar()
    {
        // Carga la escena del juego
        SceneManager.LoadScene(gameSceneName);
    }

    public void SalirDelJuego()
    {
        // Esto solo funciona en un build (.exe), no en el Editor
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void AbrirCreditos()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void CerrarCreditos()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}