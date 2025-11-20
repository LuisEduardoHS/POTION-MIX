using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    [Header("Objetos a Activar")]
    public GameObject wallToActivate; // La pared invisible que se activará
    public Transform respawnPoint;    // El punto donde reaparecerá el jugador

    private void Awake()
    {
        // 1. Suscribirse al evento de respawn
        GameManager.OnPlayerRespawn += ResetArena;
    }

    private void OnDestroy()
    {
        // 2. Desuscribirse para evitar errores
        GameManager.OnPlayerRespawn -= ResetArena;
    }

    private void ResetArena()
    {
        Debug.Log("Evento de Respawn recibido. Reseteando Arena...");

        // 3. Resetea el estado para que el jugador pueda volver a entrar
        hasTriggered = false;

        if (wallToActivate != null)
        {
            wallToActivate.SetActive(false); // ¡Baja la pared!
        }
    }

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el Player entra y es la primera vez
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            // 1. Activa la pared para encerrar al jugador
            if (wallToActivate != null)
            {
                wallToActivate.SetActive(true);
                AudioManager.instance.PlayMusic(AudioManager.instance.musicBoss);
                Debug.Log("¡ARENA CERRADA!");
            }

            // 2. Establece el nuevo punto de reaparición
            if (GameManager.instance != null && respawnPoint != null)
            {
                GameManager.instance.SetCheckpoint(respawnPoint.position);
                Debug.Log("¡Checkpoint establecido!");
            }

        }
    }
}