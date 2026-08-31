using UnityEngine;
using UnityEngine.UI;

public class GestorToggles : MonoBehaviour
{
    [Header("El MATEIX Identificador que a CargarFoto")]
    public string idUnicoFoto;

    [Header("Arrossega aquí els teus Toggles (Missions)")]
    public Toggle[] toggles;

    [Header("Arrossega aquí el Botó de la Foto")]
    public Button botoFoto;

    [Header("Quants Toggles calen per pujar la foto?")]
    public int togglesNecessaris = 2;

    void Update()
    {
        // 1. Busquem la memòria EXCLUSIVA d'aquesta escena
        string idRecompensa = "Recompensa_" + idUnicoFoto;

        if (PlayerPrefs.GetInt(idRecompensa, 0) == 1)
        {
            // Bloqueamos los clics de forma invisible añadiendo un CanvasGroup
            foreach (Toggle t in toggles)
            {
                // Busca si ya tiene un CanvasGroup, si no lo tiene, lo crea automáticamente
                CanvasGroup cg = t.GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    cg = t.gameObject.AddComponent<CanvasGroup>();
                }

                // Desactiva la detección del ratón (no se puede clicar)
                cg.blocksRaycasts = false;
            }

            this.enabled = false;
            return;
        }

        // 2. Si la foto NO s'ha pujat encara, comptem quants toggles estan marcats
        int togglesActius = 0;
        foreach (Toggle t in toggles)
        {
            if (t.isOn)
            {
                togglesActius++;
            }
        }

        // 3. Comparem amb la quantitat necessària que posis a l'Inspector
        if (togglesActius >= togglesNecessaris)
        {
            botoFoto.interactable = true;
        }
        else
        {
            botoFoto.interactable = false;
        }
    }
}