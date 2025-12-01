using UnityEngine;

public class SpawnPoint : MonoBehaviour
{   
    [SerializeField, Tooltip("Set The Coordinate for the spawn point")]
    Vector2 spawnLocation = Vector2.zero;
    
    [SerializeField, Tooltip("Override spawnLocation with the value of this spawnpoint object's world space. coord")]
    bool useGameObjectAsSpawn = false;

    void OnEnable()
    {
        if (!useGameObjectAsSpawn) return;
        
    }
}
