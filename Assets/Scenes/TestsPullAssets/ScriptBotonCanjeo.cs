using UnityEngine;
using UnityEngine.UI; // Obligatorio para controlar el componente Button

public class ScriptBotonCanjeo : MonoBehaviour
{
    [Header("Recompensa Canvas (Panel a mostrar)")]
    [Tooltip("Arrastra aquí el Canvas o Panel que quieres que aparezca al canjear con éxito")]
    public GameObject segundoCanvas;

    [Tooltip("Arrastra aquí el objeto que actuará como botón para cerrar el panel")]
    public Button botonDeCierre;

    void Start()
    {
        // Al empezar el juego, nos aseguramos de que el panel esté escondido
        if (segundoCanvas != null) segundoCanvas.SetActive(false);

        // Vinculamos el botón de cierre por código de forma automática y segura
        if (botonDeCierre != null)
        {
            botonDeCierre.onClick.RemoveAllListeners();
            botonDeCierre.onClick.AddListener(CerrarDesdePanel);
        }
    }

    /// <summary>
    /// Función exacta para conectar al evento On Click() de tu botón de canjeo.
    /// </summary>
    public void IntentarCanjearPuntos()
    {
        // 1. Leer los valores actuales directamente de la memoria del juego
        int puntos1 = PlayerPrefs.GetInt("ScoreGlobal", 0);
        int puntos2 = PlayerPrefs.GetInt("ScoreSecundario", 0);

        // 2. Verificar si el jugador tiene el mínimo requerido (60 puntos)
        if (puntos1 >= 60)
        {
            // 3. Realizar las operaciones matemáticas
            puntos1 -= 60; // Resta 60 a la puntuación 1
            puntos2 += 1;  // Suma +1 a la puntuación 2

            // 4. Guardar los nuevos valores de vuelta en la memoria
            PlayerPrefs.SetInt("ScoreGlobal", puntos1);
            PlayerPrefs.SetInt("ScoreSecundario", puntos2);

            // Forzar el guardado en el disco inmediatamente
            PlayerPrefs.Save();

            Debug.Log($"[Canje Exitoso] ScoreGlobal: {puntos1} | ScoreSecundario: {puntos2}");

            // --- PARTE AÑADIDA DEL OTRO SCRIPT ---
            // Hacemos aparecer el canvas/panel de recompensa tras el canje exitoso
            if (segundoCanvas != null)
            {
                segundoCanvas.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning($"No tienes puntos suficientes en 'ScoreGlobal'. Tienes {puntos1} y necesitas mínimo 60.");
        }
    }

    /// <summary>
    /// Función extraída de tu otro script para esconder el panel al darle al botón de cierre.
    /// </summary>
    public void CerrarDesdePanel()
    {
        if (segundoCanvas != null)
        {
            segundoCanvas.SetActive(false);
        }
    }
}
