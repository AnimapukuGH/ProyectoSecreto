using UnityEngine;
using UnityEngine.UI;

public class PuntosToggle : MonoBehaviour
{
    [Header("Identificador Único (¡Importante!)")]
    public string idMision;

    [Header("Configuración")]
    public Toggle miToggle;
    public int puntosScorePrincipal = 30;
    public int puntosScoreSecundario = 3;

    private bool candado = true; // Bloqueado por defecto al iniciar

    void Start()
    {
        if (miToggle == null)
            miToggle = GetComponent<Toggle>();

        bool estabaMarcada = PlayerPrefs.GetInt(idMision, 0) == 1;

        miToggle.SetIsOnWithoutNotify(estabaMarcada);
        miToggle.onValueChanged.AddListener(ActualizarPuntuaciones);

        // Quitamos el candado medio segundo después de cargar la escena
        Invoke("DesbloquearMatematicas", 0.5f);
    }

    void DesbloquearMatematicas()
    {
        candado = false;
    }

    void ActualizarPuntuaciones(bool estaMarcado)
    {
        // Si el candado sigue activo (escena cargando), cancelamos la suma
        if (candado) return;

        int scorePrincipal = PlayerPrefs.GetInt("ScoreGlobal", 0);
        int scoreSecundario = PlayerPrefs.GetInt("ScoreSecundario", 0);

        if (estaMarcado)
        {
            scorePrincipal += puntosScorePrincipal;
            scoreSecundario += puntosScoreSecundario;
            PlayerPrefs.SetInt(idMision, 1);
        }
        else
        {
            scorePrincipal -= puntosScorePrincipal;
            scoreSecundario -= puntosScoreSecundario;
            PlayerPrefs.SetInt(idMision, 0);
        }

        PlayerPrefs.SetInt("ScoreGlobal", scorePrincipal);
        PlayerPrefs.SetInt("ScoreSecundario", scoreSecundario);
        PlayerPrefs.Save();
    }
}
