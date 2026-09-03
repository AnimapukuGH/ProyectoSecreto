using UnityEngine;
using UnityEngine.EventSystems; // Requerido para manejar la selección

public class AbrirEnlace : MonoBehaviour
{
    public void IrAGoogle()
    {
        // 1. Abre el enlace
        Application.OpenURL("https://www.infojobs.net/");

        // 2. Deselecciona el botón inmediatamente
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
