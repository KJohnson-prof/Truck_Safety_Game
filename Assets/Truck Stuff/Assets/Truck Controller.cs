using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class TruckController : MonoBehaviour
{
    [Header("Wheels")]
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public WheelCollider WheelRL;
    public WheelCollider WheelRR;
    public Transform WheelFLtrans;
    public Transform WheelFRtrans;
    public Transform WheelRLtrans;
    public Transform WheelRRtrans;

    [Header("Physics")]
    public float truckMass = 300f;
    public float linearDamping = 0.1f;
    public float angularDamping = 0.5f;
    
    [Header("Steering")]
    public float maxSteerAngle = 30f;
    public float steerResponseSpeed = 15f;

    [Header("Braking")]
    public float maxBrakeTorque = 3000f;        // INCREASED from 1500
    public float brakePower = 5000f;            // Extra brake force
    
    [Header("Speed Modes")]
    public float normalSpeed = 7f;
    public float shiftBaseSpeed = 14f;
    public float shiftSpeedIncrement = 2f;
    public float shiftIncrementInterval = 2f;
    public float capsSpeed = 30f;

    private Rigidbody rb;
    private float currentSteerAngle = 0f;
    private bool isBraking = false;
    
    // Speed mode tracking
    private bool isShiftHeld = false;
    private bool isCapsHeld = false;
    private float shiftHoldTimer = 0f;
    private float currentTorque = 0f;

    // Sound effect settings
    [Header("Sound")]
    public AudioClip engineIdleClip;
    public AudioClip engineDrivingClip;
    private AudioSource audioSource;
    
    // Debug
    private float lastLogTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.mass = truckMass;
            rb.linearDamping = linearDamping;
            rb.angularDamping = angularDamping;
            rb.constraints = rb.constraints;
        }

        ConfigureWheels();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.spatialBlend = 0f; // full 2D so distance doesn't matter
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(engineIdleClip, 0.7f);
        PlayEngineSound(engineIdleClip);

        Debug.Log("🚚 Truck Initialized! Normal: 7 | Shift: 14+ | Caps: 30");
    }

    void ConfigureWheels()
    {
        WheelCollider[] wheels = { WheelFL, WheelFR, WheelRL, WheelRR };

        foreach (WheelCollider wheel in wheels)
        {
            if (wheel == null) continue;

            // Suspension for grip
            wheel.suspensionDistance = 0.3f;
            JointSpring spring = new JointSpring();
            spring.spring = 80000f;
            spring.damper = 5000f;
            spring.targetPosition = 0.2f;
            wheel.suspensionSpring = spring;

            // Forward friction - for acceleration and climbing
            WheelFrictionCurve forwardFriction = wheel.forwardFriction;
            forwardFriction.extremumSlip = 0.1f;
            forwardFriction.extremumValue = 2.5f;
            forwardFriction.asymptoteSlip = 0.3f;
            forwardFriction.asymptoteValue = 2.0f;
            wheel.forwardFriction = forwardFriction;

            // Sideways friction - for steering
            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
            sidewaysFriction.extremumSlip = 0.1f;
            sidewaysFriction.extremumValue = 2.2f;
            sidewaysFriction.asymptoteSlip = 0.3f;
            sidewaysFriction.asymptoteValue = 1.8f;
            wheel.sidewaysFriction = sidewaysFriction;

            wheel.mass = 50f;
            wheel.wheelDampingRate = 0.1f;
        }

        Debug.Log("✅ Wheels configured with strong grip!");
    }

    void FixedUpdate()
    {
        if (Keyboard.current == null) return;

        // Get acceleration and steering input
        float accelerationInput = GetAccelerationInput();
        float steerInput = GetSteerInput();
        
        // Get brake input
        isBraking = Keyboard.current.spaceKey.isPressed;

        // Handle speed modes
        HandleSpeedModes(accelerationInput);

        // Apply all forces
        ApplyMotor(accelerationInput);
        ApplySteering(steerInput);
        ApplyBrakes();
        RotateTruck(accelerationInput, steerInput);

        // Update wheel visuals
        UpdateWheelRotation();

        HandleEngineSound(accelerationInput);
    }

    float GetAccelerationInput()
    {
        float input = 0f;
        
        if (Keyboard.current.wKey.isPressed)
            input = 1f;
        if (Keyboard.current.sKey.isPressed)
            input = -1f;
            
        return input;
    }

    float GetSteerInput()
    {
        float input = 0f;
        
        if (Keyboard.current.aKey.isPressed)
            input = -1f;
        if (Keyboard.current.dKey.isPressed)
            input = 1f;
            
        return input;
    }

    void HandleSpeedModes(float accelerationInput)
    {
        isShiftHeld = Keyboard.current.leftShiftKey.isPressed;
        isCapsHeld = Keyboard.current.capsLockKey.isPressed;

        float finalSpeed = 0f;

        // CapsLock mode - FIXED 30 speed
        if (isCapsHeld && accelerationInput > 0)
        {
            finalSpeed = capsSpeed;
            shiftHoldTimer = 0f;
            
            if (Time.time - lastLogTime > 0.5f)
            {
                Debug.Log($"🔥 CAPS MODE | Speed: {finalSpeed} (FIXED)");
                lastLogTime = Time.time;
            }
        }
        // Shift mode - 14 base + increases by 2 every 2 secs
        else if (isShiftHeld && accelerationInput > 0)
        {
            shiftHoldTimer += Time.fixedDeltaTime;
            int increments = (int)(shiftHoldTimer / shiftIncrementInterval);
            finalSpeed = shiftBaseSpeed + (increments * shiftSpeedIncrement);
            
            if (Time.time - lastLogTime > 0.5f)
            {
                Debug.Log($"⚡ SHIFT MODE | Speed: {finalSpeed} | Time: {shiftHoldTimer.ToString("F1")}s");
                lastLogTime = Time.time;
            }
        }
        // Normal mode - FIXED 7 speed
        else if (accelerationInput > 0)
        {
            finalSpeed = normalSpeed;
            shiftHoldTimer = 0f;
            
            if (Time.time - lastLogTime > 0.5f)
            {
                Debug.Log($"📍 NORMAL MODE | Speed: {finalSpeed} (FIXED)");
                lastLogTime = Time.time;
            }
        }
        else
        {
            shiftHoldTimer = 0f;
        }

        // Convert speed to torque
        currentTorque = finalSpeed * 350f;
    }

    void ApplyMotor(float accelerationInput)
    {
        if (isBraking)
        {
            // No motor torque when braking
            if (WheelRL != null) WheelRL.motorTorque = 0;
            if (WheelRR != null) WheelRR.motorTorque = 0;
        }
        else
        {
            // Apply torque to rear wheels
            float torque = currentTorque * accelerationInput;
            
            if (WheelRL != null) WheelRL.motorTorque = torque;
            if (WheelRR != null) WheelRR.motorTorque = torque;
        }
    }

    void ApplySteering(float steerInput)
    {
        // Smooth steering
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, maxSteerAngle * steerInput, 
            Time.fixedDeltaTime * steerResponseSpeed);

        if (WheelFL != null) WheelFL.steerAngle = currentSteerAngle;
        if (WheelFR != null) WheelFR.steerAngle = currentSteerAngle;
    }

    void ApplyBrakes()
    {
        if (isBraking)
        {
            // MUCH STRONGER BRAKING
            float brakeTorque = maxBrakeTorque * 4f; // 12000 total brake force
            
            if (WheelFL != null) WheelFL.brakeTorque = brakeTorque;
            if (WheelFR != null) WheelFR.brakeTorque = brakeTorque;
            if (WheelRL != null) WheelRL.brakeTorque = brakeTorque;
            if (WheelRR != null) WheelRR.brakeTorque = brakeTorque;
            
            // EXTRA BRAKE: Apply backward force
            if (rb != null && rb.linearVelocity.magnitude > 0.1f)
            {
                rb.linearVelocity = rb.linearVelocity * 0.92f; // Reduce velocity each frame
            }
            
            if (Time.time - lastLogTime > 0.5f)
            {
                Debug.Log($"🛑 BRAKING APPLIED | Current Speed: {rb.linearVelocity.magnitude.ToString("F1")}");
                lastLogTime = Time.time;
            }
        }
        else
        {
            // No brakes
            if (WheelFL != null) WheelFL.brakeTorque = 0;
            if (WheelFR != null) WheelFR.brakeTorque = 0;
            if (WheelRL != null) WheelRL.brakeTorque = 0;
            if (WheelRR != null) WheelRR.brakeTorque = 0;
        }
    }

    void RotateTruck(float accelerationInput, float steerInput)
    {
        if (Mathf.Abs(accelerationInput) > 0.1f && Mathf.Abs(steerInput) > 0.1f)
        {
            float rotationSpeed = 50f * accelerationInput * steerInput;
            transform.Rotate(0, rotationSpeed * Time.fixedDeltaTime, 0, Space.Self);
        }
    }

    void UpdateWheelRotation()
    {
        // Front left
        if (WheelFLtrans != null && WheelFL != null)
        {
            WheelFLtrans.Rotate(WheelFL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
            Vector3 eulerFL = WheelFLtrans.localEulerAngles;
            eulerFL.y = WheelFL.steerAngle;
            WheelFLtrans.localEulerAngles = eulerFL;
        }

        // Front right
        if (WheelFRtrans != null && WheelFR != null)
        {
            WheelFRtrans.Rotate(WheelFR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
            Vector3 eulerFR = WheelFRtrans.localEulerAngles;
            eulerFR.y = WheelFR.steerAngle;
            WheelFRtrans.localEulerAngles = eulerFR;
        }

        // Rear left
        if (WheelRLtrans != null && WheelRL != null)
        {
            WheelRLtrans.Rotate(WheelRL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        }

        // Rear right
        if (WheelRRtrans != null && WheelRR != null)
        {
            WheelRRtrans.Rotate(WheelRR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        }
    }

        void PlayEngineSound(AudioClip clip)
    {
       if (audioSource.clip == clip || clip == null)
            return;
        audioSource.clip = clip;
        audioSource.Play();
    }
    void HandleEngineSound(float accelerationInput)
    {
        if (Mathf.Abs(accelerationInput) > 0.1f)
        {
            PlayEngineSound(engineDrivingClip);
            // pitch goes higher as speed increases
            audioSource.pitch = Mathf.Lerp(1f, 2f, rb.linearVelocity.magnitude / 20f);
        }
        else
        {
            PlayEngineSound(engineIdleClip);
            audioSource.pitch = 1f;
        }
    }
    public void StopEngineSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

}