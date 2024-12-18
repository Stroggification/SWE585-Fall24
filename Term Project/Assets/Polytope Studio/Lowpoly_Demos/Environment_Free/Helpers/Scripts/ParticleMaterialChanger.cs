using UnityEngine;

public class ParticleMaterialChanger : MonoBehaviour
{
    public ParticleSystem particleSystem;  // Reference to the Particle System
    public Material material15FPS;         // Material with 15 FPS texture
    public Material material45FPS;         // Material with 45 FPS texture
    public Transform playerCamera;         // Reference to the player's camera
    public float distanceThreshold = 10f;  // Distance threshold to switch materials
    public float closeEmissionRate = 100f; // Emission rate when close
    public float farEmissionRate = 10f;    // Emission rate when far

    private Renderer particleRenderer;
    private ParticleSystem.TextureSheetAnimationModule textureSheetAnimation;
    private ParticleSystem.EmissionModule emissionModule;
    private Material currentMaterial;      // To keep track of the currently applied material

    void Start()
    {
        // Get the renderer of the particle system
        particleRenderer = particleSystem.GetComponent<Renderer>();
        if (particleRenderer == null)
        {
            Debug.LogError("No Renderer found on the Particle System!");
        }

        // Access the Texture Sheet Animation Module
        textureSheetAnimation = particleSystem.textureSheetAnimation;
        if (!textureSheetAnimation.enabled)
        {
            Debug.LogError("Texture Sheet Animation is not enabled on the Particle System!");
        }

        // Access the Emission Module
        emissionModule = particleSystem.emission;

        // Initialize current material
        currentMaterial = particleRenderer.material;
    }

    void Update()
    {
        if (particleRenderer == null || playerCamera == null || particleSystem == null) return;

        // Calculate the distance between the player camera and the particle system
        float distance = Vector3.Distance(playerCamera.position, particleSystem.transform.position);

        // Adjust emission rate based on distance
        float t = Mathf.Clamp01(distance / distanceThreshold);
        emissionModule.rateOverTime = Mathf.Lerp(closeEmissionRate, farEmissionRate, t);

        // Debugging: Print the current emission rate
        Debug.Log($"Current emission rate: {emissionModule.rateOverTime.constant}");

        // Check if the distance is within the threshold and switch material
        if (distance > distanceThreshold && currentMaterial != material15FPS)
        {
            Debug.Log("Switching to 15 FPS material");
            SwitchMaterial(material15FPS, new Vector2(5, 3), 15); // Grid 5x3 for 15 FPS
        }
        else if (distance <= distanceThreshold && currentMaterial != material45FPS)
        {
            Debug.Log("Switching to 45 FPS material");
            SwitchMaterial(material45FPS, new Vector2(7, 7), 45); // Grid 7x7 for 45 FPS
        }
    }

    private void SwitchMaterial(Material material, Vector2 grid, int fps)
    {
        if (currentMaterial != material)
        {
            currentMaterial = material;
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
        if (particleSystem != null)
        {
            Gizmos.color = Color.green;
            // Draw a wire sphere around the particle system's position to visualize the distance threshold
            Gizmos.DrawWireSphere(particleSystem.transform.position, distanceThreshold);
        }
    }
}
