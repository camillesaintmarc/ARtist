using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ARTapToPlace : MonoBehaviour
{
    [Header("AR Setup")]
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private Camera arCamera;

    [Header("Interface (UI)")]
    [SerializeField] private Image previewUI; 
    [SerializeField] private List<Sprite> graffitiSprites; 

    [Header("Bibliothèque de Graffitis")]
    [SerializeField] private List<GameObject> graffitiPrefabs; 
    
    // NOUVEAU : Le filtre pour savoir quels objets on peut attraper
    [Header("Interaction")]
    [SerializeField] private LayerMask graffitiLayer; 

    // Variables internes
    private GameObject currentObjectBeingDragged; 
    private int currentPrefabIndex = 0;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isDragging = false;
    private bool isNewObject = false; // Pour savoir si on vient de le créer ou si on le déplace

    private void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
    }

    private void Update()
    {
        if (Touchscreen.current == null) return;
        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.isPressed)
        {
            Vector2 touchPos = touch.position.ReadValue();

            // CAS A : DÉBUT DU CLIC
            if (touch.press.wasPressedThisFrame)
            {
                // 1. Est-ce qu'on touche l'UI (le bouton d'aperçu) ? -> CRÉATION
                if (IsTouchingPreviewImage(touchPos))
                {
                    isDragging = true;
                    isNewObject = true;
                    SpawnObjectAt(touchPos);
                }
                // 2. Sinon, est-ce qu'on touche un graffiti existant ? -> DÉPLACEMENT
                else if (TryPickUpObject(touchPos))
                {
                    isDragging = true;
                    isNewObject = false;
                    // On ne fait rien d'autre, 'currentObjectBeingDragged' est rempli par TryPickUpObject
                }
            }

            // CAS B : PENDANT LE GLISSEMENT
            if (isDragging && currentObjectBeingDragged != null)
            {
                MoveObjectTo(touchPos);
            }
        }
        else
        {
            // CAS C : RELÂCHEMENT
            if (isDragging)
            {
                isDragging = false;
                
                // Nettoyage seulement si c'était un NOUVEL objet qu'on a lâché dans le vide
                if (isNewObject && currentObjectBeingDragged != null && !currentObjectBeingDragged.activeSelf)
                {
                    Destroy(currentObjectBeingDragged);
                }
                
                currentObjectBeingDragged = null;
            }
        }
    }

    // --- LOGIQUE ---

    private bool TryPickUpObject(Vector2 screenPos)
    {
        // On lance un rayon physique (comme un laser) depuis la caméra
        Ray ray = arCamera.ScreenPointToRay(screenPos);
        
        // On vérifie si ce rayon touche un objet qui a le bon Layer (Graffiti)
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, graffitiLayer))
        {
            // On a touché quelque chose ! On le prend en main.
            currentObjectBeingDragged = hit.collider.gameObject;
            return true;
        }

        return false;
    }

    private void SpawnObjectAt(Vector2 screenPos)
    {
        if (graffitiPrefabs.Count == 0 || currentPrefabIndex >= graffitiPrefabs.Count) return;

        currentObjectBeingDragged = Instantiate(graffitiPrefabs[currentPrefabIndex]);
        
        // Important : On assigne le Layer au nouvel objet pour qu'on puisse le retoucher plus tard !
        // (Au cas où vous auriez oublié de le mettre sur le prefab)
        SetLayerRecursively(currentObjectBeingDragged, GetLayerFromMask(graffitiLayer));

        currentObjectBeingDragged.SetActive(false); 
        MoveObjectTo(screenPos);
    }

    private void MoveObjectTo(Vector2 screenPos)
    {
        if (arRaycastManager.Raycast(screenPos, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            
            currentObjectBeingDragged.transform.position = hitPose.position + hitPose.up * 0.01f;
            currentObjectBeingDragged.transform.rotation = Quaternion.LookRotation(-hitPose.up, Vector3.up);
            
            if (!currentObjectBeingDragged.activeSelf) currentObjectBeingDragged.SetActive(true);
        }
        else
        {
            // Si on sort du mur avec un NOUVEL objet, on le cache
            if (isNewObject && currentObjectBeingDragged.activeSelf)
            {
                currentObjectBeingDragged.SetActive(false);
            }
            // Note : Si c'est un ANCIEN objet qu'on déplace, on le laisse visible à sa dernière position
            // même si le doigt part dans le ciel, c'est plus naturel.
        }
    }

    // --- FONCTIONS UI & UTILITAIRES ---

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
        if (previewUI != null && indexIsValid(currentPrefabIndex, graffitiSprites))
        {
            previewUI.sprite = graffitiSprites[currentPrefabIndex];
            if (!previewUI.gameObject.activeSelf) previewUI.gameObject.SetActive(true);
        }
    }

    private bool IsTouchingPreviewImage(Vector2 pos)
    {
        if (previewUI == null) return false;
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results) { if (result.gameObject == previewUI.gameObject) return true; }
        return false;
    }

    private bool indexIsValid<T>(int i, List<T> list) => list != null && i >= 0 && i < list.Count;

    // Petite astuce pour récupérer l'index du Layer depuis le LayerMask
    private int GetLayerFromMask(LayerMask mask)
    {
        int layerNumber = 0;
        int layer = mask.value;
        while(layer > 0) { layer >>= 1; layerNumber++; }
        return layerNumber - 1;
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (newLayer < 0) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }
}