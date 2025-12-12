using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    [Header("Scripts à contrôler")]
    public ARTapToPlace tapToPlaceScript;
    public ARPainter painterScript;

    [Header("Interface Graffiti")]
    public GameObject graffitiSelectionPanel; // Le panneau avec les boutons Mona Lisa, etc.
    public GameObject previewImageObject;     // L'image en haut à droite

    [Header("Interface Peinture")]
    public GameObject colorSelectionPanel;    // NOUVEAU : Le panneau avec les boutons de couleur

    private void Start()
    {
        // Au démarrage, on cache TOUT pour avoir un écran propre
        if (tapToPlaceScript != null) tapToPlaceScript.enabled = false;
        if (painterScript != null) painterScript.enabled = false;

        // On cache tous les menus
        if (graffitiSelectionPanel != null) graffitiSelectionPanel.SetActive(false);
        if (previewImageObject != null) previewImageObject.SetActive(false);
        if (colorSelectionPanel != null) colorSelectionPanel.SetActive(false);
    }

    // Appelé par le bouton "Place Graffiti"
    public void ActivatePlacementMode()
    {
        // 1. Gérer les scripts
        if(tapToPlaceScript != null) tapToPlaceScript.enabled = true;
        if(painterScript != null) painterScript.enabled = false;

        // 2. Gérer l'interface : On AFFICHE Graffiti, on CACHE Peinture
        if(graffitiSelectionPanel != null) graffitiSelectionPanel.SetActive(true);
        if(previewImageObject != null) previewImageObject.SetActive(true);
        
        if(colorSelectionPanel != null) colorSelectionPanel.SetActive(false); // On cache les couleurs
    }

    // Appelé par le bouton "AR Paint"
    public void ActivatePaintingMode()
    {
        // 1. Gérer les scripts
        if(tapToPlaceScript != null) tapToPlaceScript.enabled = false;
        if(painterScript != null) painterScript.enabled = true;

        // 2. Gérer l'interface : On CACHE Graffiti, on AFFICHE Peinture
        if(graffitiSelectionPanel != null) graffitiSelectionPanel.SetActive(false);
        if(previewImageObject != null) previewImageObject.SetActive(false);

        if(colorSelectionPanel != null) colorSelectionPanel.SetActive(true); // On affiche les couleurs
    }
}