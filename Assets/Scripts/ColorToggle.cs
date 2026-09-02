using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necesario para TextMeshPro

public class ColorToggle : MonoBehaviour
{
    [Header("Guardado")]
    [Tooltip("Prefijo base para la memoria. El script le añadirá la escena y el nombre del objeto para ser único.")]
    public string claveGuardado = "EstadoToggle_";

    [Header("Componentes")]
    public Toggle miToggle;
    public Image imagenFondo;
    public GameObject segundoCanvas;

    [Header("Texto de Estado")]
    [Tooltip("Arrastra aquí el componente de texto que muestra el 0/1")]
    public TextMeshProUGUI textoContador;
    // Nota: Si usas el Text antiguo de Unity, cambia la línea de arriba por: public Text textoContador;

    [Tooltip("Arrastra aquí el Panel de fondo del segundo Canvas")]
    public GameObject panelDeCierre;

    [Header("Colores")]
    public Color colorDesactivado = Color.gray;
    public Color colorActivado = Color.green;

    private bool panelCerradoDesdeFondo = false;

    void Start()
    {
        if (miToggle == null)
            miToggle = GetComponent<Toggle>();

        // Creamos una clave única combinando la Escena + el nombre del GameObject.
        string escenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        claveGuardado = claveGuardado + escenaActual + "_" + gameObject.name;

        // Conexión automática con el panel sin usar botones
        if (panelDeCierre != null)
        {
            PanelClickDetector detector = panelDeCierre.GetComponent<PanelClickDetector>();
            if (detector == null)
            {
                detector = panelDeCierre.AddComponent<PanelClickDetector>();
            }
            detector.scriptPrincipal = this;
        }

        // --- CARGAR EL ESTADO GUARDADO ---
        int estadoGuardado = PlayerPrefs.GetInt(claveGuardado, 0);
        bool estabaActivado = (estadoGuardado == 1);

        miToggle.onValueChanged.RemoveListener(OnToggleChanged);

        miToggle.isOn = estabaActivado;

        if (imagenFondo != null)
            imagenFondo.color = estabaActivado ? colorActivado : colorDesactivado;

        // Actualizamos el texto al iniciar según el estado guardado
        ActualizarTexto(estabaActivado);

        if (segundoCanvas != null)
            segundoCanvas.SetActive(false);

        miToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void OnToggleChanged(bool estaMarcado)
    {
        if (panelCerradoDesdeFondo && !estaMarcado)
        {
            ActualizarUI(false);
            panelCerradoDesdeFondo = false;

            PlayerPrefs.SetInt(claveGuardado, 0);
            PlayerPrefs.Save();
            return;
        }

        ActualizarUI(estaMarcado);

        PlayerPrefs.SetInt(claveGuardado, estaMarcado ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void CerrarDesdePanel()
    {
        if (segundoCanvas != null)
            segundoCanvas.SetActive(false);

        panelCerradoDesdeFondo = true;

        if (imagenFondo != null)
            imagenFondo.color = colorActivado;

        // Forzamos que se mantenga en 1/1 ya que permanece activado (verde)
        ActualizarTexto(true);

        PlayerPrefs.SetInt(claveGuardado, 1);
        PlayerPrefs.Save();
    }

    void ActualizarUI(bool activado)
    {
        if (imagenFondo != null)
            imagenFondo.color = activado ? colorActivado : colorDesactivado;

        if (segundoCanvas != null)
            segundoCanvas.SetActive(activado);

        // Cambia el texto dinámicamente
        ActualizarTexto(activado);
    }

    void ActualizarTexto(bool activado)
    {
        if (textoContador != null)
        {
            textoContador.text = activado ? "1/1" : "0/1";
        }
    }
}
