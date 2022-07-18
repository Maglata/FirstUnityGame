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

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = Health;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ETakeDamage(int damage)
    {
        currentHealth -= damage;

        _animator.SetTrigger("Hurt");

        if(currentHealth <= 0)
        {
            _animator.SetTrigger("Death");
            DestroyEnemy(); 
        }
    }

    private async void DestroyEnemy()
    {
        await Task.Delay(2000);
        Object.Destroy(gameObject);
    }
}
