using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Nécessaire pour gérer l'Image UI

public class ARTapToPlace : MonoBehaviour
{
    [Header("AR Setup")]
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private Camera arCamera;

    [Header("Interface (UI)")]
    [SerializeField] private Image previewUI; // Glissez votre "PreviewImage" ici
    [SerializeField] private List<Sprite> graffitiSprites; // Liste des images (Mona Lisa, etc.)

    [Header("Bibliothèque de Graffitis")]
    [SerializeField] private List<GameObject> graffitiPrefabs; // Liste des objets 3D

    // Variables internes
    private GameObject currentObjectBeingDragged; // L'objet qu'on est en train de déplacer
    private int currentPrefabIndex = 0;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isDragging = false;

    private void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
        
        // Au démarrage, on initialise l'image (si on a des sprites)
        if (graffitiSprites.Count > 0 && previewUI != null)
        {
            UpdatePreviewImage();
        }
    }

    // On utilise Update pour gérer le glisser-déposer fluide
    private void Update()
    {
        // 1. Vérification de base (Y a-t-il un doigt sur l'écran ?)
        if (Touchscreen.current == null) return;
        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.isPressed)
        {
            Vector2 touchPos = touch.position.ReadValue();

            // CAS A : LE DÉBUT DU CLIC (On vient de poser le doigt)
            if (touch.press.wasPressedThisFrame)
            {
                // On vérifie SI on touche précisément l'image de prévisualisation
                if (IsTouchingPreviewImage(touchPos))
                {
                    isDragging = true;
                    SpawnObjectAt(touchPos); // On crée l'objet tout de suite
                }
            }

            // CAS B : ON EST EN TRAIN DE GLISSER (Le doigt bouge)
            if (isDragging && currentObjectBeingDragged != null)
            {
                MoveObjectTo(touchPos);
            }
        }
        else
        {
            // CAS C : ON RELACHE LE DOIGT
            if (isDragging)
            {
                // On arrête le drag. L'objet reste là où il est.
                isDragging = false;
                currentObjectBeingDragged = null;
            }
        }
    }

    // --- FONCTIONS DE LOGIQUE ---

    private void SpawnObjectAt(Vector2 screenPos)
    {
        if (graffitiPrefabs.Count == 0 || currentPrefabIndex >= graffitiPrefabs.Count) return;

        // On instancie l'objet
        currentObjectBeingDragged = Instantiate(graffitiPrefabs[currentPrefabIndex]);
        
        // On le place initialement (il risque d'être à zéro, mais sera corrigé dès la 1ère frame de mouvement)
        MoveObjectTo(screenPos);
    }

    private void MoveObjectTo(Vector2 screenPos)
    {
        // On lance le Raycast AR pour trouver le mur derrière le doigt
        if (arRaycastManager.Raycast(screenPos, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            
            // On met à jour la position de l'objet qu'on tient
            currentObjectBeingDragged.transform.position = hitPose.position + hitPose.up * 0.01f; // Petit offset
            currentObjectBeingDragged.transform.rotation = Quaternion.LookRotation(-hitPose.up, Vector3.up);
            
            // Optionnel : On s'assure qu'il est visible
            currentObjectBeingDragged.SetActive(true);
        }
        else
        {
            // Si le doigt n'est pas sur un plan (ex: dans le ciel), on peut masquer l'objet temporairement
            // currentObjectBeingDragged.SetActive(false);
        }
    }

    // --- FONCTIONS UI ---

    // Appelé par vos boutons (Mona Lisa, etc.)
    public void SetSelectedGraffiti(int index)
    {
        if (index >= 0 && index < graffitiPrefabs.Count)
        {
            currentPrefabIndex = index;
            UpdatePreviewImage();
        }
    }

    private void UpdatePreviewImage()
    {
        // Met à jour l'image en haut à droite
        if (previewUI != null && indexIsValid(currentPrefabIndex, graffitiSprites))
        {
            previewUI.sprite = graffitiSprites[currentPrefabIndex];
            
            // Si l'image était cachée, on l'affiche
            if (!previewUI.gameObject.activeSelf) previewUI.gameObject.SetActive(true);
        }
    }

    // Vérifie si le doigt touche spécifiquement l'image de preview
    private bool IsTouchingPreviewImage(Vector2 pos)
    {
        if (previewUI == null) return false;

        // On crée un pointeur virtuel pour tester l'UI
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // On regarde si dans tout ce qu'on touche, il y a notre PreviewImage
        foreach (var result in results)
        {
            if (result.gameObject == previewUI.gameObject) return true;
        }

        return false;
    }

    private bool indexIsValid<T>(int i, List<T> list)
    {
        return list != null && i >= 0 && i < list.Count;
    }
}