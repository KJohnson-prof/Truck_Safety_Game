using UnityEngine;

public class BlindSpotDetector : MonoBehaviour
{
    public string zoneName = "Right Side No-Zone";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pedestrian")) return;
        
        Debug.Log($"Pedestrian entered: {zoneName}");
        BlindSpotDetector.lastHitZone = zoneName;
    }

    public static string lastHitZone = "";
}