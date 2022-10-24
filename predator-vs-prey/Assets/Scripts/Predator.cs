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
    int rayCastNumber = 0;
    public float[] inputs = new float[25];

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
        RaycastHit2D hitInput1 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis((rayCastNumber - 12) * 1.6f, transform.forward) * transform.right, 10f);
        inputs[rayCastNumber] = hitInput1.collider?.tag == "Prey" ?  hitInput1.distance  : 10 ;
        if (rayCastNumber == 24)
            rayCastNumber = 0;
        else
            rayCastNumber++;
        

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
