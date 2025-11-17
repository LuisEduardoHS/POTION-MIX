using UnityEngine;

public class FrascoMiel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el Player lo toca
        if (collision.CompareTag("Player"))
        {
            // Llama al GameManager para avisar que ganamos
            if (GameManager.instance != null)
            {
                GameManager.instance.NivelCompletado();
            }

            // (Opcional: añadir sonido de victoria)

            // Destruye el frasco
            Destroy(gameObject);
        }
    }
}