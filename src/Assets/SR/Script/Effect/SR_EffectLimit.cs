using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SR_EffectLimit : MonoBehaviour
{

    float CountLimit = 0;//全体のカウント

    public float DieTimeLimit =0;//消えるまでの時間

    public List<_ParticleSystems> particleSystemsList = new List<_ParticleSystems>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        CountLimit += Time.deltaTime;
       foreach (var p in particleSystemsList) 
        {

            if (p.TimeLimit < CountLimit) 
            {
                p._particleSystem.emissionRate = 0;
            }
        
        }

        if (DieTimeLimit < CountLimit) 
        {

            Destroy(gameObject);
        
        }
    }


}

[System.Serializable]
public class _ParticleSystems 
{
    public ParticleSystem _particleSystem;
    public float TimeLimit = 0;
}
