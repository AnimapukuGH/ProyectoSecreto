using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CargarFoto : MonoBehaviour
{
    [Header("Identificador Único (¡Importante!)")]
    public string idUnicoFoto; // Ej: "FotoEscena1", "FotoEscena2"

    [Header("Arrastra aquí la Imagen de tu UI")]
    public Image marcoDeFoto;

    [Header("Cambio de Color")]
    public Image assetParaCambiarColor;
    public Color colorAlSubirFoto = Color.green;

    void Start()
    {
        string rutaGuardada = PlayerPrefs.GetString(idUnicoFoto, "");

        if (rutaGuardada != "" && File.Exists(rutaGuardada))
        {
            ConvertirFotoASprite(rutaGuardada);

            if (assetParaCambiarColor != null)
            {
                assetParaCambiarColor.color = colorAlSubirFoto;
            }
        }
    }

    public void AbrirExploradorYMostrar()
    {
#if UNITY_EDITOR
        string rutaArchivo = UnityEditor.EditorUtility.OpenFilePanel("Selecciona una foto", "", "png,jpg,jpeg");

        if (rutaArchivo != "")
        {
            ConvertirFotoASprite(rutaArchivo);
            
            PlayerPrefs.SetString(idUnicoFoto, rutaArchivo);
            
            if(assetParaCambiarColor != null) 
            {
                assetParaCambiarColor.color = colorAlSubirFoto;
            }

            string idRecompensa = "Recompensa_" + idUnicoFoto;
            int recompensaCobrada = PlayerPrefs.GetInt(idRecompensa, 0);
            
            // --- AQUÍ REPARTIMOS LOS PUNTOS ---
            if (recompensaCobrada == 0)
            {
                // Sumamos 100 al Score Principal
                int scoreActual = PlayerPrefs.GetInt("ScoreGlobal", 0); 
                scoreActual += 100;
                PlayerPrefs.SetInt("ScoreGlobal", scoreActual); 

                // Sumamos 4 al Score Secundario (NUEVO)
                int scoreSecundario = PlayerPrefs.GetInt("ScoreSecundario", 0);
                scoreSecundario += 4;
                PlayerPrefs.SetInt("ScoreSecundario", scoreSecundario);

                // Registramos que YA se cobró esta foto específica
                PlayerPrefs.SetInt(idRecompensa, 1); 
            }
            
            PlayerPrefs.Save();
        }
#else
        Debug.LogWarning("Para el juego final exportado, necesitas un plugin gratuito de File Browser.");
#endif
    }

    void ConvertirFotoASprite(string ruta)
    {
        byte[] datosImagen = File.ReadAllBytes(ruta);
        Texture2D textura = new Texture2D(2, 2);
        textura.LoadImage(datosImagen);
        Sprite nuevoSprite = Sprite.Create(textura, new Rect(0, 0, textura.width, textura.height), new Vector2(0.5f, 0.5f));
        marcoDeFoto.sprite = nuevoSprite;
        marcoDeFoto.preserveAspect = true;
    }
}