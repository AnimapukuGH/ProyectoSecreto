using UnityEngine;
using UnityEngine.SceneManagement; // Esta línea es obligatoria para cambiar escenas

public class SceneChanger : MonoBehaviour
{
    // Esta función debe ser 'public' para que el botón pueda verla
    public void CambiarEscena(string nombreDeLaEscena)
    {
        SceneManager.LoadScene(Gat1);
    }
    
}