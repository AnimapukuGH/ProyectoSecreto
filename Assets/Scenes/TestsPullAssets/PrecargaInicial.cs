using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PrecargadorInicial : MonoBehaviour
{
    [Header("1. Lista de escenas a precargar en memoria")]
    [Tooltip("Escribe los nombres EXACTOS de las escenas pesadas (ej: EscenaGacha)")]
    [SerializeField] private List<string> escenasAPrecargar = new List<string>();

    [Header("2. Escena a la que irá el juego al terminar de cargar")]
    [Tooltip("Nombre EXACTO de tu pantalla principal o Menú")]
    [SerializeField] private string escenaMenuPrincipal = "MenuPrincipal";

    [Header("3. Tiempo de espera tras terminar la carga")]
    [Tooltip("Segundos adicionales a esperar antes de cambiar de escena")]
    [SerializeField] private float tiempoEsperaExtra = 3f; // 👉 3 segundos de espera tras cargar todo

    private void Start()
    {
        StartCoroutine(CargarTodoAlInicio());
    }

    private IEnumerator CargarTodoAlInicio()
    {
        // Recorre y carga cada escena en segundo plano (modo Aditivo)
        foreach (string nombreEscena in escenasAPrecargar)
        {
            if (!string.IsNullOrEmpty(nombreEscena) && !SceneManager.GetSceneByName(nombreEscena).isLoaded)
            {
                AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscena, LoadSceneMode.Additive);

                // Espera a que la escena se llene en la memoria RAM/VRAM
                while (!operacion.isDone)
                {
                    yield return null;
                }

                // Oculta temporalmente los objetos de la escena cargada para que no se vean
                Scene escenaCargada = SceneManager.GetSceneByName(nombreEscena);
                if (escenaCargada.IsValid())
                {
                    foreach (GameObject objetoRaiz in escenaCargada.GetRootGameObjects())
                    {
                        objetoRaiz.SetActive(false);
                    }
                }
            }
        }

        // 👉 Espera los 3 segundos adicionales tras completar toda la carga
        yield return new WaitForSeconds(tiempoEsperaExtra);

        // Una vez transcurrido el tiempo, entra a la escena del Menú
        SceneManager.LoadScene(escenaMenuPrincipal);
    }
}