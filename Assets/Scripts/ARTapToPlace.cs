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

    // Variable pour suivre la position du toucher (si nécessaire)
    private Vector2 touchPosition; 

    // Assurez-vous d'utiliser l'EnhancedTouchSupport
    private void OnEnable()
    {
        // Active le support des touchers avancés (nécessaire avec le New Input System pour AR)
        EnhancedTouchSupport.Enable(); 
        
        // Configuration simple avec Input System (vous pouvez utiliser un Asset d'Action)
        // Exemple basique utilisant l'API Touchscreen
        touchPressAction = new InputAction("TouchPress", binding: "<Touchscreen>/primaryTouch/press");
        touchPositionAction = new InputAction("TouchPosition", binding: "<Touchscreen>/primaryTouch/position");

        touchPressAction.Enable();
        touchPositionAction.Enable();

        // Écoutez l'événement de pression (quand le doigt touche l'écran)
        touchPressAction.performed += OnTouchPressed;
        // Mettez à jour la position de contact 
        touchPositionAction.performed += ctx => touchPosition = ctx.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        // Désactivez les actions et le support
        touchPressAction.performed -= OnTouchPressed;
        touchPressAction.Disable();
        touchPositionAction.Disable();
        
        EnhancedTouchSupport.Disable();
    }

    private void OnTouchPressed(InputAction.CallbackContext context)
    {
        // La position du toucher est stockée dans la variable 'touchPosition'
        if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            // Le raycast a réussi et a touché un 'Trackable' (ici un plan détecté)

            // 'hits[0]' contient l'information du point le plus proche
            var hitPose = hits[0].pose;

            // Instancier le cube au point de contact
            Instantiate(objectToPlacePrefab, hitPose.position, hitPose.rotation);
        }
    }
}