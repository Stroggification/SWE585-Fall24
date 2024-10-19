using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject objectToSpawn;  
    public Transform spawnPoint;  
    public KeyCode spawnKey = KeyCode.Space;  

    void Update()
    {
        // Check if the player presses the assigned spawn key
        if (Input.GetKeyDown(spawnKey))
        {
            // Instantiate (spawn) the object at the given spawn point and rotation
            Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("New object spawned!");
        }
    }
}
