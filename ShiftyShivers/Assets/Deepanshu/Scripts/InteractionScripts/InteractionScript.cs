using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionScript : MonoBehaviour
{
       [SerializeField] private List<GameObject> doors;
       private GameObject nearbyDoor;
       [SerializeField] private InputManager inputManager;
       public TextMeshProUGUI keyCard;
       [SerializeField] private List<TastyTreat> tastyTreats;
    
       public int PowerUPs { get; private set; } 
       public int KeyCards { get; private set; }
       
    
       private GameObject nearbyPowerUp;
       private GameObject nearbyKeycard;
    
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
           else if (other.CompareTag("Keycard"))
           {
               nearbyKeycard = other.gameObject;
           }
           else if (other.CompareTag("Door"))
           {
               nearbyDoor = other.gameObject;
           }
       }
       private void OnTriggerExit(Collider other)
       {
           if (other.CompareTag("PowerUp") && nearbyPowerUp == other.gameObject)
           {
               nearbyPowerUp = null;
           }
           else if (other.CompareTag("Keycard") && nearbyKeycard == other.gameObject)
           {
               nearbyKeycard = null;
           }
           else if (other.CompareTag("Door") && nearbyDoor == other.gameObject)
           {
               nearbyDoor = null;
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
               else if (nearbyKeycard != null)
               {
                   CollectKeyCard(nearbyKeycard);
               }
               else if (nearbyDoor != null)
               {
                   DoorToggle(nearbyDoor);
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
       private void CollectKeyCard(GameObject keycard)
       {
           GameManager.keycard += 1;
           KeyCards += 1;
           keyCard.text = "KeyCard: " + GameManager.keycard;
           Destroy(keycard);
           nearbyKeycard = null;
       }
       private void DoorToggle(GameObject door)
       {
           if (GameManager.keycard > 0)
           {
               door.SetActive(!door.activeSelf);
               Debug.Log("Toggled door state.");
           }
           else
           {
               Debug.Log("Door is locked");
           }
       }
}
