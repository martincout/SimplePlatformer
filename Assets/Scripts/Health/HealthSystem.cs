using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    public HealthBar healthBar;

    private void Awake()
    {
        if (gameObject.CompareTag("Player"))
        {
            healthBar = FindObjectOfType<HealthBar>().GetComponent<HealthBar>();
        }
        
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }


    private void UpdateHealthBar()
    {
        //UI Heath
        healthBar?.SetHealth(GetHealth());
    }

    public void SetHealth(float hp)
    {
        currentHealth = hp;
    }
    public void SetMaxHealth(float maxHp)
    {
        maxHealth = maxHp;
        currentHealth = maxHp;
        healthBar?.SetMaxHealth(maxHealth);
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void DealDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();
    }

    public void Heal(float healthAmount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += Mathf.Abs(healthAmount);
            UpdateHealthBar();
        }
    }
}
