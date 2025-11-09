using UnityEngine;

public class Coin : MonoBehaviour
{

    public int valor = 1;
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameManager.SumarPuntos(valor);
            // Destruye la moneda después de ser recogida
            Destroy(this.gameObject);
        }
    }
}
