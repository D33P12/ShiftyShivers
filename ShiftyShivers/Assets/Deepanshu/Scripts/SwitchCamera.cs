using Cinemachine;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
       [SerializeField] CinemachineVirtualCamera targetCamera; 
       [SerializeField] CinemachineVirtualCamera defaultCamera; 
       private bool isPlayerInTrigger = false;
       
       private void OnTriggerEnter(Collider other)
       {
           if (other.CompareTag("Player"))
           {
               SwitchToTargetCamera();
               isPlayerInTrigger = true;
           }
       }
   
       private void OnTriggerExit(Collider other)
       {
           if (other.CompareTag("Player") && isPlayerInTrigger)
           {
               SwitchToDefaultCamera();
               isPlayerInTrigger = false;
           }
       }
       private void SwitchToTargetCamera()
       {
           if (targetCamera != null)
           {
               targetCamera.Priority = 20;
           }
   
           if (defaultCamera != null)
           {
               defaultCamera.Priority = 15;
           }
       }
   
       private void SwitchToDefaultCamera()
       {
           if (targetCamera != null)
           {
               targetCamera.Priority = 15;
           }
   
           if (defaultCamera != null)
           {
               defaultCamera.Priority = 20;
           }
       }
}
