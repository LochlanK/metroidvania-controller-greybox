using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ScriptableMovementStats", menuName = "Scriptable Objects/ScriptableMovementStats")]
/// <summary>
/// This is for holding prefab-able MovementStats. Allows for loading in different stats per level or per requirement, 
/// </summary>
public class ScriptableMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 100f)] public float MaxWalkSpeed = 12.5f;
    [Range(0.2f, 50f)] public float GroundAcceleration = 5f;
    [Range(0.2f, 50f)] public float GroundDeceleration = 20f;
    [Range(0.2f, 50f)] public float AirAcceleration = 5f;
    [Range(0.2f, 50f)] public float AirDeceleration = 5f;

    [Header("Run")]
    [Range(1f,100f)] public float MaxRunSpeed = 20f;

    [Header("Dash")]
    [Range(1f,50f)] public float MaxDashForce = 10f;
    [Range(1f,20f)] public float MaxDashDistance = 8f;

    [Header("Ground Detection Sensor Settings")]
    public LayerMask GroundedLayer;
    public float GroundDetectionRayLength = 0.02f;
    public float HeadDetectionRayLength = 0.02f;
    [Range(0f, 1f)] public float HeadWidth = 0.75f;

    [Header("Jump")]
    public float JumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float JumpHeightCompensationFactor = 1.054f;
    public float TimeTilJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;
    public float MaxFallSpeed = 26f;
    [Range(1, 5)] public int NumberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float ApexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float JumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;

        [Header("Debug")]
    public bool DebugShowIsGroundedBox;
    public bool DebugShowHeadBumpBox;

    [Header("Jump Visualisation Tool")]
    public bool ShowWalkJumpArc = false;
    public bool ShowRunJumpArc = false;
    public bool StopOnCollision = false;
    public bool DrawRight = true;
    [Range(5, 100)] public int ArcResolution = 20;
    [Range(0, 500)] public int VisualisationSteps = 90;

    //Just to hide these in the inspector.
    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }
    public float AdjustedJump {get; private set;}

     void OnValidate()
    {
        CalculateValues();
    }

    void OnEnable()
    {
        CalculateValues();
    }

    private protected void CalculateValues()
    {
        //we're loading directly from the SO.
        AdjustedJump = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * AdjustedJump) / Mathf.Pow(TimeTilJumpApex, 2f);
        InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTilJumpApex;
    }
}
