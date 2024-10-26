using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionScript : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    public TextMeshProUGUI powerCount;
    public TextMeshProUGUI keyCard;
    public int PowerUPs { get; private set; }
    public int KeyCards { get; private set; }

    private GameObject nearbyPowerUp;
    private GameObject nearbyKeycard;

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
        }
    }

    private void CollectPowerUp(GameObject powerUp)
    {
        GameManager.PowerUP += 1;
        PowerUPs += 1;

        powerCount.text = "PowerUP: " + GameManager.PowerUP + "/5";
        Destroy(powerUp);
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
}
