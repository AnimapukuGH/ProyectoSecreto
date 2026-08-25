using UnityEngine;
using UnityEngine.UI; // Necessari per controlar Toggles i Botons

public class GestorToggles : MonoBehaviour
{
    [Header("Arrossega aquí els teus Toggles (Missions)")]
    public Toggle[] toggles;

    [Header("Arrossega aquí el Botó de la Foto")]
    public Button botoFoto;

    // Utilitzem Update perquè el joc vigili constantment què està passant
    void Update()
    {
        // 1. Mirem si la foto ja està pujada (és a dir, si ja va cobrar la recompensa de 100 punts)
        if (PlayerPrefs.GetInt("RecompensaFoto", 0) == 1)
        {
            // Bloquegem tots els toggles perquè no es puguin clicar més
            foreach (Toggle t in toggles)
            {
                t.interactable = false;
            }

            // Com que ja estan bloquejats, apaguem aquest script perquè no consumeixi rendiment
            this.enabled = false;
            return; // Sortim de la funció
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

        // 3. Si n'hi ha 3 (o més) actius, activem el botó. Si no, el mantenim desactivat.
        if (togglesActius >= 3)
        {
            botoFoto.interactable = true;
        }
        else
        {
            botoFoto.interactable = false;
        }
    }
}