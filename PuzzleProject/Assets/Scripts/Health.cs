using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField] public int MaxHealth;
    [SerializeField] public int CurrentHealth;
    [SerializeField] public bool isDead = false;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        isDead = false;
    }

    void TakeDamage (int amount = 1)
    {
        CurrentHealth -= amount;

        if(CurrentHealth <= 0)
        {
            isDead = true;

            anim.SetBool("IsDead", true);
        }


    }

}
