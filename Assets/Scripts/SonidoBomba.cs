using UnityEngine;

public class ButtonSoundKeeper : MonoBehaviour
{
    private AudioSource audioSource;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Evita que se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAndPersist()
    {
        // Reproducir sonido
        audioSource.Play();

        // Ocultar visualmente el botón
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // Destruir después de 2 segundos
        Destroy(gameObject, 2f);
    }
}
