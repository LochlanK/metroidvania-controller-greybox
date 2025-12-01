using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Player Spawning")]
    [SerializeField, Tooltip("Set The Default Spawn Location, Using A SpawnPoint Monobehaviour")]
    SpawnPoint defaultSpawn = null;

    //To handle common execution order issues.
    private void Start()
    {

        // Handle any back end system triggers
        // Spawn any extra environment/NPCs/Lights/Animations
        // Spawn Player
    }

    private void SpawnPlayer()
    {
        
    }

}
