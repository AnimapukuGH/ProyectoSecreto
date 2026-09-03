using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ControladorVideo : MonoBehaviour
{
    [System.Serializable]
    public struct ItemPremio
    {
        public Sprite sprite;
        [TextArea(2, 5)] public string textoAsignado;
    }

    [Header("Referencia a Visuales UI")]
    [SerializeField] private VisualesPremioUI visualesUI;

    [Header("Componentes Canvas 1")]
    [SerializeField] private Button playButton;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoRawImage;

    [Header("Componentes Canvas 2")]
    [SerializeField] private GameObject canvas2;
    [SerializeField] private CanvasGroup canvas2CanvasGroup;
    [SerializeField] private Button botonClicPantallaCanvas2;

    [Header("Configuración de Puntuación")]
    [SerializeField] private int puntosRequeridos = 10;
    [SerializeField] private TextMeshProUGUI textoPuntosVisual;
    [SerializeField] private TextMeshProUGUI textoMultiplicadorBoton;

    [Header("Configuración de Videos")]
    [SerializeField] private VideoClip videoNormal;
    [SerializeField] private VideoClip videoEspecial;
    [SerializeField] private int clicsParaVideoEspecial = 5;

    [Header("Mecánica de Sprites y Texto (Gacha)")]
    [SerializeField] private List<ItemPremio> spritesComunes = new List<ItemPremio>();
    [SerializeField] private List<ItemPremio> spritesPremios = new List<ItemPremio>();
    [SerializeField] private ItemPremio premioFinal;
    [SerializeField] private int tirosParaPremio = 10;

    private bool videoTerminado = false;
    private bool bloqueandoClicsCanvas2 = false;
    private Coroutine flashCoroutine;
    private List<ItemPremio> bolsaPremiosRestantes = new List<ItemPremio>();

    private void Start()
    {
        EstablecerEstadoInicial();
        InicializarBolsaPremios();

        if (playButton != null) playButton.onClick.AddListener(IntentarIniciarPlayback);
        if (videoPlayer != null) videoPlayer.loopPointReached += OnVideoFinished;
        if (botonClicPantallaCanvas2 != null) botonClicPantallaCanvas2.onClick.AddListener(ProcesarClicCanvas2);

        int puntuacionInicial = PlayerPrefs.GetInt("ScoreSecundario", 0);
        ActualizarTextoPuntosVisual(puntuacionInicial);
    }

    private void InicializarBolsaPremios()
    {
        bolsaPremiosRestantes = new List<ItemPremio>();
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

        if (videoRawImage != null) videoRawImage.SetActive(false);
        if (playButton != null) playButton.interactable = true;

        DeseleccionarBotonActual();
    }

    private void IntentarIniciarPlayback()
    {
        DeseleccionarBotonActual();
        int puntuacionActual = PlayerPrefs.GetInt("ScoreSecundario", 0);

        if (puntuacionActual >= puntosRequeridos)
        {
            int contadorClics = PlayerPrefs.GetInt("ClicsAcumuladosVideo", 0) + 1;
            puntuacionActual -= puntosRequeridos;

            PlayerPrefs.SetInt("ScoreSecundario", puntuacionActual);
            ActualizarTextoPuntosVisual(puntuacionActual);

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

    private void OnVideoFinished(VideoPlayer vp) => ProcesarFinDeVideo();

    private void ProcesarFinDeVideo()
    {
        if (videoTerminado) return;
        videoTerminado = true;

        if (videoPlayer != null) videoPlayer.Stop();
        if (videoRawImage != null) videoRawImage.SetActive(false);

        if (canvas2 != null) canvas2.SetActive(true);
        if (canvas2CanvasGroup != null)
        {
            canvas2CanvasGroup.interactable = true;
            canvas2CanvasGroup.blocksRaycasts = true;
        }

        if (visualesUI != null)
        {
            StartCoroutine(RutinaRevelarTrasVideo());
        }
        else
        {
            CalcularYMostrarSiguienteSprite();
        }
    }

    private IEnumerator RutinaRevelarTrasVideo()
    {
        bloqueandoClicsCanvas2 = true;

        yield return visualesUI.RutinaFlash(() => {
            CalcularYMostrarSiguienteSprite();
        });

        yield return new WaitForSeconds(0.5f);
        bloqueandoClicsCanvas2 = false;
    }

    private void CalcularYMostrarSiguienteSprite()
    {
        int tirosActuales = PlayerPrefs.GetInt("ContadorTirosGacha", 0) + 1;
        ItemPremio premioSeleccionado = default;
        VisualesPremioUI.TipoPremio tipoPremio = VisualesPremioUI.TipoPremio.Comun;

        if (tirosActuales >= tirosParaPremio)
        {
            if (bolsaPremiosRestantes.Count == 0)
            {
                if (premioFinal.sprite != null)
                {
                    premioSeleccionado = premioFinal;
                    tipoPremio = VisualesPremioUI.TipoPremio.Final;
                }
            }
            else
            {
                int indiceAleatorio = Random.Range(0, bolsaPremiosRestantes.Count);
                premioSeleccionado = bolsaPremiosRestantes[indiceAleatorio];
                tipoPremio = VisualesPremioUI.TipoPremio.Especial;

                int indiceOriginal = spritesPremios.FindIndex(item => item.sprite == premioSeleccionado.sprite);
                if (indiceOriginal != -1)
                {
                    PlayerPrefs.SetInt("PremioEntregado_" + indiceOriginal, 1);
                    PlayerPrefs.Save();
                }

                bolsaPremiosRestantes.RemoveAt(indiceAleatorio);
            }
            PlayerPrefs.SetInt("ContadorTirosGacha", tirosParaPremio);
        }
        else
        {
            if (spritesComunes.Count > 0)
            {
                int indiceAleatorio = Random.Range(0, spritesComunes.Count);
                premioSeleccionado = spritesComunes[indiceAleatorio];
                tipoPremio = VisualesPremioUI.TipoPremio.Comun;
            }
            PlayerPrefs.SetInt("ContadorTirosGacha", tirosActuales);
        }

        if (visualesUI != null)
        {
            visualesUI.MostrarPremio(premioSeleccionado.sprite, premioSeleccionado.textoAsignado, tipoPremio);
        }
    }

    private void ProcesarClicCanvas2()
    {
        DeseleccionarBotonActual();
        if (bloqueandoClicsCanvas2) return;

        int tirosActuales = PlayerPrefs.GetInt("ContadorTirosGacha", 0);

        if (tirosActuales >= tirosParaPremio)
        {
            PlayerPrefs.SetInt("ContadorTirosGacha", 0);
            PlayerPrefs.Save();
            RegresarAlEstadoOriginal();
        }
        else if (visualesUI != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(RutinaProcesarFlash());
        }
    }

    private IEnumerator RutinaProcesarFlash()
    {
        bloqueandoClicsCanvas2 = true;

        yield return visualesUI.RutinaFlash(() => {
            CalcularYMostrarSiguienteSprite();
        });

        yield return new WaitForSeconds(0.5f);
        bloqueandoClicsCanvas2 = false;
    }

    public void RegresarAlEstadoOriginal()
    {
        StopAllCoroutines();

        if (visualesUI != null) visualesUI.DetenerAudio();
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            if (videoPlayer.targetTexture != null) videoPlayer.targetTexture.Release();
        }

        EstablecerEstadoInicial();
    }

    private void DeseleccionarBotonActual()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    public void ActualizarTextoPuntosVisual(int puntosReales)
    {
        int puntosClampeados = Mathf.Clamp(puntosReales, 0, 10);
        if (textoMultiplicadorBoton != null) textoMultiplicadorBoton.text = "x" + puntosClampeados;
        if (textoPuntosVisual != null) textoPuntosVisual.text = puntosReales.ToString();
    }

    private void OnDestroy()
    {
        if (playButton != null) playButton.onClick.RemoveListener(IntentarIniciarPlayback);
        if (videoPlayer != null) videoPlayer.loopPointReached -= OnVideoFinished;
        if (botonClicPantallaCanvas2 != null) botonClicPantallaCanvas2.onClick.RemoveListener(ProcesarClicCanvas2);
    }
}