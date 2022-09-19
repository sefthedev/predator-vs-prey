using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    int preyEaten = 0;
    [SerializeField] int eatenPreyToSplit = 3;
    [SerializeField] static float maxEnergy = 15f;
    [SerializeField] static float energyConsumption = 1f;
    float energy = maxEnergy;
    
    PopulationManager populationManager;

    private void Awake()
    {
        populationManager = GameObject.Find("PopulationManager").GetComponent<PopulationManager>();
    }
    void Start()
    {

    }

    void Update()
    {
        energy = Mathf.Clamp(energy-energyConsumption*Time.deltaTime, 0f, maxEnergy);
        if(energy <= 0)
        { 
            populationManager.RemoveFromPredatorPopulation(gameObject);
            Destroy(gameObject);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.tag == "Trunk")
        {
            if (collision.gameObject.name.StartsWith("Prey"))
            {
                populationManager.RemoveFromPreyPopulation(collision.gameObject);
                Destroy(collision.gameObject);
                preyEaten++;
                energy = Mathf.Clamp(energy + 0.4f*maxEnergy, 0f, maxEnergy);
                if (preyEaten >= eatenPreyToSplit)
                {
                    preyEaten = 0;
                    if (populationManager.PredatorPopulation < populationManager.preadatorPopulationMaxSize)
                    { 
                        GameObject go = Instantiate(gameObject);
                        go.name = "Predator";
                        populationManager.AddToPredatorPopulation(go);
                    }
                }
            }
        }   
    }
}
