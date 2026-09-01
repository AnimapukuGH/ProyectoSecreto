using UnityEngine;
using UnityEngine.EventSystems;

// Este script va pegado en el PANEL del segundo Canvas
public class PanelClickDetector : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public ColorToggle scriptPrincipal;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Cuando alguien hace clic en el panel, avisa al script del Toggle
        if (scriptPrincipal != null)
        {
            scriptPrincipal.CerrarDesdePanel();
        }
    }
}
