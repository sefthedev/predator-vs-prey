using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] GameObject predatorPrefab;
    [SerializeField] GameObject preyPrefab;
    [SerializeField] int predatorPopulationSize = 120;
    [SerializeField] int preyPopulationSize = 800;
    
    void Start()
    {
        for (int i = 0; i < predatorPopulationSize; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-38, 38), Random.Range(-23, 23), 0);
            Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 365));
            GameObject go = Instantiate(predatorPrefab, pos, rot);
        }
        for (int i = 0; i < preyPopulationSize; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-38, 38), Random.Range(-23, 23), 0);
            Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 365));
            GameObject go = Instantiate(preyPrefab, pos, rot);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
