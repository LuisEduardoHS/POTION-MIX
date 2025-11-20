using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Mixer")]
    public AudioMixer mainMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Para la música de fondo
    public AudioSource sfxSource;   // Para efectos de sonido

    [Header("Music Clips")]
    public AudioClip musicMenu;
    public AudioClip musicCabana;
    public AudioClip musicBosque;
    public AudioClip musicVolcan;
    public AudioClip musicBoss;
    public AudioClip musicVictoria;

    [Header("Player SFX Clip")]
    public AudioClip sfxPlayerJump;
    public AudioClip sfxPlayerDamage;
    public AudioClip sfxPlayerThrow;
    public AudioClip sfxPlayerRun;
    public AudioClip sfxCoin;

    [Header("Enemy SFX Clips")]
    public AudioClip sfxAvispaAttack;

    [Header("Future SFX (Placeholders)")]
    public AudioClip sfxAvispaFlyLoop;
    public AudioClip sfxCorneliusSteps;

    private void Awake()
    {
        // Patrón Singleton Persistente
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string currentSceneName = "";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        float defaultVolume = 0.5f;
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(defaultVolume) * 20);
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(defaultVolume) * 20);

        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

    }

    void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name; 

        if (currentSceneName == "Menu")
        {
            PlayMusic(musicMenu);
        }
        else if (currentSceneName == "Cabana_Intro")
        {
            PlayMusic(musicCabana);
        }
        else if (currentSceneName == "SampleScene") // Bosque
        {
            PlayMusic(musicBosque);
        }
        else if (currentSceneName == "Volcan_Nivel") // Volcán
        {
            PlayMusic(musicVolcan);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // --- Funciones para los Sliders de Ajustes ---

    public void SetMusicVolume(float volume)
    {
        // 'volume' viene de 0 a 1 (del slider)
        // 'MusicVolume' es el nombre del parámetro que crearemos en el AudioMixer
        // Convertimos de lineal (0-1) a logarítmico (decibelios)
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        // 'SFXVolume' es el parámetro para los efectos de sonido
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    // --- Función para reproducir Efectos de Sonido ---

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}