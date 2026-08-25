using UnityEngine;
using TMPro; // Esta línea es obligatoria para poder usar los textos de TextMeshPro

public class MostrarPuntuacion : MonoBehaviour
{
    [Header("Arrastra aquí tu texto de la jerarquía")]
    public TextMeshProUGUI textoScore;

    // Usamos Update para que el texto se actualice constantemente al instante
    void Update()
    {
        // Leemos la puntuación global de la memoria (si no hay nada guardado, será 0)
        int scoreActual = PlayerPrefs.GetInt("ScoreGlobal", 0);

        // Escribimos el número en el texto de la pantalla
        textoScore.text = "Score: " + scoreActual.ToString();
    }
}