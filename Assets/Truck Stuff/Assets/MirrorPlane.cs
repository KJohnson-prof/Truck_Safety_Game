using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MirrorPlane : MonoBehaviour
{
    public Camera mainCamera;
    private RenderTexture mirrorTexture;
    private Camera mirrorCamera;
    private GameObject mirrorCamObj;

    void Start()
    {
        mirrorTexture = new RenderTexture(256, 256, 16);
        mirrorCamObj = new GameObject($"Mirror Camera ({gameObject.name})");
        mirrorCamera = mirrorCamObj.AddComponent<Camera>();
        mirrorCamera.enabled = false;
        mirrorCamera.targetTexture = mirrorTexture;

        var camData = mirrorCamObj.AddComponent<UniversalAdditionalCameraData>();
        camData.renderShadows = false;

        // Force a unique material instance per plane
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetTexture("_BaseMap", mirrorTexture);
    }

    void LateUpdate()
    {
        if (mirrorCamera == null || mainCamera == null || mirrorCamObj == null) return;

        mirrorCamObj.transform.position = mainCamera.transform.position;
        mirrorCamObj.transform.rotation = mainCamera.transform.rotation;
        mirrorCamObj.transform.Rotate(0, 180, 0);

        mirrorCamera.Render();
    }
}