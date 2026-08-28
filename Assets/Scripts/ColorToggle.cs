using UnityEngine;
using UnityEngine.UI; // Necesario para la UI

public class ColorToggle : MonoBehaviour
{
    [Header("Componentes")]
    public Toggle miToggle;
    public Image imagenFondo;

    [Header("Colores")]
    public Color colorDesactivado = Color.white; // Color normal
    public Color colorActivado = Color.green;    // Color al marcarlo

    void Start()
    {
        // Si se nos olvidó asignar el toggle, lo busca automáticamente
        if (miToggle == null)
            miToggle = GetComponent<Toggle>();

        // Ajustamos el color inicial al empezar la escena
        ActualizarColor(miToggle.isOn);

        // Le decimos al Toggle que avise a nuestra función cada vez que se haga clic
        miToggle.onValueChanged.AddListener(ActualizarColor);
    }

    // Esta función se ejecuta sola cada vez que el Toggle cambia
    void ActualizarColor(bool estaMarcado)
    {
        if (estaMarcado)
        {
            imagenFondo.color = colorActivado;
        }
        else
        {
            imagenFondo.color = colorDesactivado;
        }
    }
}