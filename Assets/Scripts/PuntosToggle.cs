using UnityEngine;
using UnityEngine.UI;

public class PuntosToggle : MonoBehaviour
{
    [Header("Identificador Único (¡Importante!)")]
    public string idMision; // Ponle un nombre distinto a cada Toggle en el Inspector (ej: "Mision1", "Mision2")

    [Header("Configuración")]
    public Toggle miToggle;
    public int puntosScorePrincipal = 30;
    public int puntosScoreSecundario = 3;

    void Start()
    {
        if (miToggle == null)
            miToggle = GetComponent<Toggle>();

        // 1. Leemos si esta misión ya estaba hecha (0 = No, 1 = Sí)
        bool estabaMarcada = PlayerPrefs.GetInt(idMision, 0) == 1;

        // 2. Activamos/Desactivamos el Toggle SIN sumar ni restar puntos extra al iniciar
        miToggle.SetIsOnWithoutNotify(estabaMarcada);

        // 3. Activamos el detector de clics para cuando el jugador lo pulse
        miToggle.onValueChanged.AddListener(ActualizarPuntuaciones);
    }

    void ActualizarPuntuaciones(bool estaMarcado)
    {
        int scorePrincipal = PlayerPrefs.GetInt("ScoreGlobal", 0);
        int scoreSecundario = PlayerPrefs.GetInt("ScoreSecundario", 0);

        if (estaMarcado)
        {
            scorePrincipal += puntosScorePrincipal;
            scoreSecundario += puntosScoreSecundario;
            PlayerPrefs.SetInt(idMision, 1); // Guardamos que la misión está hecha
        }
        else
        {
            scorePrincipal -= puntosScorePrincipal;
            scoreSecundario -= puntosScoreSecundario;
            PlayerPrefs.SetInt(idMision, 0); // Guardamos que la misión se ha desmarcado
        }

        PlayerPrefs.SetInt("ScoreGlobal", scorePrincipal);
        PlayerPrefs.SetInt("ScoreSecundario", scoreSecundario);
        PlayerPrefs.Save();
    }
}