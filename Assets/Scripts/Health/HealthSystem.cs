using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    public HealthBar healthBar;

    public HealthSystem(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    public void SetHealthBar(HealthBar p_healthBar)
    {
        currentHealth = maxHealth;
        this.healthBar = p_healthBar;
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

    /// <summary>
    /// Gets the health with a normalized value (0 to 1)
    /// </summary>
    /// <returns></returns>
    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}
