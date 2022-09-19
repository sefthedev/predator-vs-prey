using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour
{
    [SerializeField] static float timeToSplit = 10f;
    bool firstGeneration = true;
    float splitTime = 0f;
    [SerializeField] static float maxEnergy = 15f;
    float energy = maxEnergy;
    PopulationManager populationManager;
    Vector3 pos;
    private void Awake()
    {
        populationManager = GameObject.Find("PopulationManager").GetComponent<PopulationManager>();
        if (firstGeneration)
        {
            firstGeneration = false;
        }
        else
        {
        }
    }

    void Start()
    {
        pos = transform.position;
    }

    void Update()
    {
        if (transform.position == pos)
        {
            if (energy < maxEnergy)
            {
               
                energy = Mathf.Clamp(energy + Time.deltaTime, 0f, maxEnergy);
            }
        }
        else
        {
            pos = transform.position;
        }
        splitTime += Time.deltaTime;
        if (splitTime > timeToSplit)
        {
            splitTime = 0f;
            if (populationManager.PreyPopulation < populationManager.PreyPopulationMaxSize)
            {
                GameObject go = Instantiate(gameObject);
                go.name = "Prey";
                populationManager.AddToPreyPopulation(go);

            }
        }
    }

}
