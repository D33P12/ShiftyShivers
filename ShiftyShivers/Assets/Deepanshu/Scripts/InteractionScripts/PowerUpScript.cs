using TMPro;
using UnityEngine;

public class PowerUpScript : MonoBehaviour
{
    public TextMeshProUGUI powerCount;
    public int PowerUPs { get; private set; }
  
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            GameManager.PowerUP += 1;

            Destroy(gameObject);

            powerCount.GetComponent<TextMeshProUGUI>().text = "PowerUP: " + GameManager.PowerUP + "/5";

        }
    }
    
}
