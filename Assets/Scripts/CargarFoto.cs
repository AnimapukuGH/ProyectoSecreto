using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.EventSystems;

public class CargarFoto : MonoBehaviour
{
    [Header("Identificador Único (¡Importante!)")]
    public string idUnicoFoto;

    [Header("Arrastra aquí la Imagen de tu UI")]
    public Image marcoDeFoto;

    [Header("Imagen del Checkmark")]
    [Tooltip("Arrastra aquí la imagen del Check o Tic que quieres que se encienda")]
    public Image imagenCheck;

    [Header("Componentes Segundo Canvas")]
    public GameObject segundoCanvas;
    [Tooltip("Arrastra aquí el Panel de fondo del segundo Canvas")]
    public GameObject panelDeCierre;

    void Start()
    {
        if (marcoDeFoto != null)
        {
            marcoDeFoto.gameObject.SetActive(false);
        }

        if (segundoCanvas != null)
        {
            segundoCanvas.SetActive(false);
        }

        if (panelDeCierre != null)
        {
            EventTrigger trigger = panelDeCierre.GetComponent<EventTrigger>() ?? panelDeCierre.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { CerrarDesdePanel(); });
            trigger.triggers.Add(entry);
        }

        string rutaGuardada = PlayerPrefs.GetString(idUnicoFoto, "");

        if (rutaGuardada != "" && File.Exists(rutaGuardada))
        {
            ConvertirFotoASprite(rutaGuardada);

            if (marcoDeFoto != null)
            {
                marcoDeFoto.gameObject.SetActive(true);
            }

            EncenderCheckmark(); // Enciende el check si ya existía una foto guardada
        }
    }

    public void AbrirExploradorYMostrar()
    {
#if UNITY_EDITOR
        string rutaArchivo = UnityEditor.EditorUtility.OpenFilePanel(
            "Selecciona una foto",
            "",
            "png,jpg,jpeg"
        );

        if (!string.IsNullOrEmpty(rutaArchivo))
        {
            StartCoroutine(ProcesoCargaFluida(rutaArchivo));
        }
#else
        Debug.LogWarning(
            "Para el juego final exportado, necesitas un plugin gratuito de File Browser."
        );
#endif
    }

    IEnumerator ProcesoCargaFluida(string ruta)
    {
        ConvertirFotoASprite(ruta);

        if (marcoDeFoto != null)
        {
            marcoDeFoto.gameObject.SetActive(true);
        }

        EncenderCheckmark(); // Enciende el check al cargar una nueva foto

        PlayerPrefs.SetString(idUnicoFoto, ruta);

        string idRecompensa = "Recompensa_" + idUnicoFoto;
        int recompensaCobrada = PlayerPrefs.GetInt(idRecompensa, 0);

        if (recompensaCobrada == 0)
        {
            int scoreActual = PlayerPrefs.GetInt("ScoreGlobal", 0);
            scoreActual += 60;
            PlayerPrefs.SetInt("ScoreGlobal", scoreActual);

            int scoreSecundario = PlayerPrefs.GetInt("ScoreSecundario", 0);
            scoreSecundario += 4;
            PlayerPrefs.SetInt("ScoreSecundario", scoreSecundario);

            PlayerPrefs.SetInt(idRecompensa, 1);
        }

        PlayerPrefs.Save();

        yield return new WaitForSecondsRealtime(0.2f);

        if (recompensaCobrada == 0 && segundoCanvas != null)
        {
            segundoCanvas.SetActive(true);
        }
    }

    public void CerrarDesdePanel()
    {
        if (segundoCanvas != null)
        {
            segundoCanvas.SetActive(false);
        }
    }

    // --- MÉTODO ACTUALIZADO CON TU COLOR HEXADECIMAL 00B99F ---
    void EncenderCheckmark()
    {
        if (imagenCheck != null)
        {
            Color colorHex;
            // Convierte el string hexadecimal a un color real de Unity
            if (ColorUtility.TryParseHtmlString("#00B99F", out colorHex))
            {
                colorHex.a = 1f; // Nos aseguramos de que la opacidad esté al 100%
                imagenCheck.color = colorHex;
            }
        }
    }

    void ConvertirFotoASprite(string ruta)
    {
        byte[] datosImagen = File.ReadAllBytes(ruta);
        Texture2D textura = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        textura.LoadImage(datosImagen);

        for (int x = 0; x < textura.width; x++)
        {
            for (int y = 0; y < textura.height; y++)
            {
                Color pixelColor = textura.GetPixel(x, y);
                if (Mathf.Approximately(pixelColor.r, 1f) &&
                    Mathf.Approximately(pixelColor.g, 1f) &&
                    Mathf.Approximately(pixelColor.b, 1f))
                {
                    pixelColor.a = 0f;
                    textura.SetPixel(x, y, pixelColor);
                }
            }
        }

        textura.Apply();

        if (marcoDeFoto.sprite != null && marcoDeFoto.sprite.texture != null)
        {
            Destroy(marcoDeFoto.sprite.texture);
            Destroy(marcoDeFoto.sprite);
        }

        Sprite nuevoSprite = Sprite.Create(
            textura,
            new Rect(0, 0, textura.width, textura.height),
            new Vector2(0.5f, 0.5f)
        );

        marcoDeFoto.sprite = nuevoSprite;
        marcoDeFoto.preserveAspect = true;
    }
}
