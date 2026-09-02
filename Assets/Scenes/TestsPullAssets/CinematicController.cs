using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class CanvasVideoController : MonoBehaviour
{
    [Header("Componentes Canvas 1")]
    [SerializeField] private Button playButton;
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Componentes Canvas 2")]
    [SerializeField] private GameObject canvas2;
    [SerializeField] private CanvasGroup canvas2CanvasGroup;

    [Header("Configuración del Fade")]
    [SerializeField] private float fadeDuration = 1.5f;

    private void Start()
    {
        // Asegurar estado inicial
        if (canvas2 != null) canvas2.SetActive(false);
        if (canvas2CanvasGroup != null) canvas2CanvasGroup.alpha = 0f;

        // Asignar eventos
        if (playButton != null) playButton.onClick.AddListener(StartPlayback);
        if (videoPlayer != null) videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void StartPlayback()
    {
        if (videoPlayer != null)
        {
            // Desactivar el botón para que no se pulse dos veces
            playButton.interactable = false;
            videoPlayer.Play();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // El video ha terminado, iniciamos la transición
        StartCoroutine(FadeInCanvas2());
    }

    private IEnumerator FadeInCanvas2()
    {
        // Activar el objeto Canvas 2 en la jerarquía
        canvas2.SetActive(true);

        float currentTime = 0f;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            // Incrementa el alpha linealmente hasta 1
            canvas2CanvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
            yield return null;
        }

        // Asegurar que quede totalmente opaco
        canvas2CanvasGroup.alpha = 1f;
    }

    private void OnDestroy()
    {
        // Buena práctica: remover listeners al destruir el objeto
        if (playButton != null) playButton.onClick.RemoveListener(StartPlayback);
        if (videoPlayer != null) videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
