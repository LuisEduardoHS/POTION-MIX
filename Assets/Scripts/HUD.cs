using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI puntos;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.instance;
        }

        GameManager.OnPointsUpdated += ActualizarTextoPuntos;

        puntos.text = gameManager.PuntosTotales.ToString();
    }

    private void ActualizarTextoPuntos(int nuevosPuntos)
    {
        puntos.text = nuevosPuntos.ToString();
    }

    private void OnDestroy()
    {
        GameManager.OnPointsUpdated -= ActualizarTextoPuntos;
    }
}