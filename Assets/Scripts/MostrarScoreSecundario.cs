using UnityEngine;
using TMPro; 

public class MostrarScoreSecundario : MonoBehaviour
{
    public TextMeshProUGUI textoScoreSecundario;

    void Update()
    {
        // Fíjate que aquí lee "ScoreSecundario" en lugar de "ScoreGlobal"
        int scoreActual = PlayerPrefs.GetInt("ScoreSecundario", 0);
        textoScoreSecundario.text = scoreActual.ToString();
    }
}
