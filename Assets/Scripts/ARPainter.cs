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

    private Color currentColor = Color.black; 

    ARRaycastManager raycastManager;
    GameObject currentLine;
    LineRenderer currentLineRenderer;
    List<Vector3> currentPoints = new List<Vector3>();
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    // --- NOUVEAU : Historique des lignes ---
    private List<GameObject> linesHistory = new List<GameObject>();

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

        currentLineRenderer.startColor = currentColor;
        currentLineRenderer.endColor = currentColor;
        currentLineRenderer.material.color = currentColor; 

        // --- NOUVEAU : On ajoute la ligne à l'historique ---
        linesHistory.Add(currentLine);
        // --------------------------------------------------

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

    // --- FONCTION 1 : TOUT EFFACER (Sécurisée) ---
    public void ClearAllLines()
    {
        // On détruit tout ce qui est dans notre liste
        foreach (GameObject line in linesHistory)
        {
            if (line != null) Destroy(line);
        }
        linesHistory.Clear(); // On vide la liste
    }

    // --- FONCTION 2 : UNDO (Annuler le dernier trait) ---
    public void UndoLastLine()
    {
        if (linesHistory.Count > 0)
        {
            // 1. Récupérer le dernier objet de la liste
            int lastIndex = linesHistory.Count - 1;
            GameObject lastLine = linesHistory[lastIndex];

            // 2. Le détruire de la scène
            if (lastLine != null) Destroy(lastLine);

            // 3. Le retirer de la liste
            linesHistory.RemoveAt(lastIndex);
        }
    }
    // ----------------------------------------------------

    private bool IsPointerOverUI(UnityEngine.InputSystem.Controls.TouchControl touch)
    {
        int touchId = touch.touchId.ReadValue();
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touchId);
    }

    public void SetBrushColor(Color newColor)
    {
        currentColor = newColor;
    }
    
    public void SetBrushColorByName(string colorName)
    {
        if(ColorUtility.TryParseHtmlString(colorName, out Color c))
        {
            currentColor = c;
        }
    }
}