using TMPro;
using UnityEngine;
public class PlayerHealthScript : MonoBehaviour
{
    
    [SerializeField] public float pmaxhealth;
    [SerializeField] private TextMeshProUGUI healthText; 
    [SerializeField] private GameObject GameOverCanvas;
    private float previousHealth;
    void Start()
    {
        pmaxhealth = GameManager.phealth; 
        UpdateHealthUI();
        previousHealth = GameManager.phealth;
    }

    void Update()
    {
        UpdateHealthUI();
        if (GameManager.phealth < previousHealth)
        {
            SoundManager.Instance.PlayAudio(AudioType.PLAYERTAKEDAMAGE);
            previousHealth = GameManager.phealth;
        }
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
