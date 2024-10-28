using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteScript : MonoBehaviour
{
    [SerializeField] private GameObject LevelCompleteCanvas;

    private void Start()
    {
        LevelCompleteCanvas.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelCompleteCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
