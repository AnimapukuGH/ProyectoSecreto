using UnityEngine;
using UnityEngine.UI;

public class ColorToggle : MonoBehaviour
{
    [Header("Guardado")]
    [Tooltip("Prefijo base para la memoria. El script le añadirá la escena y el nombre del objeto para ser único.")]
    public string claveGuardado = "EstadoToggle_";

    [Header("Componentes")]
    public Toggle miToggle;
    public Image imagenFondo;
    public GameObject segundoCanvas;

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

        PlayerPrefs.SetInt(claveGuardado, 1);
        PlayerPrefs.Save();
    }

    void ActualizarUI(bool activado)
    {
        if (imagenFondo != null)
            imagenFondo.color = activado ? colorActivado : colorDesactivado;

        if (segundoCanvas != null)
            segundoCanvas.SetActive(activado);
    }
}
