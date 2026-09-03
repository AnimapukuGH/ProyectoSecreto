using UnityEngine;

public class CanjeadorPuntos : MonoBehaviour
{
    // Variables globales donde almacenas tus puntos actuales
    public int puntuacion1;
    public int puntuacion2;

    /// <summary>
    /// Esta es la función que ejecutará el botón al ser pulsado.
    /// </summary>
    public void IntentarCanjearPuntos()
    {
        // Verifica si la puntuación 1 tiene el mínimo requerido (60)
        if (puntuacion1 >= 60)
        {
            puntuacion1 -= 60; // Resta 60 a la puntuación 1
            puntuacion2 += 1;  // Suma +1 a la puntuación 2

            Debug.Log("Canje exitoso. Puntuación 1: " + puntuacion1 + " | Puntuación 2: " + puntuacion2);

            // Aquí puedes llamar a tus funciones que actualizan los textos en pantalla si las tienes
        }
        else
        {
            Debug.Log("No tienes suficientes puntos. Mínimo requerido: 60.");
        }
    }
}
