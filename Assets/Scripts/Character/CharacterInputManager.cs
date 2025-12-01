using UnityEngine;

//doesn't need to be a monobehaviour.
public class CharacterInputManager
{
    //Using the PlayerControls input mappings from the new input system.
    private InputSystem_Actions playerControls;

    //Initialise Within Character.cs 

    /// <summary>
    /// Initialises the CharacterInputManager by creating an instance of playerControls and Enabling them.
    /// Call DisableControls() to disable control management or EnableControls() to re-enable.
    /// </summary>
    public void Init()
    {
        //Instantiate an object of the PlayerControls class.
        playerControls = new InputSystem_Actions();
        EnableControls();
    }

    public void EnableControls()
    {
        //Enable the playerControls when the monobehaviour is enabled.
        playerControls.Enable();
    }

    public void DisableControls()
    {
        //Handle Disabling when scene change occurs.
        playerControls.Disable();
    }

    public bool CheckJump()
    {

        //just return false for now.
        return false;
    }

    //Returns the Input of the Cartesian Movement with InputHandling.
    public Vector2 CartesianMovement()
    {
        //just return zero for now.
        return Vector2.zero;
    }


}
