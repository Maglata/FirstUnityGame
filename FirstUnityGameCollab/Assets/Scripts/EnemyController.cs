using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using CodeMonkey.HealthSystemCM;
using System.Diagnostics;

public class EnemyController : MonoBehaviour, IGetHealthSystem
{
    private Animator _animator;
    public int Health = 100;
    int currentHealth;
    private AudioManager_PrototypeHero _audioManager;
    private HealthSystem healthSystem;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = Health;
        _animator = GetComponent<Animator>();
        _audioManager = AudioManager_PrototypeHero.instance;
    }

    void Awake()
    {
        healthSystem = new HealthSystem(Health);
    }

    public void ETakeDamage(int damage)
    {
        currentHealth -= damage;
        healthSystem.Damage(damage);

        _animator.SetTrigger("Hurt");
        _audioManager.PlaySound("Hurt");

        if (currentHealth <= 0)
        {
            _animator.SetTrigger("Death");
            _audioManager.PlaySound("Death");
            DestroyEnemy(); 
        }
    }

    private async void DestroyEnemy()
    {
        GetComponent<PatrolAI>().enabled = false;
        await Task.Delay(1000);
        Object.Destroy(gameObject);
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }
}
