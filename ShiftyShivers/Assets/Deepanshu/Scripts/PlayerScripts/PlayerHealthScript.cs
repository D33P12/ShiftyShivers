using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthScript : MonoBehaviour
{
    
    [SerializeField] public float pmaxhealth;
    [SerializeField] private TextMeshProUGUI healthText; 
    [SerializeField] private GameObject GameOverCanvas;
    void Start()
    {
        pmaxhealth = GameManager.phealth; 
        UpdateHealthUI();
    }

    void Update()
    {
        UpdateHealthUI();
        if ( GameManager.phealth <= 0)
        {
            GameOver();
        }
    }
    private void UpdateHealthUI()
    {
        healthText.text = $"{ GameManager.phealth}/{pmaxhealth}"; 
    }
    private void GameOver()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameOverCanvas.SetActive(true);
        Time.timeScale = 0;
       
    }

}
