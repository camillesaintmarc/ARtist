using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    [Header("Scripts à contrôler")]
    public ARTapToPlace tapToPlaceScript;
    public ARPainter painterScript;

    private void Start()
    {
        // Au démarrage, on active par exemple le mode Placement par défaut
        ActivatePlacementMode();
    }

    // Appelez cette fonction via le bouton "Mode Placement"
    public void ActivatePlacementMode()
    {
        Debug.Log("Mode Placement Activé");
        
        // On active le script de placement
        if(tapToPlaceScript != null) tapToPlaceScript.enabled = true;
        
        // On désactive le script de peinture
        if(painterScript != null) painterScript.enabled = false;
    }

    // Appelez cette fonction via le bouton "Mode Peinture"
    public void ActivatePaintingMode()
    {
        Debug.Log("Mode Peinture Activé");

        // On désactive le script de placement
        if(tapToPlaceScript != null) tapToPlaceScript.enabled = false;

        // On active le script de peinture
        if(painterScript != null) painterScript.enabled = true;
    }
}