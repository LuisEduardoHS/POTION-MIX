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
            AudioManager.instance.PlaySFX(AudioManager.instance.sfxCoin);
            // Destruye la moneda después de ser recogida
            Destroy(this.gameObject);
        }
    }
}
