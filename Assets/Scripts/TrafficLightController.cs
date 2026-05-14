using System.Collections;
using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
// TrafficLightController
//
// PedestrianController reads IsGreen:
//   - true  ->safe to cross (green light)
//   - false -> must wait    (red light)
// ─────────────────────────────────────────────────────────────────────────────

public class TrafficLightController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector Settings
    // ─────────────────────────────────────────────

    [Header("Timing")]
    public float greenDuration = 8f;
    public float redDuration   = 10f;

    [Header("Renderer")]
    public Renderer lightRenderer;

    [Header("Materials")]
    public Material redLightOn;
    public Material redLightOff;
    public Material greenLightOn;
    public Material greenLightOff;

    // ─────────────────────────────────────────────
    // Public State — read by PedestrianController
    // ─────────────────────────────────────────────

    /// <summary>True when the light is green — pedestrians may cross.</summary>
    public bool IsGreen { get; private set; } = false;

    // ─────────────────────────────────────────────
    // Lifecycle
    // ─────────────────────────────────────────────

    void Start()
    {
        // Start on red so pedestrians don't immediately cross on spawn
        SetLight(false);
        StartCoroutine(LightCycle());
    }

    // ─────────────────────────────────────────────
    // Light Cycle
    // ─────────────────────────────────────────────

    IEnumerator LightCycle()
    {
        while (true)
        {
            // Red phase — pedestrians wait
            SetLight(false);
            yield return new WaitForSeconds(redDuration);

            // Green phase — pedestrians may cross
            SetLight(true);
            yield return new WaitForSeconds(greenDuration);
        }
    }

    void SetLight(bool green)
    {
        IsGreen = green;

        if (lightRenderer != null)
            lightRenderer.material =
                green ? greenLightOn : redLightOn;

        Debug.Log("Traffic light --> " +
            (green ? "GREEN (cross)" : "RED (wait)"));
    }

    void SetEmission(Renderer rend, Color color)
    {
        // Works with Standard shader and URP/Lit shader
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        rend.GetPropertyBlock(block);
        block.SetColor("_EmissionColor", color);
        rend.SetPropertyBlock(block);
    }

    // ─────────────────────────────────────────────
    // Gizmos — show current state above the light
    // ─────────────────────────────────────────────

    // void OnDrawGizmos()
    // {
    //     #if UNITY_EDITOR
    //     UnityEditor.Handles.Label(
    //         transform.position + Vector3.up * 2f,
    //         IsGreen ? "🟢 GREEN" : "🔴 RED"
    //     );
    //     #endif
    // }
}