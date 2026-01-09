using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections.Generic;
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
    
    // Référence optionnelle pour changer la couleur du bouton poubelle
    [SerializeField] private Image deleteButtonImage; 
    [SerializeField] private Color deleteActiveColor = Color.red;
    [SerializeField] private Color deleteInactiveColor = Color.white;

    [Header("Bibliothèque de Graffitis")]
    [SerializeField] private List<GameObject> graffitiPrefabs;

    [Header("Interaction")]
    [SerializeField] private LayerMask graffitiLayer;

    [Header("Scaling")]
    [SerializeField] private float scaleSpeed = 0.005f;
    [SerializeField] private float minScale = 0.05f;
    [SerializeField] private float maxScale = 1.5f;

    // Internal
    private GameObject currentObjectBeingDragged;
    private int currentPrefabIndex = 0;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isDragging = false;
    private bool isNewObject = false;
    private float previousPinchDistance = 0f;

    // Mode Suppression
    private bool isDeleteMode = false;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
    }

    private void Update()
    {
        if (Touchscreen.current == null) return;

        // =========================
        // PINCH TO SCALE (2 doigts)
        // =========================
        // Désactivé en mode suppression pour éviter les accidents
        if (!isDeleteMode && currentObjectBeingDragged != null &&
            TryGetTwoPressedTouches(out TouchControl t0, out TouchControl t1))
        {
            HandlePinchScale(t0, t1);
            return; 
        }

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.isPressed)
        {
            Vector2 touchPos = touch.position.ReadValue();

            // ---- DÉBUT DU CLIC ----
            if (touch.press.wasPressedThisFrame)
            {
                // PRIORITÉ 1 : Est-ce qu'on touche l'image de prévisualisation ? (Spawn)
                // On vérifie ça AVANT de bloquer l'UI globale
                if (IsTouchingPreviewImage(touchPos))
                {
                    // Si on est en mode suppression, on l'annule peut-être ? 
                    // Pour l'instant on force le spawn
                    isDeleteMode = false; // Optionnel : désactive la poubelle si on spawn un nouveau
                    UpdateDeleteButtonVisual();
                    
                    isDragging = true;
                    isNewObject = true;
                    SpawnObjectAt(touchPos);
                    return; // On arrête là, on a trouvé notre action
                }

                // PRIORITÉ 2 : Est-ce qu'on touche un bouton (Poubelle, Menu, etc.) ?
                if (IsPointerOverUI(touch)) return; 

                // PRIORITÉ 3 : Logique 3D (Monde réel)
                
                // A. MODE SUPPRESSION
                if (isDeleteMode)
                {
                    if (TryPickUpObject(touchPos)) 
                    {
                        Destroy(currentObjectBeingDragged);
                        currentObjectBeingDragged = null;
                    }
                    return; 
                }

                // B. MODE NORMAL (Déplacer existant)
                if (TryPickUpObject(touchPos))
                {
                    isDragging = true;
                    isNewObject = false;
                }
            }

            // ---- DRAG (Seulement si pas en mode suppression) ----
            if (!isDeleteMode && isDragging && currentObjectBeingDragged != null)
            {
                MoveObjectTo(touchPos);
            }
        }
        else
        {
            // ---- RELACHEMENT ----
            if (isDragging)
            {
                isDragging = false;
                previousPinchDistance = 0f;

                // Si on relâche un NOUVEL objet dans le vide (pas de mur trouvé), on le supprime
                if (isNewObject &&
                    currentObjectBeingDragged != null &&
                    !currentObjectBeingDragged.activeSelf)
                {
                    Destroy(currentObjectBeingDragged);
                }

                currentObjectBeingDragged = null;
            }
        }
    }

    // =========================
    // TOGGLE DELETE MODE
    // =========================
    public void ToggleDeleteMode()
    {
        isDeleteMode = !isDeleteMode;
        UpdateDeleteButtonVisual();

        // Reset du drag si on change de mode
        isDragging = false;
        currentObjectBeingDragged = null;
    }

    private void UpdateDeleteButtonVisual()
    {
        if (deleteButtonImage != null)
        {
            deleteButtonImage.color = isDeleteMode ? deleteActiveColor : deleteInactiveColor;
        }
    }

    // =========================
    // SCALING
    // =========================
    private bool TryGetTwoPressedTouches(out TouchControl t0, out TouchControl t1)
    {
        t0 = null; t1 = null;
        int count = 0;
        foreach (var touch in Touchscreen.current.touches)
        {
            if (touch.press.isPressed)
            {
                if (count == 0) t0 = touch;
                else if (count == 1) t1 = touch;
                count++;
                if (count >= 2) return true;
            }
        }
        return false;
    }

    private void HandlePinchScale(TouchControl touch0, TouchControl touch1)
    {
        float currentDistance = Vector2.Distance(touch0.position.ReadValue(), touch1.position.ReadValue());

        if (previousPinchDistance == 0f)
        {
            previousPinchDistance = currentDistance;
            return;
        }

        float delta = currentDistance - previousPinchDistance;
        float scaleFactor = 1f + delta * scaleSpeed;

        Vector3 newScale = currentObjectBeingDragged.transform.localScale * scaleFactor;
        float clampedScale = Mathf.Clamp(newScale.x, minScale, maxScale);

        // On applique le scale uniforme
        currentObjectBeingDragged.transform.localScale = new Vector3(clampedScale, clampedScale, 1f); 
        // Note: Si vos graffitis sont 3D, mettez 'Vector3.one * clampedScale'

        previousPinchDistance = currentDistance;
    }

    // =========================
    // PICK UP
    // =========================
    private bool TryPickUpObject(Vector2 screenPos)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, graffitiLayer))
        {
            currentObjectBeingDragged = hit.collider.gameObject;
            return true;
        }
        return false;
    }

    // =========================
    // SPAWN
    // =========================
    private void SpawnObjectAt(Vector2 screenPos)
    {
        if (graffitiPrefabs.Count == 0 || currentPrefabIndex >= graffitiPrefabs.Count) return;

        currentObjectBeingDragged = Instantiate(graffitiPrefabs[currentPrefabIndex]);
        SetLayerRecursively(currentObjectBeingDragged, GetLayerFromMask(graffitiLayer));
        
        // Ajustement Ratio (Anti-étirement)
        AdjustAspectRatio(currentObjectBeingDragged, currentPrefabIndex);

        currentObjectBeingDragged.SetActive(false);
        MoveObjectTo(screenPos);
    }

    private void AdjustAspectRatio(GameObject obj, int index)
    {
        if (index < 0 || index >= graffitiSprites.Count) return;
        
        Sprite sprite = graffitiSprites[index];
        float width = sprite.rect.width;
        float height = sprite.rect.height;
        Vector3 newScale = Vector3.one;

        // Si l'image est plus large que haute
        if (width > height)
            newScale.y = height / width;
        // Si l'image est plus haute que large
        else
            newScale.x = width / height;

        obj.transform.localScale = newScale;
    }

    // =========================
    // MOVE
    // =========================
    private void MoveObjectTo(Vector2 screenPos)
    {
        if (arRaycastManager.Raycast(screenPos, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            currentObjectBeingDragged.transform.position = hitPose.position + hitPose.up * 0.05f;
            currentObjectBeingDragged.transform.rotation = Quaternion.LookRotation(-hitPose.up, Vector3.up);

            if (!currentObjectBeingDragged.activeSelf) currentObjectBeingDragged.SetActive(true);
        }
        else
        {
            if (isNewObject && currentObjectBeingDragged.activeSelf) currentObjectBeingDragged.SetActive(false);
        }
    }

    // =========================
    // UI
    // =========================
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
        if (previewUI != null && graffitiSprites != null && currentPrefabIndex < graffitiSprites.Count)
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

        foreach (var result in results)
        {
            if (result.gameObject == previewUI.gameObject) return true;
        }
        return false;
    }

    private bool IsPointerOverUI(TouchControl touch)
    {
        int id = touch.touchId.ReadValue();
        return EventSystem.current.IsPointerOverGameObject(id);
    }

    // =========================
    // UTILS
    // =========================
    private int GetLayerFromMask(LayerMask mask)
    {
        int layerNumber = 0;
        int layer = mask.value;
        while (layer > 0) { layer >>= 1; layerNumber++; }
        return layerNumber - 1;
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (newLayer < 0) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }
}