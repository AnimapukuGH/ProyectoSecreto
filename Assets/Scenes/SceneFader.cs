using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;

    [Header("Tiempos de Transición")]
    public float fadeInDuration = 1.0f;  // Tiempo al ENTRAR a la escena
    public float fadeOutDuration = 1.0f; // Tiempo al SALIR de la escena

    void Start()
    {
        // Asegura que empiece visible y se aclare
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(1, 1, 1, 1);
        StartCoroutine(FadeInRoutine());
    }

    public void FadeToScene(string sceneName)
    {
        // Detiene otros fundidos por si acaso y arranca el de salida
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(sceneName));
    }

    IEnumerator FadeInRoutine()
    {
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            // Evita pasar de 1 en la división
            float progress = Mathf.Clamp01(timer / fadeInDuration);
            fadeImage.color = new Color(1, 1, 1, 1f - progress);
            yield return null;
        }

        fadeImage.color = new Color(1, 1, 1, 0);
        fadeImage.gameObject.SetActive(false);
    }

    IEnumerator FadeOutRoutine(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeOutDuration);
            fadeImage.color = new Color(1, 1, 1, progress);
            yield return null;
        }

        fadeImage.color = new Color(1, 1, 1, 1);

        // Pequeña pausa de seguridad antes de cargar para congelar el blanco puro
        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene(sceneName);
    }
}
