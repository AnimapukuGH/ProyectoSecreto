using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Image contenedorSpriteUI;
    [SerializeField] private Button botonClicPantallaCanvas2;

    [Header("Configuración del Fade Principal")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Configuración del Flash entre Sprites")]
    [SerializeField] private float tiempoEntradaBlanco = 0.05f;
    [SerializeField] private float tiempoSalidaBlanco = 0.5f;

    [Header("Configuración de Puntuación")]
    [SerializeField] private int puntosRequeridos = 10;
    [SerializeField] private TextMeshProUGUI textoPuntosVisual;

    [Header("Configuración de Videos Estructurados")]
    [SerializeField] private VideoClip videoNormal;
    [SerializeField] private VideoClip videoEspecial;
    [SerializeField] private int clicsParaVideoEspecial = 5;

    [Header("Mecánica de Sprites (Gacha)")]
    [SerializeField] private List<Sprite> spritesComunes = new List<Sprite>();
    [SerializeField] private List<Sprite> spritesPremios = new List<Sprite>();
    [SerializeField] private Sprite spritePremioFinal;
    [SerializeField] private int tirosParaPremio = 10;
    [SerializeField] private Animator animadorContenedorSprite;

    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoPremioComun;
    [SerializeField] private AudioClip sonidoPremioEspecial;
    [SerializeField] private AudioClip sonidoPremioFinal;
    [SerializeField] private float duracionFadeOutAudio = 0.15f; // 👉 Tiempo que tarda en apagarse el sonido activo

    private bool videoTerminado = false;
    private bool bloqueandoClicsCanvas2 = false;
    private Coroutine flashCoroutine;
    private Coroutine fadeAudioCoroutine;

    private List<Sprite> bolsaPremiosRestantes = new List<Sprite>();
    private float volumenOriginalAudio = 1f;

    private void Start()
    {
        if (audioSource != null)
        {
            volumenOriginalAudio = audioSource.volume;
        }

        EstablecerEstadoInicial();
        InicializarBolsaPremios();

        if (playButton != null) playButton.onClick.AddListener(IntentarIniciarPlayback);
        if (videoPlayer != null) videoPlayer.loopPointReached += OnVideoFinished;

        if (botonClicPantallaCanvas2 != null) botonClicPantallaCanvas2.onClick.AddListener(ProcesarClicCanvas2);
    }

    private void InicializarBolsaPremios()
    {
        bolsaPremiosRestantes = new List<Sprite>();

        for (int i = 0; i < spritesPremios.Count; i++)
        {
            if (PlayerPrefs.GetInt("PremioEntregado_" + i, 0) == 0)
            {
                bolsaPremiosRestantes.Add(spritesPremios[i]);
            }
        }
    }

    private void EstablecerEstadoInicial()
    {
        videoTerminado = false;
        bloqueandoClicsCanvas2 = false;

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
            int contadorClics = PlayerPrefs.GetInt("ClicsAcumuladosVideo", 0);
            contadorClics++;

            puntuacionActual -= puntosRequeridos;
            PlayerPrefs.SetInt("ScoreSecundario", puntuacionActual);

            if (textoPuntosVisual != null)
            {
                textoPuntosVisual.text = puntuacionActual.ToString();
            }

            if (videoPlayer != null)
            {
                if (contadorClics >= clicsParaVideoEspecial)
                {
                    videoPlayer.clip = videoEspecial;
                    contadorClics = 0;
                }
                else
                {
                    videoPlayer.clip = videoNormal;
                }
            }

            PlayerPrefs.SetInt("ClicsAcumuladosVideo", contadorClics);

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
        bloqueandoClicsCanvas2 = true;

        if (canvas2 != null) canvas2.SetActive(true);

        if (canvas2CanvasGroup != null)
        {
            canvas2CanvasGroup.interactable = true;
            canvas2CanvasGroup.blocksRaycasts = true;
        }

        CalcularYMostrarSiguienteSprite();

        float currentTime = 0f;
        Color colorOriginal = faderBlanco.color;

        while (currentTime < tiempoSalidaBlanco)
        {
            currentTime += Time.deltaTime;
            if (faderBlanco != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, currentTime / tiempoSalidaBlanco);
                faderBlanco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
            }
            yield return null;
        }

        if (faderBlanco != null)
        {
            faderBlanco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0f);
        }

        yield return new WaitForSeconds(0.5f);

        bloqueandoClicsCanvas2 = false;
    }

    private void CalcularYMostrarSiguienteSprite()
    {
        int tirosActuales = PlayerPrefs.GetInt("ContadorTirosGacha", 0);
        tirosActuales++;

        if (tirosActuales >= tirosParaPremio)
        {
            if (contenedorSpriteUI != null)
            {
                if (bolsaPremiosRestantes.Count == 0)
                {
                    if (spritePremioFinal != null)
                    {
                        contenedorSpriteUI.sprite = spritePremioFinal;
                        ReproducirSonidoConFade(sonidoPremioFinal);
                    }
                }
                else
                {
                    int indiceAleatorio = Random.Range(0, bolsaPremiosRestantes.Count);
                    Sprite premioElegido = bolsaPremiosRestantes[indiceAleatorio];
                    contenedorSpriteUI.sprite = premioElegido;
                    ReproducirSonidoConFade(sonidoPremioEspecial);

                    int indiceOriginal = spritesPremios.IndexOf(premioElegido);
                    if (indiceOriginal != -1)
                    {
                        PlayerPrefs.SetInt("PremioEntregado_" + indiceOriginal, 1);
                        PlayerPrefs.Save();
                    }

                    bolsaPremiosRestantes.RemoveAt(indiceAleatorio);
                }
            }
            PlayerPrefs.SetInt("ContadorTirosGacha", tirosParaPremio);
        }
        else
        {
            if (spritesComunes.Count > 0 && contenedorSpriteUI != null)
            {
                int indiceAleatorio = Random.Range(0, spritesComunes.Count);
                contenedorSpriteUI.sprite = spritesComunes[indiceAleatorio];
                ReproducirSonidoConFade(sonidoPremioComun);
            }
            PlayerPrefs.SetInt("ContadorTirosGacha", tirosActuales);
        }

        if (animadorContenedorSprite != null)
        {
            animadorContenedorSprite.Play("AparecerSprite", -1, 0f);
        }
    }

    private void ReproducirSonidoConFade(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        if (fadeAudioCoroutine != null) StopCoroutine(fadeAudioCoroutine);
        fadeAudioCoroutine = StartCoroutine(RutinaFadeOutYPlay(clip));
    }

    private IEnumerator RutinaFadeOutYPlay(AudioClip nuevoClip)
    {
        // 👉 Si hay un sonido sonando, bajamos su volumen gradualmente
        if (audioSource.isPlaying)
        {
            float volumenInicial = audioSource.volume;
            float tiempo = 0f;

            while (tiempo < duracionFadeOutAudio)
            {
                tiempo += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(volumenInicial, 0f, tiempo / duracionFadeOutAudio);
                yield return null;
            }

            audioSource.Stop();
        }

        // 👉 Restauramos el volumen y reproducimos el nuevo efecto
        audioSource.volume = volumenOriginalAudio;
        audioSource.clip = nuevoClip;
        audioSource.Play();
    }

    private void ProcesarClicCanvas2()
    {
        if (bloqueandoClicsCanvas2) return;

        int tirosActuales = PlayerPrefs.GetInt("ContadorTirosGacha", 0);

        if (tirosActuales >= tirosParaPremio)
        {
            PlayerPrefs.SetInt("ContadorTirosGacha", 0);
            PlayerPrefs.Save();
            RegresarAlEstadoOriginal();
        }
        else
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashEntreSprites());
        }
    }

    private IEnumerator FlashEntreSprites()
    {
        bloqueandoClicsCanvas2 = true;

        if (faderBlanco != null)
        {
            faderBlanco.color = new Color(faderBlanco.color.r, faderBlanco.color.g, faderBlanco.color.b, 0f);
            faderBlanco.gameObject.SetActive(true);
        }

        float currentTime = 0f;
        Color colorOriginal = faderBlanco.color;

        while (currentTime < tiempoEntradaBlanco)
        {
            currentTime += Time.deltaTime;
            if (faderBlanco != null)
            {
                float alpha = Mathf.Lerp(0f, 1f, currentTime / tiempoEntradaBlanco);
                faderBlanco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
            }
            yield return null;
        }

        CalcularYMostrarSiguienteSprite();

        currentTime = 0f;
        while (currentTime < tiempoSalidaBlanco)
        {
            currentTime += Time.deltaTime;
            if (faderBlanco != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, currentTime / tiempoSalidaBlanco);
                faderBlanco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
            }
            yield return null;
        }

        if (faderBlanco != null)
        {
            faderBlanco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0f);
        }

        yield return new WaitForSeconds(0.5f);

        bloqueandoClicsCanvas2 = false;
    }

    public void RegresarAlEstadoOriginal()
    {
        StopAllCoroutines();

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.volume = volumenOriginalAudio;
        }

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
        if (botonClicPantallaCanvas2 != null) botonClicPantallaCanvas2.onClick.RemoveListener(ProcesarClicCanvas2);
    }
}