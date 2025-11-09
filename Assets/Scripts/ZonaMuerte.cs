using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Aquí puedes agregar lógica adicional si es necesario
            Debug.Log("El jugador ha entrado en la zona de muerte.");
            collision.GetComponent<Player>().RecibeDamage(Vector2.zero, 1000);

        }
    }
}
