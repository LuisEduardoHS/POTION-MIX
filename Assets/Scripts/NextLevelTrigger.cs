using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    [Header("Configuración de Escena")]
    public string sceneToLoad;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"¡TRIGGER TOCADO! Objeto: {collision.gameObject.name}");

        Debug.Log($"Tag del objeto: {collision.gameObject.tag}");

        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            Debug.Log($"¡ÉXITO! Tag 'Player' confirmado. Cargando escena: {sceneToLoad}");
            SceneTransitionManager.instance.LoadSceneWithFade(sceneToLoad);
        }
        else if (hasTriggered)
        {
            Debug.Log("El trigger ya fue activado. Ignorando.");
        }
        else
        {
            Debug.LogWarning($"FALLO: El objeto {collision.gameObject.name} (Tag: {collision.gameObject.tag}) no es 'Player'.");
        }
    }
}