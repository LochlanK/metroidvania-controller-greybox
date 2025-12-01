using UnityEngine;

public class Character : MonoBehaviour
{
    //Initialise Controls and Modules before operation.

    private protected bool isDisabling = false;

    //Adding Private Protected Virtualisation if needing to inherit from Character in the future and override functionality.
    //Such as making an AI character.
    //Also provides a bit of security when assembled, external assemblies cannot access the internal functions directly, even if it inherits from this.
    private protected virtual void OnEnable()
    {
        isDisabling = false;
        //Initialise CharacterInputManager
        //Initialise CharacterMovementStats
        //Initialise CharacterSoundManager
        //Initialise CharacterAnimationManager
        //Initialise CharacterPhysicsManager
    }

    private protected virtual void OnDisable()
    {
        isDisabling = true;
        //Disable PhysicsManager
        //Disable AnimationManager
        //Disable SoundManager
        //Disable MovementStats
        //Disable InputManager
    }

    private protected void Update()
    {
        if (isDisabling) return;
        //get inputs from input manager (which handles input buffering etc.)
        //update Animation (updates animations as immediately as possible.)
        //update Sound (handle non animation controller sounds)
    }

    private protected void FixedUpdate()
    {
        //update Physics - inject input and movement stats (handles all physical movement calculations and states)
    }


}
