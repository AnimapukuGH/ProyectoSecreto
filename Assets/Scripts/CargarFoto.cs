using UnityEngine;
using UnityEngine.UI;
using System.IO; 

public class CargarFoto : MonoBehaviour
{
    [Header("Arrastra aquí la Imagen de tu UI")]
    public Image marcoDeFoto;

    [Header("Cambio de Color")]
    public Image assetParaCambiarColor; // El asset (botón, fondo, icono) que va a cambiar
    public Color colorAlSubirFoto = Color.green; // El color que quieres que tome

    void Start()
    {
        string rutaGuardada = PlayerPrefs.GetString("RutaMiFoto", "");

        // Si ya había una foto guardada al empezar la escena...
        if (rutaGuardada != "" && File.Exists(rutaGuardada))
        {
            ConvertirFotoASprite(rutaGuardada);
            
            // Mantenemos el nuevo color porque ya hay foto
            if(assetParaCambiarColor != null) 
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
            
            PlayerPrefs.SetString("RutaMiFoto", rutaArchivo);
            
            // --- NUEVO: CAMBIO DE COLOR AL ADJUNTAR ---
            if(assetParaCambiarColor != null) 
            {
                assetParaCambiarColor.color = colorAlSubirFoto;
            }
            // ------------------------------------------

            int recompensaCobrada = PlayerPrefs.GetInt("RecompensaFoto", 0);
            
            if (recompensaCobrada == 0)
            {
                int scoreActual = PlayerPrefs.GetInt("ScoreGlobal", 0); 
                scoreActual += 100;
                PlayerPrefs.SetInt("ScoreGlobal", scoreActual); 
                PlayerPrefs.SetInt("RecompensaFoto", 1); 
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