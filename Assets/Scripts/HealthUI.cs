using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image[] hearts;

    private void OnEnable()
    {
        playerHealth.OnHealthChange += UpdateHearts;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChange -= UpdateHearts;
    }

    private void UpdateHearts(int current, int max)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < current;
        }
    }
}
