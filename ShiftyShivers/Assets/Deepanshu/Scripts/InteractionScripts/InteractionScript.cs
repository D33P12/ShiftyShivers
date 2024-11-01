using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionScript : MonoBehaviour
{
       [SerializeField] private InputManager inputManager;
       [SerializeField] private List<TastyTreat> tastyTreats;
    
       public int PowerUPs { get; private set; } 
       
       private GameObject nearbyPowerUp;
       private TastyTreat nearbyTastyTreat;
       private void OnEnable()
       {
           inputManager.onInteract += OnInteract;
       }
   
       private void OnDisable()
       {
           inputManager.onInteract -= OnInteract;
       }
       private void OnTriggerEnter(Collider other)
       {
           if (other.CompareTag("PowerUp"))
           {
               nearbyPowerUp = other.gameObject;
               
           }
       }
       private void OnTriggerExit(Collider other)
       {
           if (other.CompareTag("PowerUp") && nearbyPowerUp == other.gameObject)
           {
               nearbyPowerUp = null;
           }
       }
       private void OnInteract(bool isInteracting)
       {
           if (isInteracting)
           {
               if (nearbyPowerUp != null)
               {
                   CollectPowerUp(nearbyPowerUp);
               }
           }
       }
       private void CollectPowerUp(GameObject powerUp)
       {
           TastyTreat closestTreat = null;
           float closestDistance = Mathf.Infinity;
    
           foreach (TastyTreat treat in tastyTreats)
           {
               float distance = Vector3.Distance(transform.position, treat.transform.position);
               if (distance < closestDistance)
               {
                   closestDistance = distance;
                   closestTreat = treat;
               }
           }
           if (closestTreat != null)
           {
               closestTreat.EatenState();
               PowerUPs += 1; 
               GameManager.PowerUP = PowerUPs;
               Debug.Log("Collected a power-up. Total: " + PowerUPs);
           }
           nearbyPowerUp = null;
       }
}
