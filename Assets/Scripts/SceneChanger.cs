using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void CambiarEscena(string Gat1)
    {
        // Usamos la variable, NO el nombre fijo
        SceneManager.LoadScene(Gat1);
    }
}