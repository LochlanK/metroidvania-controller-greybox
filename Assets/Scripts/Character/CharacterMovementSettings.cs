using UnityEngine;

public class CharacterMovementSettings : MonoBehaviour
{
    [SerializeField] ScriptableMovementStats movementStats;

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
        if(!movementStats) return;
        //we're loading directly from the SO.
        Gravity = -(2f * movementStats.JumpHeight) / Mathf.Pow(movementStats.TimeTilJumpApex, 2f);
        InitialJumpVelocity = Mathf.Abs(Gravity) * movementStats.TimeTilJumpApex;
    }

}
