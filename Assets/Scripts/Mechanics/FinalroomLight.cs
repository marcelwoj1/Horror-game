using UnityEngine;
using UnityEngine.Rendering.Universal;


public class FinalroomLight : MonoBehaviour
{
    [Header("Components")]
    public Light2D finalLight;
    //Turns the light brighter for the final room
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            finalLight.intensity = 0.4f;
        }
    }
    
}
