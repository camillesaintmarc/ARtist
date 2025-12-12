using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPainter : MonoBehaviour
{
    [Header("Références")]
    public Camera arCamera;
    public GameObject linePrefab;
    public ARPlaneManager arPlaneManager;

    [Header("Paramètres du trait")]
    public float minDistanceBetweenPoints = 0.01f;
    public float lineOffset = 0.01f;
    public TrackableType trackableTypes = TrackableType.PlaneWithinPolygon;

    // --- NOUVEAU : Couleur par défaut ---
    private Color currentColor = Color.black; 

    ARRaycastManager raycastManager;
    GameObject currentLine;
    LineRenderer currentLineRenderer;
    List<Vector3> currentPoints = new List<Vector3>();

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        if (arCamera == null) arCamera = Camera.main;
    }

    void Update()
    {
        if (Touchscreen.current == null) return;
        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.isPressed)
        {
            if (IsPointerOverUI(touch)) return; 
            Vector2 screenPos = touch.position.ReadValue();
            HandleTouchHold(screenPos);
        }
        else
        {
            if (currentLine != null) FinishLine();
        }
    }

    void HandleTouchHold(Vector2 screenPos)
    {
        if (raycastManager.Raycast(screenPos, s_Hits, trackableTypes))
        {
            var hit = s_Hits[0];
            Vector3 hitPos = hit.pose.position;
            
            ARPlane plane = null;
            if (arPlaneManager != null) plane = arPlaneManager.GetPlane(hit.trackableId);

            Vector3 normal = (plane != null) ? plane.transform.up : hit.pose.rotation * Vector3.up;
            Vector3 finalPos = hitPos + normal * lineOffset;

            if (currentLine == null)
            {
                StartNewLine(finalPos, hit);
            }
            else
            {
                if (currentPoints.Count == 0 || Vector3.Distance(currentPoints[currentPoints.Count - 1], finalPos) >= minDistanceBetweenPoints)
                {
                    AddPoint(finalPos);
                }
            }
        }
    }

    void StartNewLine(Vector3 startPos, ARRaycastHit hit)
    {
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        currentLineRenderer = currentLine.GetComponent<LineRenderer>();
        
        if (currentLineRenderer == null)
        {
            Debug.LogError("LinePrefab n'a pas de LineRenderer");
            Destroy(currentLine);
            currentLine = null;
            return;
        }

        // --- NOUVEAU : Application de la couleur ---
        // On change la couleur de début et de fin du trait
        currentLineRenderer.startColor = currentColor;
        currentLineRenderer.endColor = currentColor;

        // Astuce : Pour que la couleur marche, le matériau du LineRenderer doit être compatible.
        // On force souvent le material color aussi par sécurité.
        currentLineRenderer.material.color = currentColor; 
        // -------------------------------------------

        currentPoints.Clear();
        AddPoint(startPos);
    }

    void AddPoint(Vector3 worldPos)
    {
        currentPoints.Add(worldPos);
        currentLineRenderer.positionCount = currentPoints.Count;
        currentLineRenderer.SetPositions(currentPoints.ToArray());
    }

    void FinishLine()
    {
        currentLine = null;
        currentLineRenderer = null;
        currentPoints.Clear();
    }

    public void ClearAllLines()
    {
        foreach (var lr in FindObjectsOfType<LineRenderer>()) Destroy(lr.gameObject);
    }

    private bool IsPointerOverUI(UnityEngine.InputSystem.Controls.TouchControl touch)
    {
        int touchId = touch.touchId.ReadValue();
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touchId);
    }

    // --- NOUVELLE FONCTION PUBLIQUE POUR LES BOUTONS ---
    public void SetBrushColor(Color newColor)
    {
        currentColor = newColor;
    }
    
    // Surcharge pratique si vous voulez utiliser des boutons Unity standard avec des presets string (ex: "red")
    public void SetBrushColorByName(string colorName)
    {
        if(ColorUtility.TryParseHtmlString(colorName, out Color c))
        {
            currentColor = c;
        }
    }
}