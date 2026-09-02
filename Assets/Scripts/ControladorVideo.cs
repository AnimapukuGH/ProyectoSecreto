using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using TMPro;

public class ControladorVideo : MonoBehaviour
{
    [Header("Componentes Canvas 1")]
    [SerializeField] private Button playButton;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoRawImage;

    [Header("Componentes Canvas 2")]
    [SerializeField] private GameObject canvas2;
    [SerializeField] private CanvasGroup canvas2CanvasGroup;
    [SerializeField] private Image faderBlanco;

    [Header("Configuración del Fade")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Configuración de Puntuación")]
    [SerializeField] private int puntosRequeridos = 10;
    [SerializeField] private TextMeshProUGUI textoPuntosVisual;

    [Header("Configuración de Videos Estructurados")]
    [SerializeField] private VideoClip videoNormal;
    [SerializeField] private VideoClip videoEspecial;
    [SerializeField] private int clicsParaVideoEspecial = 5;

    private bool videoTerminado = false;

    private void Start()
    {
        EstablecerEstadoInicial();

        if (playButton != null) playButton.onClick.AddListener(IntentarIniciarPlayback);
        if (videoPlayer != null) videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void EstablecerEstadoInicial()
    {
        videoTerminado = false;

        if (canvas2 != null) canvas2.SetActive(false);

        if (canvas2CanvasGroup != null)
        {
            canvas2CanvasGroup.alpha = 1f;
            canvas2CanvasGroup.interactable = false;
            canvas2CanvasGroup.blocksRaycasts = false;
        }

        if (faderBlanco != null)
        {
            Color c = faderBlanco.color;
            c.a = 1f;
            faderBlanco.color = c;
            faderBlanco.gameObject.SetActive(true);
        }

        if (videoRawImage != null) videoRawImage.SetActive(false);
        if (playButton != null) playButton.interactable = true;
    }

    private void IntentarIniciarPlayback()
    {
        int puntuacionActual = PlayerPrefs.GetInt("ScoreSecundario", 0);

        if (puntuacionActual >= puntosRequeridos)
        {
            // PERSISTENCIA: Leemos las pulsaciones acumuladas de la memoria del dispositivo
            int contadorClics = PlayerPrefs.GetInt("ClicsAcumuladosVideo", 0);
            contadorClics++;

            // Restamos puntos y guardamos la nueva puntuación
            puntuacionActual -= puntosRequeridos;
            PlayerPrefs.SetInt("ScoreSecundario", puntuacionActual);

            if (textoPuntosVisual != null)
            {
                textoPuntosVisual.text = puntuacionActual.ToString();
            }

            // ASIGNACIÓN DE VIDEO INTERCAMBIABLE
            if (videoPlayer != null)
            {
                if (contadorClics >= clicsParaVideoEspecial)
                {
                    videoPlayer.clip = videoEspecial;
                    contadorClics = 0; // Se reinicia la racha a cero en disco duro
                }
                else
                {
                    videoPlayer.clip = videoNormal;
                }
            }

            // PERSISTENCIA: Guardamos el estado del contador en disco duro de forma segura
            PlayerPrefs.SetInt("ClicsAcumuladosVideo", contadorClics);
            PlayerPrefs.Save();

            StartPlayback();
        }
        else
        {
            Debug.LogWarning("No tienes suficientes puntos. Se requieren: " + puntosRequeridos);
        }
    }

    private void StartPlayback()
    {
        if (videoPlayer != null)
        {
            videoTerminado = false;
            playButton.interactable = false;

            if (videoRawImage != null) videoRawImage.SetActive(true);

            videoPlayer.Play();
            StartCoroutine(VerificarFinDelVideo());
        }
    }

    private IEnumerator VerificarFinDelVideo()
    {
        yield return new WaitUntil(() => videoPlayer.isPlaying);

        while (videoPlayer.isPlaying)
        {
            if (videoPlayer.time >= (videoPlayer.length - 0.1f))
            {
                ProcesarFinDeVideo();
                yield break;
            }
            yield return null;
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        ProcesarFinDeVideo();
    }

    private void ProcesarFinDeVideo()
    {
        if (videoTerminado) return;
        videoTerminado = true;

        if (videoPlayer != null) videoPlayer.Stop();
        if (videoRawImage != null) videoRawImage.SetActive(false);

        StartCoroutine(FadeFromWhite());
    }

    private IEnumerator FadeFromWhite()
    {
        if (canvas2 != null) canvas2.SetActive(true);

        float currentTime = 0f;
        Color colorOriginal = faderBlanco.color;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            if (faderBlanco != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
                faderBlanco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
            }
            yield return null;
        }

        if (faderBlanco != null) faderBlanco.gameObject.SetActive(false);

        if (canvas2CanvasGroup != null)
        {
            canvas2CanvasGroup.interactable = true;
            canvas2CanvasGroup.blocksRaycasts = true;
        }
    }

    public void RegresarAlEstadoOriginal()
    {
        StopAllCoroutines();

        if (videoPlayer != null) videoPlayer.Stop();

        if (videoPlayer != null && videoPlayer.targetTexture != null)
        {
            videoPlayer.targetTexture.Release();
        }

        EstablecerEstadoInicial();
    }

    private void OnDestroy()
    {
        if (playButton != null) playButton.onClick.RemoveListener(IntentarIniciarPlayback);
        if (videoPlayer != null) videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
