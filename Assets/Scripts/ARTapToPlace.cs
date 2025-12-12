using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;

public class ARTapToPlace : MonoBehaviour
{
    [Header("AR Setup")]
    [SerializeField]
    private ARRaycastManager arRaycastManager;
    [SerializeField]
    private GameObject objectToPlacePrefab;
    [SerializeField]
    private Camera arCamera; // Assignez votre "AR Camera" ici

    [Header("Spawn Settings")]
    [SerializeField]
    private bool m_OnlySpawnInView = true;
    [SerializeField, Range(0f, 0.5f)]
    private float m_ViewportPeriphery = 0.1f; // Marge de sécurité sur les bords (10%)
    [SerializeField]
    private bool m_SpawnAsChildren = false;

    [Header("Bibliothèque de Graffitis")]
    // REMPLACEZ 'private GameObject objectToPlacePrefab;' PAR CECI :
    [SerializeField]
    private List<GameObject> graffitiPrefabs = new List<GameObject>(); // Une liste !

    private int currentPrefabIndex = 0; // Lequel est sélectionné (0 par défaut)

    // Liste pour stocker les résultats du raycast
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Input System
    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    private Vector2 touchPosition;

    private void Awake()
    {
        // Si la caméra n'est pas assignée manuellement, on cherche la caméra principale
        if (arCamera == null) arCamera = Camera.main;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        touchPressAction = new InputAction("TouchPress", binding: "<Touchscreen>/primaryTouch/press");
        touchPositionAction = new InputAction("TouchPosition", binding: "<Touchscreen>/primaryTouch/position");

        touchPressAction.Enable();
        touchPositionAction.Enable();

        touchPressAction.performed += OnTouchPressed;
        touchPositionAction.performed += ctx => touchPosition = ctx.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        touchPressAction.performed -= OnTouchPressed;
        touchPressAction.Disable();
        touchPositionAction.Disable();

        EnhancedTouchSupport.Disable();
    }

    private void OnTouchPressed(InputAction.CallbackContext context)
    {
        // 1. Protection UI : Si on touche un bouton, on ne fait rien
        if (EventSystem.current.IsPointerOverGameObject(context.control.device.deviceId))
        {
            return; 
        }
        // Raycast depuis la position du toucher
        if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            
            // Dans ARFoundation, hitPose.up correspond à la normale du plan
            Vector3 spawnNormal = hitPose.up;
            Vector3 spawnPoint = hitPose.position;

            // Appel de la logique personnalisée
            TrySpawnObject(spawnPoint, spawnNormal);
        }
    }

    // VOTRE LOGIQUE IMPLÉMENTÉE ICI
    public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
    {
        // 1. Vérification si le point est dans le champ de vision (Viewport)
        if (m_OnlySpawnInView)
        {
            var inViewMin = m_ViewportPeriphery;
            var inViewMax = 1f - m_ViewportPeriphery;
            
            // Conversion World -> Viewport
            var pointInViewportSpace = arCamera.WorldToViewportPoint(spawnPoint);

            // Vérification des limites (z < 0 signifie derrière la caméra)
            if (pointInViewportSpace.z < 0f || 
                pointInViewportSpace.x > inViewMax || pointInViewportSpace.x < inViewMin ||
                pointInViewportSpace.y > inViewMax || pointInViewportSpace.y < inViewMin)
            {
                return false; // Hors du champ de vision défini
            }
        }

        // 2. Instanciation
        // On vérifie que la liste n'est pas vide
        if (graffitiPrefabs.Count == 0) return false;

        // On instancie l'objet correspondant à l'index choisi
        var newObject = Instantiate(graffitiPrefabs[currentPrefabIndex]);

        // ... (Le reste : parent, position, rotation reste pareil) ...
        if (m_SpawnAsChildren) newObject.transform.parent = transform;
        newObject.transform.position = spawnPoint + spawnNormal * 0.02f;
        newObject.transform.rotation = Quaternion.LookRotation(spawnNormal * -1, Vector3.up);

        return true;
    }


    // Elle sera appelée par vos boutons d'interface (Graffiti 1, Graffiti 2, etc.)
    public void SetSelectedGraffiti(int index)
    {
        if (index >= 0 && index < graffitiPrefabs.Count)
        {
            currentPrefabIndex = index;
            Debug.Log("Graffiti sélectionné : " + index);
        }
        else
        {
            Debug.LogError("Index invalide ! Vérifiez le nombre de prefabs dans la liste.");
        }
    }




}