using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para la Imagen

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;

    public Image fadeImage; // La imagen negra que usaremos
    public float fadeSpeed = 1f;

    private void Start()
    {
        // Al empezar el juego (en la escena Menu),
        // nos aseguramos de que la imagen esté negra y ejecutamos
        // la corutina de "fade in" (quitar el negro).
        fadeImage.color = Color.black;
        StartCoroutine(Fade(false)); // (false = de negro a transparente)
    }

    private void Awake()
    {
        // --- Patrón Singleton Persistente ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ¡La magia! No se destruye al cargar escenas.
        }
        else
        {
            // Si ya existe uno (porque volvimos al Menú), destruye esta copia nueva.
            Destroy(gameObject);
        }
    }

    // Esta es la función PÚBLICA que todos los demás scripts llamarán
    public void LoadSceneWithFade(string sceneName)
    {
        // Inicia la corutina que hace el trabajo
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    // Corutina que maneja la secuencia completa de Fade-Carga-Fade
    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // 1. Fade a negro (Fade Out)
        yield return StartCoroutine(Fade(true));

        // 2. Carga la nueva escena
        SceneManager.LoadScene(sceneName);

        // 3. Fade desde negro (Fade In)
        yield return StartCoroutine(Fade(false));
    }

    // Corutina ayudante que anima el color del fade
    private IEnumerator Fade(bool fadeIn)
    {
        float t = 0;
        // Si fadeIn es true, vamos de transparente (alpha 0) a negro (alpha 1)
        // Si fadeIn es false, vamos de negro (alpha 1) a transparente (alpha 0)
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        fadeImage.gameObject.SetActive(true); // Asegura que la imagen esté activa

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            Color newColor = fadeImage.color;
            newColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = newColor;
            yield return null;
        }

        // Asegura el valor final
        Color finalColor = fadeImage.color;
        finalColor.a = endAlpha;
        fadeImage.color = finalColor;

        // Si es un fade-in (false), oculta la imagen al terminar
        if (!fadeIn)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }
}