using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Para la imagen de Fade
using TMPro; // Para el texto de diálogo

public class IntroManager : MonoBehaviour
{
    // --- Esta struct [System.Serializable] es la clave para el Inspector ---
    [System.Serializable]
    public struct LineaDeDialogo
    {
        public string nombre; // "Cornelius" o "Zephyr"
        [TextArea(2, 5)]
        public string texto;
    }

    [Header("Actores y Escenario")]
    public GameObject zephyr;
    public GameObject cornelius;
    public Transform corneliusStartPoint; // Un objeto vacío donde Cornelius empieza (fuera de pantalla, izq)
    public Transform corneliusStopPoint;  // Un objeto vacío donde Cornelius se detiene (frente a Zephyr)
    public Transform zephyrWalkOffPoint;  // Un objeto vacío donde Zephyr se va (fuera de pantalla, der)
    public float moveSpeed = 2f;

    [Header("UI de Diálogo")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject panelDelHUD;

    [Header("Contenido del Diálogo")]
    public LineaDeDialogo[] lineasDeDialogo; // Aquí pones todos tus diálogos en orden
    public float typewriterSpeed = 0.05f; // ¡NUEVO! Velocidad de la "máquina de escribir" (segundos por letra)

    [Header("Contenido Final")]
    [TextArea(2, 5)]
    public string dialogoFinal_Galleta; // Texto para la entrega
    public float tiempoExtraGalleta = 2f; // Tiempo extra después de la anim de "dar"

    [Header("Control de Escena")]
    public string proximaEscena = "SampleScene"; // Escena del Bosque/Tutorial

    // Referencias privadas a los "cerebros" de animación
    private Animator corneliusAnimator;
    private Animator zephyrAnimator;

    void Start()
    {
        // 1. Obtener los componentes Animator
        corneliusAnimator = cornelius.GetComponent<Animator>();
        zephyrAnimator = zephyr.GetComponent<Animator>();

        // 2. Preparar la escena
        cornelius.transform.position = corneliusStartPoint.position;
        dialoguePanel.SetActive(false);
        dialogueText.text = ""; // Limpiar texto

        if (panelDelHUD != null) 
            panelDelHUD.SetActive(false);

        // 3. ¡Lanzar la corutina (la cinemática)!
        StartCoroutine(SecuenciaDeIntro());
    }

    // Esta es la Corutina que maneja la secuencia de eventos
    private IEnumerator SecuenciaDeIntro()
    {
        // --- INICIO DEL GUION ---

        // 1. FADE IN
        // yield return StartCoroutine(Fade(false)); // (false = quitar fade)

        // 2. CORNELIUS CAMINA DE IZQUIERDA A DERECHA
        corneliusAnimator.SetBool("isWalking", true);
        while (Vector2.Distance(cornelius.transform.position, corneliusStopPoint.position) > 0.1f)
        {
            cornelius.transform.position = Vector2.MoveTowards(
                cornelius.transform.position,
                corneliusStopPoint.position,
                moveSpeed * Time.deltaTime);

            yield return null; // Espera un fotograma y repite el 'while'
        }

        // 4. CORNELIUS SE DETIENE
        cornelius.transform.position = corneliusStopPoint.position; // Asegura la posición final
        corneliusAnimator.SetBool("isWalking", false);

        yield return new WaitForSeconds(1f); // Pausa dramática

        // 5. DIÁLOGOS (con efecto typewriter)
        dialoguePanel.SetActive(true);

        foreach (LineaDeDialogo linea in lineasDeDialogo)
        {
            // Activa la animación de "hablar" del personaje correcto
            SetTalking(linea.nombre, true);

            // ¡NUEVO! Llama a la corutina de typewriter
            // El \n es el "Enter" que pediste
            string textoFormateado = $"<b>{linea.nombre}</b>:\n{linea.texto}";
            yield return StartCoroutine(ShowTypewriterText(textoFormateado));

            // Desactiva la animación de "hablar"
            SetTalking(linea.nombre, false);

            yield return new WaitForSeconds(1.5f); // Pausa entre diálogos
        }

        // 6. CORNELIUS DA LA GALLETA
        corneliusAnimator.SetTrigger("doGive"); // Usamos el Trigger que ya hicimos
        string textoGalleta = $"<b>Cornelius</b>:\n{dialogoFinal_Galleta}";
        yield return StartCoroutine(ShowTypewriterText(textoGalleta));

        yield return new WaitForSeconds(tiempoExtraGalleta); // Espera a que termine la anim de dar

        dialoguePanel.SetActive(false); // Oculta el diálogo

        // 7. ZEPHYR SE DA LA VUELTA
        zephyr.transform.localScale = new Vector3(-1, 1, 1); // Voltea el sprite
        yield return new WaitForSeconds(1f); // Pausa

        // 8. ZEPHYR CAMINA Y SE VA
        zephyrAnimator.SetBool("isWalking", true);
        while (Vector2.Distance(zephyr.transform.position, zephyrWalkOffPoint.position) > 0.1f)
        {
            zephyr.transform.position = Vector2.MoveTowards(
                zephyr.transform.position,
                zephyrWalkOffPoint.position,
                moveSpeed * Time.deltaTime);

            yield return null;
        }
        zephyrAnimator.SetBool("isWalking", false);

        // 9. FADE OUT
        SceneTransitionManager.instance.LoadSceneWithFade(proximaEscena);

        yield return null;
    }

    // --- FUNCIONES AYUDANTES ---

    // ¡NUEVO! Corutina para el efecto "Máquina de Escribir" (Typewriter)
    private IEnumerator ShowTypewriterText(string textoCompleto)
    {
        dialogueText.text = ""; // Limpia el texto
        int charIndex = 0;

        // Bucle que añade una letra cada 'typewriterSpeed' segundos
        while (charIndex < textoCompleto.Length)
        {
            // Esto maneja las etiquetas <b>...</b> para que no se escriban letra por letra
            if (textoCompleto[charIndex] == '<')
            {
                while (textoCompleto[charIndex] != '>')
                {
                    dialogueText.text += textoCompleto[charIndex];
                    charIndex++;
                }
            }

            dialogueText.text += textoCompleto[charIndex];
            charIndex++;

            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    // ¡NUEVO! Función para controlar quién está hablando
    private void SetTalking(string nombrePersonaje, bool isTalking)
    {
        if (nombrePersonaje == "Cornelius")
        {
            corneliusAnimator.SetBool("isTalking", isTalking);
            zephyrAnimator.SetBool("isTalking", false);
        }
        else if (nombrePersonaje == "Zephyr")
        {
            corneliusAnimator.SetBool("isTalking", false);
            zephyrAnimator.SetBool("isTalking", isTalking);
        }
        else
        {
            // Si es un narrador, nadie habla
            corneliusAnimator.SetBool("isTalking", false);
            zephyrAnimator.SetBool("isTalking", false);
        }
    }


}