using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator _animator;
    public int Health = 100;
    int currentHealth;

    private AudioManager_PrototypeHero _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = Health;
        _animator = GetComponent<Animator>();
        _audioManager = AudioManager_PrototypeHero.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ETakeDamage(int damage)
    {
        currentHealth -= damage;

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
        await Task.Delay(1000);
        Object.Destroy(gameObject);
    }
}
