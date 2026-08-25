using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CargarFoto : MonoBehaviour
{
    [Header("Arrastra aquí la Imagen de tu UI")]
    public Image marcoDeFoto;

    // Cuando la escena empieza, comprobamos si ya había una foto guardada
    void Start()
    {
        // Buscamos si guardamos una ruta anteriormente
        string rutaGuardada = PlayerPrefs.GetString("RutaMiFoto", "");

        // Si la ruta no está vacía y el archivo sigue existiendo en el ordenador...
        if (rutaGuardada != "" && File.Exists(rutaGuardada))
        {
            ConvertirFotoASprite(rutaGuardada); // La cargamos automáticamente
        }
    }

    public void AbrirExploradorYMostrar()
    {
#if UNITY_EDITOR
        string rutaArchivo = UnityEditor.EditorUtility.OpenFilePanel("Selecciona una foto", "", "png,jpg,jpeg");

        if (rutaArchivo != "")
        {
            ConvertirFotoASprite(rutaArchivo);
            
            // ¡NUEVO! Guardamos la ruta del archivo en la memoria del juego
            PlayerPrefs.SetString("RutaMiFoto", rutaArchivo);
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