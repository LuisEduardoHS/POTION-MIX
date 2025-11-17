using UnityEngine;
using TMPro; // ¡Necesario para el texto!

public class ImmunityUI : MonoBehaviour
{
    public TextMeshProUGUI immunityTimerText; // El texto que mostrará "Inmune: 5.0s"
    public Player player; // El script del jugador al que nos suscribiremos

    void Start()
    {
        // 1. Oculta el texto al empezar
        if (immunityTimerText != null)
        {
            immunityTimerText.gameObject.SetActive(false);
        }

        // 2. Busca al jugador por su Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<Player>();
        }

        // 3. ¡La Magia! Suscribirse a los 3 eventos que creamos en Player.cs
        if (player != null)
        {
            player.OnImmunityStarted += HandleImmunityStarted;
            player.OnImmunityTick += HandleImmunityTick;
            player.OnImmunityEnded += HandleImmunityEnded;
        }
        else
        {
            Debug.LogError("ImmunityUI no pudo encontrar al 'Player'!");
        }
    }

    private void OnDestroy()
    {
        // 4. Desuscribirse siempre para evitar errores
        if (player != null)
        {
            player.OnImmunityStarted -= HandleImmunityStarted;
            player.OnImmunityTick -= HandleImmunityTick;
            player.OnImmunityEnded -= HandleImmunityEnded;
        }
    }

    // --- FUNCIONES LLAMADAS POR LOS EVENTOS ---

    // Evento: OnImmunityStarted
    private void HandleImmunityStarted()
    {
        // Muestra el texto
        immunityTimerText.gameObject.SetActive(true);
    }

    // Evento: OnImmunityTick (se llama cada fotograma durante la inmunidad)
    private void HandleImmunityTick(float timeRemaining)
    {
        // Actualiza el texto con el tiempo, formateado a un decimal
        immunityTimerText.text = $"Inmunidad: {timeRemaining:F1}s";
    }

    // Evento: OnImmunityEnded
    private void HandleImmunityEnded()
    {
        // Oculta el texto
        immunityTimerText.gameObject.SetActive(false);
    }
}