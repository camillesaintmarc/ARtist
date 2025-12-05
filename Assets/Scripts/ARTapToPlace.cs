using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;

public class ARTapToPlace : MonoBehaviour
{
    // Références à assigner dans l'Inspector
    [SerializeField]
    private ARRaycastManager arRaycastManager;
    [SerializeField]
    private GameObject objectToPlacePrefab;

    // Liste pour stocker les résultats du raycast
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    // Référence à l'action de l'Input System pour le toucher
    private InputAction touchPositionAction;
    private InputAction touchPressAction; 

    // Variable pour suivre la position du toucher
    private Vector2 touchPosition; 

    private void OnEnable()
    {
        // Active le support des touchers avancés
        EnhancedTouchSupport.Enable(); 
        
        // Configuration des inputs
        touchPressAction = new InputAction("TouchPress", binding: "<Touchscreen>/primaryTouch/press");
        touchPositionAction = new InputAction("TouchPosition", binding: "<Touchscreen>/primaryTouch/position");

        touchPressAction.Enable();
        touchPositionAction.Enable();

        // Abonnements aux événements
        touchPressAction.performed += OnTouchPressed;
        touchPositionAction.performed += ctx => touchPosition = ctx.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        // Désabonnement et nettoyage
        touchPressAction.performed -= OnTouchPressed;
        touchPressAction.Disable();
        touchPositionAction.Disable();
        
        EnhancedTouchSupport.Disable();
    }

    private void OnTouchPressed(InputAction.CallbackContext context)
    {
        // Raycast depuis la position du toucher vers les plans détectés
        if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            // Récupère la pose (position + rotation) du plan touché
            var hitPose = hits[0].pose;

            // --- CORRECTION ---
            // Le Quad d'Unity est vertical par défaut. 
            // hitPose.rotation aligne l'objet avec la normale du plan (vers le haut).
            // On ajoute une rotation de 90 degrés sur l'axe X pour le coucher à plat.
            Quaternion rotationFinale = hitPose.rotation * Quaternion.Euler(90, 0, 0);

            // Instancier le prefab avec la nouvelle rotation calculée
            Instantiate(objectToPlacePrefab, hitPose.position, rotationFinale);
        }
    }
}