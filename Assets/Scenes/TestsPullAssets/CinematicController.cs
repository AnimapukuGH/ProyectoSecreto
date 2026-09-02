using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class TimelineCinematicController : MonoBehaviour
{
    [Header("Cámaras")]
    public GameObject mainCamera;
    public GameObject cinematicCamera;

    [Header("Timeline")]
    public PlayableDirector timelineDirector;

    [Header("UI Fundido")]
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    [Header("UI de la Escena (Gameplay/Menú)")]
    // OPCIÓN A: Arrastra aquí el CanvasGroup de tu interfaz para ocultarla elegantemente
    public CanvasGroup gameplayCanvasGroup;

    // OPCIÓN B: O si lo prefieres, arrastra directamente el GameObject del Canvas para apagarlo
    public GameObject gameplayCanvasObject;

    void Start()
    {
        // Estado inicial de la escena
        if (mainCamera != null) mainCamera.SetActive(true);
        if (cinematicCamera != null) cinematicCamera.SetActive(false);
        if (fadeImage != null) fadeImage.color = new Color(1, 1, 1, 0); // Totalmente transparente

        // Asegurar que la UI sea visible al inicio
        ConfigurarVisibilidadUI(true);
    }

    public void IniciarSecuencia()
    {
        StartCoroutine(SecuenciaTimeline());
    }

    private IEnumerator SecuenciaTimeline()
    {
        // 1. DESAPARECER EL CANVAS inmediatamente al pulsar el botón
        ConfigurarVisibilidadUI(false);

        // 2. Activar cámara cinemática
        if (mainCamera != null) mainCamera.SetActive(false);
        if (cinematicCamera != null) cinematicCamera.SetActive(true);

        // 3. Reproducir el Timeline y esperar a que termine
        if (timelineDirector != null)
        {
            timelineDirector.Play();

            while (timelineDirector.state == PlayState.Playing)
            {
                yield return null;
            }
        }

        // 4. Fundido a Blanco (Fade In)
        yield return StartCoroutine(Fade(0, 1));

        // 5. Cambiar de cámaras (mientras la pantalla está en blanco)
        if (cinematicCamera != null) cinematicCamera.SetActive(false);
        if (mainCamera != null) mainCamera.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        // 6. Volver a MOSTRAR EL CANVAS (se hace aquí para que aparezca tras el fundido)
        ConfigurarVisibilidadUI(true);

        // 7. Desaparecer el Blanco (Fade Out)
        yield return StartCoroutine(Fade(1, 0));
    }

    // Método auxiliar para ocultar/mostrar la interfaz según lo que configures en el Inspector
    private void ConfigurarVisibilidadUI(bool visible)
    {
        // Si usas CanvasGroup (Desactiva visibilidad e interacción, pero no destruye rendimiento)
        if (gameplayCanvasGroup != null)
        {
            gameplayCanvasGroup.alpha = visible ? 1f : 0f;
            gameplayCanvasGroup.interactable = visible;
            gameplayCanvasGroup.blocksRaycasts = visible;
        }

        // Si prefieres apagar el GameObject por completo
        if (gameplayCanvasObject != null)
        {
            gameplayCanvasObject.SetActive(visible);
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}
