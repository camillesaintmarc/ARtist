using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public ARTapToPlace tapToPlaceScript;
    public ARPainter painterScript;

    [Header("Interface")]
    public GameObject graffitiSelectionPanel; // Le panneau contenant les choix de graffitis


    private void Start()
    {
        // Au démarrage, on s'assure que tout est éteint
        Debug.Log("Démarrage : Aucun mode activé");

        if (tapToPlaceScript != null) 
            tapToPlaceScript.enabled = false;

        if (painterScript != null) 
            painterScript.enabled = false;

        // On cache aussi le panneau de choix de graffitis s'il est assigné
        if (graffitiSelectionPanel != null) 
            graffitiSelectionPanel.SetActive(false);
    }

    public void ActivatePlacementMode()
    {
        tapToPlaceScript.enabled = true;
        painterScript.enabled = false;

        // On affiche le menu de choix
        if(graffitiSelectionPanel != null) graffitiSelectionPanel.SetActive(true);
    }

    public void ActivatePaintingMode()
    {
        tapToPlaceScript.enabled = false;
        painterScript.enabled = true;

        // On cache le menu de choix car on veut peindre
        if(graffitiSelectionPanel != null) graffitiSelectionPanel.SetActive(false);
    }
}