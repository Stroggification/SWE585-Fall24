using UnityEngine;

public class SampleScript : MonoBehaviour
{
    public float moveSpeed = 5f;  

    void Update()
    {
        // Move the object on Z
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
