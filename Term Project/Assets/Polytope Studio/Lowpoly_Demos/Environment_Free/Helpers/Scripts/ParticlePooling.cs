using UnityEngine;
using System.Collections.Generic;

public class ParticlePooling : MonoBehaviour
{
    public GameObject particlePrefab;
    public int poolSize = 30; // Predefined pool size
    public int gridRows = 3; // Number of rows in the grid
    public int gridColumns = 3; // Number of columns in the grid
    public float spacing = 2.0f; // Distance between particles in the grid

    public LODLatest lodLatest; // Reference to the LODLatest script

    private Queue<GameObject> particlePool;

    void Start()
    {
        InitializePool();
        SpawnParticlesOnGrid();
    }

    void InitializePool()
    {
        particlePool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(particlePrefab);
            particle.SetActive(false);
            particlePool.Enqueue(particle);
        }
    }

    void SpawnParticlesOnGrid()
    {
        // Get the position of the spawner object
        Vector3 spawnerPosition = transform.position;

        for (int row = 0; row < gridRows; row++)
        {
            for (int col = 0; col < gridColumns; col++)
            {
                // Calculate the position for each particle in the grid
                Vector3 position = spawnerPosition + new Vector3(
                    col * spacing,   // Offset in the X direction
                    0,               // Keep Y position the same
                    row * spacing    // Offset in the Z direction
                );

                // Get a particle from the pool and place it at the calculated position
                GameObject particle = GetPooledParticle(position);
                if (particle == null)
                {
                    Debug.LogWarning("Particle pool exhausted!");
                }
            }
        }
    }

    public GameObject GetPooledParticle(Vector3 position)
    {
        if (particlePool.Count > 0)
        {
            GameObject particle = particlePool.Dequeue();
            particle.SetActive(true);
            particle.transform.position = position;

            // Register the particle with LODLatest
            if (lodLatest != null)
            {
                ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    lodLatest.AddParticleSystem(particleSystem);
                }
            }

            return particle;
        }
        return null; // Pool exhausted
    }

    public void ReturnToPool(GameObject particle)
    {
        particle.SetActive(false);
        
        // Deregister the particle from LODLatest
        if (lodLatest != null)
        {
            ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                lodLatest.RemoveParticleSystem(particleSystem);
            }
        }

        particlePool.Enqueue(particle);
    }
}
