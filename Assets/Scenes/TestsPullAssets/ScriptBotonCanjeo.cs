using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems; 
using TMPro; // AÑADIDO: Necesario si usas TextMeshPro para tus textos

public class ScriptBotonCanjeo : MonoBehaviour
{
    [Header("Recompensa Canvas (Panel a mostrar)")]
    [Tooltip("Arrastra aquí el Canvas o Panel que quieres que aparezca al canjear con éxito")]
    public GameObject segundoCanvas;

    [Tooltip("Arrastra aquí el componente de Texto dentro del panel que muestra la cantidad conseguida")]
    public TextMeshProUGUI textoCantidadRecompensa; // AÑADIDO: Referencia al texto de la recompensa

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
    /// Canjea múltiples bloques de 60 puntos de una sola vez y muestra la cantidad en pantalla.
    /// </summary>
    public void IntentarCanjearPuntos()
    {
        // 1. Leer los valores actuales directamente de la memoria del juego
        int puntos1 = PlayerPrefs.GetInt("ScoreGlobal", 0);
        int puntos2 = PlayerPrefs.GetInt("ScoreSecundario", 0);

        // 2. Verificar si el jugador tiene el mínimo requerido (60 puntos)
        if (puntos1 >= 60)
        {
            // 3. Calcular cuántos canjes completos de 60 puntos se pueden realizar
            int canjesRealizados = puntos1 / 60; 

            // 4. Calcular los puntos sobrantes que no llegan a 60
            int puntosRestantes = puntos1 % 60;  

            // 5. Aplicar los cambios
            puntos1 = puntosRestantes;
            puntos2 += canjesRealizados;

            // 6. Guardar los nuevos valores de vuelta en la memoria
            PlayerPrefs.SetInt("ScoreGlobal", puntos1);
            PlayerPrefs.SetInt("ScoreSecundario", puntos2);

            // Forzar el guardado en el disco inmediatamente
            PlayerPrefs.Save();

            Debug.Log($"[Canje Exitoso] Se canjearon {canjesRealizados} recompensa(s). Nuevo ScoreGlobal: {puntos1} | Nuevo ScoreSecundario: {puntos2}");

           // --- ACTUALIZACIÓN DE TEXTO Y PANEL ---
// Asignamos el valor obtenido al componente de texto directamente
if (textoCantidadRecompensa != null)
{
    textoCantidadRecompensa.text = canjesRealizados.ToString();
}

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

        // Deselecciona el botón de canjeo inmediatamente tras pulsarlo
        DeseleccionarBotonActual();
    }

    /// <summary>
    /// Función para esconder el panel al darle al botón de cierre.
    /// </summary>
    public void CerrarDesdePanel()
    {
        if (segundoCanvas != null)
        {
            segundoCanvas.SetActive(false);
        }

        // Deselecciona el botón de cierre tras pulsarlo
        DeseleccionarBotonActual();
    }

    /// <summary>
    /// Método privado auxiliar para evitar repetir código de deselección.
    /// </summary>
    private void DeseleccionarBotonActual()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}