using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways] // Permite ver el efecto en el editor sin tener que darle al Play
public class ControladorDestelloUI : MonoBehaviour
{
    private Image imagenUI;

    // Esta variable SÍ aparecerá de forma directa en tu ventana de Animation
    [Range(0f, 1f)] public float cantidadBlanco = 0f;

    void Start()
    {
        imagenUI = GetComponent<Image>();
    }

    void Update()
    {
        if (imagenUI != null && imagenUI.material != null)
        {
            // Vincula el deslizador de la animación con la propiedad oculta del shader
            imagenUI.material.SetFloat("_FlashAmount", cantidadBlanco);
        }
    }
}
