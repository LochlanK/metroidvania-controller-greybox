using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] CharacterPhysicsManager cPhysicsManager;
    CharacterInputManager cInputManager;

    //Not Used Atm.
    [SerializeField] CharacterSoundManager cSoundManager;
    [SerializeField] CharacterAnimationManager cAnimationManager;

    private protected bool isDisabling = false;
    bool isGrounded = false;
    bool isFacingRight = false;

    //Adding Private Protected Virtualisation if needing to inherit from Character in the future and override functionality.
    //Such as making an AI character.
    //Also provides a bit of security when assembled, external assemblies cannot access the internal functions directly, even if it inherits from this.
    private protected virtual void OnEnable()
    {
        isDisabling = false;
        //Initialise CharacterInputManager
        cInputManager = new CharacterInputManager();
        cInputManager.Init();
        //Initialise CharacterSoundManager
        //Initialise CharacterAnimationManager
        //Initialise CharacterPhysicsManager
        cPhysicsManager.Init();
    }

    private protected virtual void OnDisable()
    {
        isDisabling = true;
        //Disable PhysicsManager
        cPhysicsManager.enabled = false;
        //Disable AnimationManager
        //Disable SoundManager
        //Disable InputManager
        cInputManager.DisableControls();
        
    }

    private protected void Update()
    {
        if (isDisabling) return;

        //Input buffering isn't handled explicitly yet, just getting values direct from the input manager.
        //Jump buffering alone will suffice until inputs are buffered appropriately.
        var (jumpPressed, jumpHeld, jumpReleased) = cInputManager.CheckJumpInput();
        var (sprintPressed, sprintHeld, sprintReleased) = cInputManager.CheckSprintInput();

        // Update Movement Timers
        cPhysicsManager.UpdateTimers();

        // Update Physics Inputs
        cPhysicsManager.JumpChecks(jumpPressed, jumpHeld, jumpReleased);
        cPhysicsManager.UpdateMovementInput(cInputManager.GetCartesianMovement(), sprintHeld);

        //update Animation (updates animations as immediately as possible.)
        //update Sound (handle non animation controller sounds)
    }

    private protected void FixedUpdate()
    {
        //update Physics calcs and physically apply them
        var (grounded, facingRight) = cPhysicsManager.ProcessPhysics();

        //allows for sharing these states with the animation module
        isGrounded = grounded; 
        isFacingRight = facingRight;
    }


}
