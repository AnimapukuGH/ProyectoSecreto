using UnityEngine;
using UnityEngine.UI;

public class PuntosToggle : MonoBehaviour
{
    [Header("Arrastra el Toggle aquí")]
    public Toggle miToggle;

    [Header("Configuración de Puntos")]
    public int puntosScorePrincipal = 30; // Puntos para el ScoreGlobal
    public int puntosScoreSecundario = 3; // Puntos para el nuevo Score

    void Start()
    {
        // Si se nos olvida arrastrarlo, lo busca automáticamente
        if (miToggle == null)
            miToggle = GetComponent<Toggle>();

        // Le decimos al Toggle que ejecute nuestra función cada vez que se haga clic
        miToggle.onValueChanged.AddListener(ActualizarPuntuaciones);
    }

    void ActualizarPuntuaciones(bool estaMarcado)
    {
        // 1. Leemos cómo están las dos puntuaciones ahora mismo en la memoria
        int scorePrincipal = PlayerPrefs.GetInt("ScoreGlobal", 0);
        int scoreSecundario = PlayerPrefs.GetInt("ScoreSecundario", 0);

        // 2. Si el jugador marca el Toggle, sumamos. Si lo desmarca, restamos.
        if (estaMarcado)
        {
            scorePrincipal += puntosScorePrincipal;
            scoreSecundario += puntosScoreSecundario;
        }
        else
        {
            scorePrincipal -= puntosScorePrincipal;
            scoreSecundario -= puntosScoreSecundario;
        }

        // 3. Guardamos los nuevos resultados en sus respectivas memorias
        PlayerPrefs.SetInt("ScoreGlobal", scorePrincipal);
        PlayerPrefs.SetInt("ScoreSecundario", scoreSecundario);
        
        // 4. Confirmamos los cambios
        PlayerPrefs.Save();
    }
}