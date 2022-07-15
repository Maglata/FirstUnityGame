using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackHitbox;
    public float attackRange = 0.5f;
    public int damage = 40;
    public LayerMask enemyLayers;
    private AudioManager_PrototypeHero _audioManager;

    void Start()
    {
        _audioManager = AudioManager_PrototypeHero.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    private void Attack()
    {
        //detects enemies
        Collider2D[] hitEnemies = 
            Physics2D.OverlapCircleAll(attackHitbox.position
            ,attackRange,enemyLayers);

        //damage enemies
        foreach(Collider2D enemy in hitEnemies)
        {

        }
        AE_Hit();
    }

    private void OnDrawGizmosSelected()
    {
        if(attackHitbox == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackHitbox.position, attackRange);
    }

    void AE_Hit()
    {
        _audioManager.PlaySound("Hit");
    }
}
