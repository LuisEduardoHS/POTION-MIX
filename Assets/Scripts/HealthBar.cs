using UnityEngine;
using UnityEngine.UI; // Agrega esta línea para importar el tipo Image

public class HealthBar : MonoBehaviour
{
    public Image rellenoBarraVida;
    private Player playerController;
    private float vidaMaxima;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<Player>();
        vidaMaxima = playerController.vidas;

    }

    // Update is called once per frame
    void Update()
    {
        rellenoBarraVida.fillAmount = playerController.vidas / vidaMaxima;
    }
}
