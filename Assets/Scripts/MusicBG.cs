using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic instance;
    private AudioSource audioSource;

    [Header("Configuración de Reproducción")]
    [Tooltip("Escribe los nombres de las escenas en las que SÍ debe sonar esta música")]
    [SerializeField] private string[] escenasPermitidas;

    private void Awake()
    {
        // Patron Singleton con DontDestroyOnLoad
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();

            // Pausar inmediatamente si el AudioSource tenía PlayOnAwake activo
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Suscribirse al evento de cambio/carga de escena
        SceneManager.sceneLoaded += EvaluadorDeEscena;
    }

    private void OnDisable()
    {
        // Desuscribirse para evitar fugas de memoria
        SceneManager.sceneLoaded -= EvaluadorDeEscena;
    }

    private void EvaluadorDeEscena(Scene escena, LoadSceneMode modo)
    {
        if (audioSource == null) return;

        // Si la escena que se acaba de activar está en la lista de permitidas
        if (EsEscenaValida(escena.name))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // Si entramos a la PantallaCarga o a una escena no deseada, se detiene
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private bool EsEscenaValida(string nombreEscena)
    {
        if (escenasPermitidas == null || escenasPermitidas.Length == 0) return true;

        foreach (string nombre in escenasPermitidas)
        {
            if (nombre == nombreEscena) return true;
        }

        return false;
    }
}