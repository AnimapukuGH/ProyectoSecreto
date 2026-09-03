using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class VisualesPremioUI : MonoBehaviour
{
    public enum TipoPremio { Comun, Especial, Final }

    [Header("Componentes de UI")]
    [SerializeField] private Image contenedorSpriteUI;
    [SerializeField] private TextMeshProUGUI textoPremioUI;
    [SerializeField] private Image faderBlanco;

    [Header("Estrellas de Premio")]
    [SerializeField] private GameObject contenedorEstrellas;
    [SerializeField] private List<GameObject> listaEstrellas = new List<GameObject>();
    [SerializeField] private float retrasoInicialEstrellas = 0.5f;
    [SerializeField] private float retrasoEntreEstrellas = 0.15f;
    [SerializeField] private float duracionEscalado = 0.2f;

    [Header("Animador del Sprite del Premio")]
    [SerializeField] private Animator animadorContenedorSprite;
    [SerializeField] private string nombreEstadoAnimacionSprite = "AparecerSprite"; // Nombre del nodo en el Animator del Sprite

    [Header("Animador del Fondo del Premio")]
    [SerializeField] private Animator animadorFondoPremio;
    [SerializeField] private string nombreEstadoAnimacionFondo = "BGPremios"; // Nombre del nodo en el Animator del Fondo

    [Header("Animador del Texto")]
    [SerializeField] private Animator animadorTextoPremio;
    [SerializeField] private string nombreEstadoAnimacionTexto = "AparecerTexto";

    [Header("Configuración de Flash")]
    [SerializeField] private float tiempoEntradaBlanco = 0.05f;
    [SerializeField] private float tiempoSalidaBlanco = 0.5f;

    [Header("Audio del Sistema")]
    [SerializeField] private AudioSource audioSource;

    [Header("Clips de Premio")]
    [SerializeField] private AudioClip sonidoPremioComun;
    [SerializeField] private AudioClip sonidoPremioEspecial;
    [SerializeField] private AudioClip sonidoPremioFinal;

    private float volumenOriginalAudio = 1f;
    private Coroutine fadeAudioCoroutine;
    private Coroutine corrutinaEstrellas;

    private void Awake()
    {
        if (audioSource != null)
        {
            volumenOriginalAudio = audioSource.volume;
            if (volumenOriginalAudio <= 0.01f) volumenOriginalAudio = 1f;
        }
    }

    public void MostrarPremio(Sprite sprite, string texto, TipoPremio tipo)
    {
        // 1. Asignar la imagen del premio
        if (contenedorSpriteUI != null && sprite != null)
        {
            contenedorSpriteUI.sprite = sprite;
        }

        // 2. REPRODUCIR ANIMACIÓN DEL SPRITE DEL PREMIO
        if (animadorContenedorSprite != null)
        {
            animadorContenedorSprite.gameObject.SetActive(true);
            animadorContenedorSprite.enabled = false;
            animadorContenedorSprite.enabled = true;
            animadorContenedorSprite.Play(nombreEstadoAnimacionSprite, 0, 0f);
        }

        // 3. REPRODUCIR ANIMACIÓN DEL FONDO (BG)
        if (animadorFondoPremio != null)
        {
            animadorFondoPremio.gameObject.SetActive(true);
            animadorFondoPremio.enabled = false;
            animadorFondoPremio.enabled = true;
            animadorFondoPremio.Play(nombreEstadoAnimacionFondo, 0, 0f);
        }

        // 4. Texto del Premio
        if (textoPremioUI != null)
        {
            textoPremioUI.gameObject.SetActive(false);
            textoPremioUI.text = texto;
            textoPremioUI.gameObject.SetActive(true);

            if (animadorTextoPremio != null)
            {
                animadorTextoPremio.enabled = false;
                animadorTextoPremio.enabled = true;
                animadorTextoPremio.Play(nombreEstadoAnimacionTexto, 0, 0f);
            }
        }

        // 5. Estrellas
        ActualizarEstrellasProgresivas(tipo);

        // 6. Audio
        AudioClip clipAProducir = ObtenerClipPorTipo(tipo);
        if (clipAProducir != null)
        {
            ReproducirSonidoDirecto(clipAProducir);
        }
    }

    private void ActualizarEstrellasProgresivas(TipoPremio tipo)
    {
        if (contenedorEstrellas == null || listaEstrellas.Count == 0) return;

        if (corrutinaEstrellas != null)
        {
            StopCoroutine(corrutinaEstrellas);
        }

        int cantidadEstrellas = 3;
        if (tipo == TipoPremio.Especial) cantidadEstrellas = 4;
        else if (tipo == TipoPremio.Final) cantidadEstrellas = 5;

        corrutinaEstrellas = StartCoroutine(RutinaAparecerEstrellasProgresivas(cantidadEstrellas));
    }

    private IEnumerator RutinaAparecerEstrellasProgresivas(int cantidadVisibles)
    {
        contenedorEstrellas.SetActive(true);

        for (int i = 0; i < listaEstrellas.Count; i++)
        {
            if (listaEstrellas[i] != null)
            {
                bool esVisible = i < cantidadVisibles;
                listaEstrellas[i].SetActive(esVisible);
                listaEstrellas[i].transform.localScale = Vector3.zero;
            }
        }

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(contenedorEstrellas.GetComponent<RectTransform>());

        yield return new WaitForSeconds(retrasoInicialEstrellas);

        for (int i = 0; i < cantidadVisibles; i++)
        {
            if (i < listaEstrellas.Count && listaEstrellas[i] != null)
            {
                GameObject estrella = listaEstrellas[i];

                float t = 0f;
                while (t < duracionEscalado)
                {
                    t += Time.deltaTime;
                    float escala = Mathf.Lerp(0f, 1f, t / duracionEscalado);
                    estrella.transform.localScale = new Vector3(escala, escala, escala);
                    yield return null;
                }
                estrella.transform.localScale = Vector3.one;

                yield return new WaitForSeconds(retrasoEntreEstrellas);
            }
        }
    }

    private AudioClip ObtenerClipPorTipo(TipoPremio tipo)
    {
        switch (tipo)
        {
            case TipoPremio.Comun: return sonidoPremioComun;
            case TipoPremio.Especial: return sonidoPremioEspecial;
            case TipoPremio.Final: return sonidoPremioFinal;
            default: return null;
        }
    }

    public IEnumerator RutinaFlash(System.Action alAlcanzarPuntoBlanco)
    {
        if (textoPremioUI != null)
        {
            textoPremioUI.gameObject.SetActive(false);
        }

        if (corrutinaEstrellas != null)
        {
            StopCoroutine(corrutinaEstrellas);
        }

        if (contenedorEstrellas != null)
        {
            contenedorEstrellas.SetActive(false);
        }

        if (faderBlanco != null)
        {
            faderBlanco.gameObject.SetActive(true);
        }

        float t = 0f;
        Color colorOriginal = faderBlanco != null ? faderBlanco.color : Color.white;
        float alphaInicial = colorOriginal.a;

        while (t < tiempoEntradaBlanco)
        {
            t += Time.deltaTime;
            if (faderBlanco != null)
            {
                faderBlanco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, Mathf.Lerp(alphaInicial, 1f, t / tiempoEntradaBlanco));
            }
            yield return null;
        }

        alAlcanzarPuntoBlanco?.Invoke();

        t = 0f;
        while (t < tiempoSalidaBlanco)
        {
            t += Time.deltaTime;
            if (faderBlanco != null)
            {
                faderBlanco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, Mathf.Lerp(1f, 0f, t / tiempoSalidaBlanco));
            }
            yield return null;
        }

        if (faderBlanco != null)
        {
            faderBlanco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0f);
        }
    }

    public void ReproducirSonidoDirecto(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        if (fadeAudioCoroutine != null)
        {
            StopCoroutine(fadeAudioCoroutine);
        }

        audioSource.Stop();
        audioSource.volume = volumenOriginalAudio;
        audioSource.clip = clip;
        audioSource.time = 0f;
        audioSource.Play();
    }

    public void DetenerAudio()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.volume = volumenOriginalAudio;
        }
    }
}