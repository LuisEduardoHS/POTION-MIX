using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public Button craftButton;     // El botón de "Fabricar Poción"
    public int costToCraft = 5;    // El costo (5 pociones/puntos)

    private GameManager gameManager;

    void Start()
    {
        // 1. Busca el GameManager en la escena
        gameManager = GameManager.instance;
        if (gameManager == null)
        {
            Debug.LogError("CraftingUI no pudo encontrar el GameManager!");
            return;
        }

        // 2. Suscríbete al evento de actualización de puntos
        GameManager.OnPointsUpdated += UpdateCraftButtonState;

        // 3. Establece el estado inicial del botón (probablemente desactivado)
        UpdateCraftButtonState(gameManager.PuntosTotales);
    }

    private void OnDestroy()
    {
        // 4. Desuscríbete para evitar errores
        GameManager.OnPointsUpdated -= UpdateCraftButtonState;
    }

    private void UpdateCraftButtonState(int currentPoints)
    {
        // 5. La lógica clave: Si tienes suficientes puntos, el botón se puede presionar.
        if (currentPoints >= costToCraft)
        {
            craftButton.interactable = true;
        }
        else
        {
            craftButton.interactable = false;
        }
    }

    public void OnCraftButtonPressed()
    {
        // 6. Doble chequeo de seguridad
        if (gameManager.PuntosTotales >= costToCraft)
        {
            // 7. Gasta los puntos (el GameManager avisará a todos los que escuchan)
            gameManager.GastarPociones(costToCraft);

            // 8. Llama al Player para activar la inmunidad
            gameManager.player.ActivateImmunity();
        }
    }
}