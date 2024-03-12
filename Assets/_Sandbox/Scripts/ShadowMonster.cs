using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMonster : MonoBehaviour
{
    [SerializeField] private float monsterHealth;
    [Range(0.1f, 100f)][SerializeField] private float damageThresh;
    private float damageTaken;
    
    public void TakeDamage(float damage)
    {
        damageTaken += damage;
        Debug.Log("Damage Taken: " + damageTaken);

        if(damageTaken < damageThresh) return;
         monsterHealth -= damage;
         
         
        // TODO: move enemy away from light after taking damage.
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
