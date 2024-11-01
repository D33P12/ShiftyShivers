using UnityEngine;

public class LevelCompleteScript : MonoBehaviour
{
    [SerializeField] private GameObject LevelCompleteCanvas;
    [SerializeField] private int requiredPowerUp;
    
    private void Start()
    {
        LevelCompleteCanvas.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.PowerUP >= requiredPowerUp)
        {
            LevelCompleteCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
