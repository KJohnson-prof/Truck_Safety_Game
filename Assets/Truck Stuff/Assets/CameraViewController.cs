using UnityEngine;
using UnityEngine.InputSystem;

public class CameraViewController : MonoBehaviour
{
    [SerializeField] private FollowCamera followCamera;
    
    [Header("View Mode Settings")]
    public int viewMode = 0;  // 0 = Third Person, 1 = First Person
    public KeyCode switchViewKey = KeyCode.V;  // Press V to switch views

    private int maxViewModes = 2;  // Only 2 view modes
    private float lastSwitchTime = 0f;
    private float switchCooldown = 0.3f;  // Prevent rapid switching

    void Start()
    {
        if (followCamera == null)
        {
            followCamera = GetComponent<FollowCamera>();
        }

        if (followCamera == null)
        {
            Debug.LogError("❌ FollowCamera script not found!");
        }
    }

    void Update()
    {
        // Check for view mode switch input
        if (Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame)
        {
            SwitchViewMode();
        }
    }

    void SwitchViewMode()
    {
        // Cooldown check
        if (Time.time - lastSwitchTime < switchCooldown)
            return;

        lastSwitchTime = Time.time;

        // Toggle between view modes (0 and 1)
        viewMode = (viewMode + 1) % maxViewModes;

        // Apply the view mode to camera
        if (followCamera != null)
        {
            followCamera.viewMode = viewMode;
        }

        // Log current view mode
        string modeName = GetViewModeName(viewMode);
        Debug.Log($"📷 Camera switched to: {modeName}");
    }

    string GetViewModeName(int mode)
    {
        return mode switch
        {
            0 => "Third Person 👀",
            1 => "First Person 🔫",
            _ => "Unknown"
        };
    }

    // Public method to set specific view mode
    public void SetViewMode(int newMode)
    {
        if (newMode >= 0 && newMode < maxViewModes)
        {
            viewMode = newMode;
            if (followCamera != null)
            {
                followCamera.viewMode = newMode;
            }

            string modeName = GetViewModeName(viewMode);
            Debug.Log($"📷 Camera set to: {modeName}");
        }
    }
}