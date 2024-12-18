using UnityEngine;
using System.Collections.Generic;

public class LODLatest : MonoBehaviour
{
    public GameObject targetObject;              // Any object in the scene
    public List<ParticleSystem> particleSystems;  // List of Particle Systems (clones from pooling)
    public Material material15FPS;               // Material with 15 FPS texture
    public Material material45FPS;               // Material with 45 FPS texture
    public Transform playerCamera;               // Reference to the player's camera
    public float distanceThreshold = 10f;        // Distance threshold to switch materials
    public float closeEmissionRate = 100f;       // Emission rate when close
    public float farEmissionRate = 10f;          // Emission rate when far

    private Material currentMaterial;            // To keep track of the currently applied material

    void Start()
    {
        // Ensure that particleSystems list is initialized
        if (particleSystems == null)
        {
            particleSystems = new List<ParticleSystem>();
        }
    }

    void Update()
    {
         if (playerCamera == null || targetObject == null || particleSystems.Count == 0) return;

    // Calculate the distance between the player camera and the target object
    float distance = Vector3.Distance(playerCamera.position, targetObject.transform.position);

    // Adjust emission rate based on distance
    float t = Mathf.Clamp01(distance / distanceThreshold);
    float adjustedEmissionRate = Mathf.Lerp(closeEmissionRate, farEmissionRate, t);

    // Iterate over all particle systems and apply changes
    foreach (var particleSystem in particleSystems)
    {
        if (particleSystem == null) continue;

        // Access necessary modules for each particle system
        Renderer particleRenderer = particleSystem.GetComponent<Renderer>();
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;

        if (particleRenderer == null) continue;

        // Update emission rate dynamically
        ParticleSystem.MinMaxCurve emissionCurve = new ParticleSystem.MinMaxCurve(adjustedEmissionRate);
        emissionModule.rateOverTime = emissionCurve;

        // Debugging: Log the emission rate to verify changes
        Debug.Log($"ParticleSystem: {particleSystem.name}, Emission rate set to: {adjustedEmissionRate}");

        // Check if the distance is within the threshold and switch material
        if (distance > distanceThreshold && particleRenderer.material != material15FPS)
        {
            SwitchMaterial(particleSystem, particleRenderer, particleSystem.textureSheetAnimation, material15FPS, new Vector2(5, 3), 15);
        }
        else if (distance <= distanceThreshold && particleRenderer.material != material45FPS)
        {
            SwitchMaterial(particleSystem, particleRenderer, particleSystem.textureSheetAnimation, material45FPS, new Vector2(7, 7), 45);
        }
    }
    }

    private void SwitchMaterial(ParticleSystem particleSystem, Renderer particleRenderer, ParticleSystem.TextureSheetAnimationModule textureSheetAnimation, Material material, Vector2 grid, int fps)
    {
        if (particleRenderer.material != material)
        {
            particleRenderer.material = material;
            textureSheetAnimation.numTilesX = (int)grid.x;
            textureSheetAnimation.numTilesY = (int)grid.y;

            // Adjust the frame rate using a curve
            AnimationCurve fpsCurve = AnimationCurve.Linear(0, 0, 1, fps * grid.x * grid.y);
            textureSheetAnimation.frameOverTime = new ParticleSystem.MinMaxCurve(1, fpsCurve);

            particleSystem.Stop();
            particleSystem.Play();

            // Debugging: Log the material switch
            Debug.Log($"Switched to material: {material.name}, Grid: {grid.x}x{grid.y}, FPS: {fps}");
        }
    }

    // Draw Gizmos to visualize the distance threshold
    void OnDrawGizmos()
    {
        if (targetObject != null)
        {
            Gizmos.color = Color.green;
            // Draw a wire sphere around the target object to visualize the distance threshold
            Gizmos.DrawWireSphere(targetObject.transform.position, distanceThreshold);
        }
    }

    // Add a new particle system to the list (called by pooling system)
    public void AddParticleSystem(ParticleSystem newParticleSystem)
    {
        if (!particleSystems.Contains(newParticleSystem))
        {
            particleSystems.Add(newParticleSystem);
        }
    }

    // Remove a particle system from the list (when it's disabled or destroyed)
    public void RemoveParticleSystem(ParticleSystem particleSystemToRemove)
    {
        if (particleSystems.Contains(particleSystemToRemove))
        {
            particleSystems.Remove(particleSystemToRemove);
        }
    }
}
