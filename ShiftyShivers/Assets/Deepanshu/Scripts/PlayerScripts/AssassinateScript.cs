using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AssassinateScript : MonoBehaviour
{
    [SerializeField] private int NumbberOfPowerUp = 3;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private List<GameObject> enemies;
    private GameObject currentEnemy;

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
        if (other.CompareTag("KillZone"))
        {
            currentEnemy = other.transform.parent.gameObject;
            Debug.Log("Enemy detected: " + currentEnemy.name);
            
            if (!enemies.Contains(currentEnemy))
            {
                enemies.Add(currentEnemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("KillZone") && currentEnemy == other.transform.parent.gameObject)
        {
            currentEnemy = null;
        }
    }

    private void OnInteract(bool isInteracting)
    {
        if (isInteracting && currentEnemy != null && GameManager.PowerUP >= NumbberOfPowerUp)
        {
            KillEnemy(currentEnemy);
        }
    }

    private void KillEnemy(GameObject enemy)
    {
        EnemyAIBase enemyScript = enemy.GetComponent<EnemyAIBase>();
        if (enemyScript != null)
        {
            enemyScript.DeathState(); 
            Debug.Log("Enemy killed: " + enemy.name);
            enemies.Remove(enemy);
            currentEnemy = null; 
        }
    }
}
