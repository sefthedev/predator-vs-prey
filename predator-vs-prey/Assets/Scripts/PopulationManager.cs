using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] GameObject predatorPrefab;
    [SerializeField] GameObject preyPrefab;
    [SerializeField] int predatorPopulationMaxSize = 120;
    List<GameObject> predatorPopulation = new List<GameObject>();
    [SerializeField] int preyPopulationMaxSize = 800;
    List<GameObject> preyPopulation = new List<GameObject>();



    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 150;
        for (int i = 0; i < predatorPopulationMaxSize; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-38, 38), Random.Range(-23, 23), 0);
            Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 365));
            GameObject go = Instantiate(predatorPrefab, pos, rot);
            predatorPopulation.Add(go);
        }
        for (int i = 0; i < preyPopulationMaxSize; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-38, 38), Random.Range(-23, 23), 0);
            Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 365));
            GameObject go = Instantiate(preyPrefab, pos, rot);
            preyPopulation.Add(go);
        }
    }
    
    public void RemoveFromPreyPopulation(GameObject go)
    {
        preyPopulation.Remove(go);
    }
    public void RemoveFromPredatorPopulation(GameObject go)
    {
        predatorPopulation.Remove(go);
    }
    public void AddToPreyPopulation(GameObject go)
    {
        preyPopulation.Add(go);
    }
    public void AddToPredatorPopulation(GameObject go)
    {
        predatorPopulation.Add(go);
    }
    void Update()
    {

    }

    public int PreyPopulation 
    {
        get { return preyPopulation.Count; }
    }

    public int PreyPopulationMaxSize {
        get { return preyPopulationMaxSize; }
    }

    public int PredatorPopulation
    {
        get { return predatorPopulation.Count; }
    }

    public int preadatorPopulationMaxSize
    {
        get { return predatorPopulationMaxSize; }
    }
}
