using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ControladorAnimacionTrama : MonoBehaviour
{
    [Range(0f, 5f)]
    public float nivelBlanco = 1.0f;

    private Image imagenUI;
    private Material materialInstancia;

    private void OnEnable()
    {
        imagenUI = GetComponent<Image>();
        if (imagenUI != null && imagenUI.material != null)
        {
            // Crea una copia única del material para este objeto
            materialInstancia = new Material(imagenUI.material);
            imagenUI.material = materialInstancia;
        }
    }

    private void Update()
    {
        if (materialInstancia != null)
        {
            materialInstancia.SetFloat("_WhiteLevel", nivelBlanco);
        }
    }
}