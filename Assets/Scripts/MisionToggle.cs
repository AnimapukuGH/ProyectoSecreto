using UnityEngine;
using UnityEngine.UI; // Necesario para usar Toggles

public class MisionToggle : MonoBehaviour
{
    [Header("Escribe un nombre ÚNICO para cada misión")]
    public string idMision; 

    private Toggle miToggle;

    void Start()
    {
        // Cogemos el componente Toggle de este mismo objeto
        miToggle = GetComponent<Toggle>();

        // 1. COMPROBAR MEMORIA: ¿Esta misión ya estaba completada antes?
        // PlayerPrefs.GetInt devuelve 1 si estaba completada, 0 si no.
        if (PlayerPrefs.GetInt(idMision, 0) == 1)
        {
            miToggle.isOn = true; // La marcamos
        }
        else
        {
            miToggle.isOn = false; // La desmarcamos
        }

        // 2. ESCUCHAR CLICS: Le decimos qué hacer cuando el jugador haga clic
        miToggle.onValueChanged.AddListener(AlCambiarToggle);
    }

    void AlCambiarToggle(bool estaMarcado)
    {
        // Leemos la puntuación global actual (si no existe, será 0)
        int scoreActual = PlayerPrefs.GetInt("ScoreGlobal", 0);

        if (estaMarcado)
        {
            scoreActual += 10; // Sumamos 10 puntos
            PlayerPrefs.SetInt(idMision, 1); // Guardamos que esta misión específica está completada
        }
        else
        {
            scoreActual -= 10; // Restamos 10 puntos si el jugador la desmarca
            PlayerPrefs.SetInt(idMision, 0); // Guardamos que ya no está completada
        }

        // Guardamos la nueva puntuación global
        PlayerPrefs.SetInt("ScoreGlobal", scoreActual);
        PlayerPrefs.Save(); // Confirmamos el guardado

        // Mostramos la puntuación en la consola para que veas que funciona
        Debug.Log("Misión: " + idMision + " | Puntos totales: " + scoreActual);
    }
}