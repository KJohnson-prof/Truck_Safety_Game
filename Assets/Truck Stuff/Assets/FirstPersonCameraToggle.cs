using UnityEngine;

public class FirstPersonCameraToggle : MonoBehaviour
{
    public Renderer truckMeshRenderer;
    public Camera playerCamera;
    private FollowCamera followCamera;
    private Material[] originalMaterials;

    void Start()
    {
        if (playerCamera != null)
        {
            followCamera = playerCamera.GetComponent<FollowCamera>();
        }
        
        if (truckMeshRenderer != null)
        {
            originalMaterials = truckMeshRenderer.materials;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (followCamera != null)
            {
                followCamera.viewMode = (followCamera.viewMode == 0) ? 1 : 0;
                UpdateTruckMeshVisibility();
            }
        }
    }

    void UpdateTruckMeshVisibility()
    {
        if (truckMeshRenderer == null) return;

        Material[] materials = truckMeshRenderer.materials;

        if (followCamera.viewMode == 1)
        {
            // First person - hide elements 5,6,7, keep 0-4 visible
            for (int i = 5; i < 8; i++)
            {
                materials[i].SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Front);
            }
            Debug.Log("👁️ FIRST PERSON - Body visible (0-4), wheels/exhaust hidden (5-7)");
        }
        else
        {
            // Third person - show all elements (skip index 4 = truck-alu, keep its Render Face = Both intact)
            for (int i = 0; i < materials.Length; i++)
            {
                if (i == 4) continue; // skip truck-alu
                materials[i].SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Back);
            }
            Debug.Log("🚚 THIRD PERSON - All visible");
        }
    }
}