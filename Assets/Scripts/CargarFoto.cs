using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using SimpleFileBrowser;
using TMPro;

public class CargarFoto : MonoBehaviour
{
    [Header("Identificador Único (¡Importante!)")]
    public string idUnicoFoto;

    [Header("Imágenes")]
    public Image marcoDeFoto;
    public Image imagenCheck;
    public TextMeshProUGUI textoContador;

    [Header("Ratón Personalizado")]
    [Tooltip("Arrastra aquí el GameObject de la UI que usas como ratón personalizado")]
    public GameObject ratonPersonalizadoUI;

    [Header("Recompensa Canvas")]
    public GameObject segundoCanvas;
    [Tooltip("Arrastra aquí el objeto que actuará como botón para cerrar el panel")]
    public Button botonDeCierre;

    void Start()
    {
        if (marcoDeFoto != null) marcoDeFoto.gameObject.SetActive(false);
        if (segundoCanvas != null) segundoCanvas.SetActive(false);

        // Vinculamos el botón de cierre por código de forma segura
        if (botonDeCierre != null)
        {
            botonDeCierre.onClick.RemoveAllListeners();
            botonDeCierre.onClick.AddListener(CerrarDesdePanel);
        }

        string rutaGuardada = PlayerPrefs.GetString(idUnicoFoto, "");

        if (rutaGuardada != "" && File.Exists(rutaGuardada))
        {
            ConvertirFotoASprite(rutaGuardada);
            if (marcoDeFoto != null) marcoDeFoto.gameObject.SetActive(true);
            EncenderCheckmark();
            ActualizarTexto(true);
        }
        else
        {
            ActualizarTexto(false);
        }
    }

    public void AbrirExploradorYMostrar()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Imágenes", ".png", ".jpg", ".jpeg"));
        FileBrowser.SetDefaultFilter(".png");
        StartCoroutine(MostrarExplorador());
    }

    IEnumerator MostrarExplorador()
    {
        // 👉 Mostramos el cursor del sistema y ocultamos el ratón UI personalizado
        ConfigurarEstadoRaton(mostrarSistema: true);

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Selecciona una foto", "Subir");

        if (FileBrowser.Success)
        {
            string rutaArchivo = FileBrowser.Result[0];
            ConvertirFotoASprite(rutaArchivo);

            if (marcoDeFoto != null) marcoDeFoto.gameObject.SetActive(true);

            EncenderCheckmark();
            ActualizarTexto(true);
            PlayerPrefs.SetString(idUnicoFoto, rutaArchivo);

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
                PlayerPrefs.Save();

                // Mostramos el canvas de recompensa solo cuando se sube por primera vez
                yield return new WaitForSecondsRealtime(0.2f);
                if (segundoCanvas != null) segundoCanvas.SetActive(true);
            }
        }

        // 👉 Ocultamos el cursor del sistema y volvemos a mostrar el ratón UI personalizado
        ConfigurarEstadoRaton(mostrarSistema: false);
    }

    /// <summary>
    /// Alterna entre el ratón por defecto del sistema operativo y tu ratón de UI personalizado.
    /// </summary>
    void ConfigurarEstadoRaton(bool mostrarSistema)
    {
        Cursor.visible = mostrarSistema;
        Cursor.lockState = CursorLockMode.None;

        if (ratonPersonalizadoUI != null)
        {
            ratonPersonalizadoUI.SetActive(!mostrarSistema);
        }
    }

    public void CerrarDesdePanel()
    {
        if (segundoCanvas != null) segundoCanvas.SetActive(false);
    }

    void EncenderCheckmark()
    {
        if (imagenCheck != null)
        {
            Color colorHex;
            if (ColorUtility.TryParseHtmlString("#00B99F", out colorHex))
            {
                colorHex.a = 1f;
                imagenCheck.color = colorHex;
            }
        }
    }

    void ActualizarTexto(bool fotoCargada)
    {
        if (textoContador != null) textoContador.text = fotoCargada ? "1/1" : "0/1";
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

        Sprite nuevoSprite = Sprite.Create(textura, new Rect(0, 0, textura.width, textura.height), new Vector2(0.5f, 0.5f));
        marcoDeFoto.sprite = nuevoSprite;
        marcoDeFoto.preserveAspect = true;
    }
}