using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CargarFoto : MonoBehaviour
{
    [Header("Arrastra aquí la Imagen de tu UI")]
    public Image marcoDeFoto;

    void Start()
    {
        string rutaGuardada = PlayerPrefs.GetString("RutaMiFoto", "");

        if (rutaGuardada != "" && File.Exists(rutaGuardada))
        {
            ConvertirFotoASprite(rutaGuardada);
        }
    }

    public void AbrirExploradorYMostrar()
    {
#if UNITY_EDITOR
        string rutaArchivo = UnityEditor.EditorUtility.OpenFilePanel("Selecciona una foto", "", "png,jpg,jpeg");

        if (rutaArchivo != "")
        {
            ConvertirFotoASprite(rutaArchivo);
            
            // Guardamos la ruta de la foto para que se mantenga al cambiar de escena
            PlayerPrefs.SetString("RutaMiFoto", rutaArchivo);
            
            // --- NUEVO CÓDIGO: LIMITAR A UNA SOLA RECOMPENSA ---
            // Miramos en la memoria si ya cobró la recompensa (0 = No, 1 = Sí)
            int recompensaCobrada = PlayerPrefs.GetInt("RecompensaFoto", 0);
            
            if (recompensaCobrada == 0) // Si es 0, es la primera vez que sube foto
            {
                int scoreActual = PlayerPrefs.GetInt("ScoreGlobal", 0); 
                scoreActual += 100; // Sumamos 100
                PlayerPrefs.SetInt("ScoreGlobal", scoreActual); 
                
                // Le decimos a la memoria que YA ha cobrado el premio
                // Así, la próxima vez que suba foto, esta condición no se cumplirá
                PlayerPrefs.SetInt("RecompensaFoto", 1); 
            }
            // ---------------------------------------------------
            
            PlayerPrefs.Save(); // Confirmamos todos los cambios
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