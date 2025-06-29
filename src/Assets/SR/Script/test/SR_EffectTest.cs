using UnityEngine;

public class SR_EffectTest : MonoBehaviour
{

    [SerializeField] GameObject EffectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        { 
        
            Instantiate(EffectPrefab,Vector3.zero,Quaternion.identity);
        
        }
    }
}
