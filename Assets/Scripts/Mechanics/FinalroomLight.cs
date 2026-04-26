using UnityEngine;
using UnityEngine.Rendering.Universal;


public class FinalroomLight : MonoBehaviour
{
    public Light2D finalLight;
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            finalLight.intensity = 0.4f;
        }
    }
    
}
