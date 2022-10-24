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
    int rayCastNumber = 0;
    public float[] inputs = new float[25];

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
        RaycastHit2D hitInput1 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis((rayCastNumber - 12) * 13f, transform.forward) * transform.right, 6f);
        inputs[rayCastNumber] = hitInput1.collider?.tag == "Predator" ? hitInput1.distance : 10;
        if (rayCastNumber == 24)
            rayCastNumber = 0;
        else
            rayCastNumber++;

    }

}
