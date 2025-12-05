using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPainter : MonoBehaviour
{
    [Header("Références")]
    public Camera arCamera; // la AR Camera
    public GameObject linePrefab; // prefab avec LineRenderer
    public ARPlaneManager arPlaneManager;

    [Header("Paramètres du trait")]
    public float minDistanceBetweenPoints = 0.01f; // distance minimale entre deux points pour ajouter
    public float lineOffset = 0.01f; // écarte le trait du mur pour éviter z-fighting
    public TrackableType trackableTypes = TrackableType.PlaneWithinPolygon;

    ARRaycastManager raycastManager;
    GameObject currentLine;
    LineRenderer currentLineRenderer;
    List<Vector3> currentPoints = new List<Vector3>();

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        if (arCamera == null)
            arCamera = Camera.main;
    }

    void Update()
    {
        // Utilisation du New Input System : Touchscreen
        if (Touchscreen.current == null)
            return; // pas de touchscreen détecté

        var touch = Touchscreen.current.primaryTouch;
        if (touch.press.isPressed) // le doigt est posé
        {
            Vector2 screenPos = touch.position.ReadValue();
            HandleTouchHold(screenPos);
        }
        else
        {
            // si on relâche et on a une ligne active -> finir la ligne
            if (currentLine != null)
            {
                FinishLine();
            }
        }
    }

    void HandleTouchHold(Vector2 screenPos)
    {
        // Raycast AR depuis l'écran
        if (raycastManager.Raycast(screenPos, s_Hits, trackableTypes))
        {
            var hit = s_Hits[0];
            Vector3 hitPos = hit.pose.position;
            // Récupérer la plane (pour obtenir la normale si besoin)
            ARPlane plane = null;
            if (arPlaneManager != null)
            {
                plane = arPlaneManager.GetPlane(hit.trackableId);
            }


            Vector3 normal = Vector3.up;
            if (plane != null)
            {
                normal = plane.transform.up; // normal de la plane
            }
            else
            {
                // fallback : utiliser le pose.rotation
                normal = hit.pose.rotation * Vector3.up;
            }

            // applique un léger offset suivant la normale pour éviter z-fighting
            Vector3 finalPos = hitPos + normal * lineOffset;

            if (currentLine == null)
            {
                StartNewLine(finalPos, hit);
            }
            else
            {
                // ajouter des points s'il y a assez de distance
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
        // Optionnel : si la ligne n'a que 1 point, on peut faire un petit ajustement
        currentLine = null;
        currentLineRenderer = null;
        currentPoints.Clear();
    }

    // Méthodes utilitaires publiques
    public void ClearAllLines()
    {
        foreach (var lr in FindObjectsOfType<LineRenderer>())
        {
            Destroy(lr.gameObject);
        }
    }
}
