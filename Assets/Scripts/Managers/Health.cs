using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] protected int currLife;
    [SerializeField] protected int maxLife;
    [SerializeField] protected float inmunityTime;
    [SerializeField] protected float timeToReappear;
    protected CombatManager combatManager;
    protected bool isInmune;
    protected Animator animator;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currLife = maxLife;
        animator = GetComponent<Animator>();
        isInmune = false;
        combatManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CombatManager>();
    }

    public void DoDamage(int amount)
    {
        if (!isInmune)
        {
            //Comprueba si hay menos de 0 de vida
            currLife = (currLife - amount) < 0 ? 0 : currLife - amount;

            onDamage();

            if (currLife <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(Inmunity());
            }         
        }
    }

    public void DoHeal(int amount)
    {    
        currLife += amount;

        onHeal();

        if (currLife > maxLife)
        {
            currLife = maxLife;
        }       
    }
    public void IncreaseMaxLife(int amount)
    {
        maxLife += amount;
        //Si se quiere que se cure la vida a la vez que se aumenta:
        //currLife = maxLife;
    }
    private void Die()
    {
        onDeath();
        isInmune = true;
    }
    protected virtual void onDamage() { }
    protected virtual void onHeal() { } 
    protected virtual void onDeath() { }     

    private IEnumerator Inmunity()
    {
        isInmune = true;
        yield return new WaitForSeconds(inmunityTime);
        isInmune = false;
    }

    public void CanDamage(bool _b)
    {
        isInmune = _b;
    }
    public bool IsAlive()
    {
        return currLife > 0;
    }
}
