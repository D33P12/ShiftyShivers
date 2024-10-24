using UnityEngine;

public class HidingZone : MonoBehaviour
{
    public bool playerIsHiding = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsHiding = true;  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsHiding = false; 
        }
    }
}
