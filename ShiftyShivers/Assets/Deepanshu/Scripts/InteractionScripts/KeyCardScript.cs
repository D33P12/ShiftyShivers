using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyCardScript : MonoBehaviour
{
    public TextMeshProUGUI keyCount;
    public int keycards { get; private set; }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.keycard += 1;
            Destroy(gameObject);

            keyCount.GetComponent<TextMeshProUGUI>().text = "KeyCard: " + GameManager.keycard ;

        }
    }
}
