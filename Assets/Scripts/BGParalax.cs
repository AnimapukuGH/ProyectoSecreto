using UnityEngine;
using UnityEngine.InputSystem; // Obligatorio para el nuevo sistema

public class FondoMovil : MonoBehaviour
{
    public float intensidad = 100f;
    public float suavizado = 5f;

    private RectTransform rect;
    private Vector2 posicionInicial;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.LogError("¡ERROR! Este script DEBE estar pegado dentro de una imagen de UI (Canvas).");
            return;
        }
        posicionInicial = rect.anchoredPosition;
    }

    void Update()
    {
        if (rect == null) return;

        // Lee la posición del ratón usando el Nuevo Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Rango de -0.5 a 0.5
        float mouseX = (mousePos.x / Screen.width) - 0.5f;
        float mouseY = (mousePos.y / Screen.height) - 0.5f;

        Vector2 posicionObjetivo = posicionInicial + new Vector2(mouseX * intensidad, mouseY * intensidad);

        // Movimiento suave
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, posicionObjetivo, Time.deltaTime * suavizado);
    }
}
