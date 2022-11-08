using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;

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
    public double[] inputs = new double[25];
    public double[] outputs = new double[2];
    public NEAT.Genome Genome { get; set; }
    System.Random random = new System.Random();

    double MUTATION_RATE = 0.7;
    double ADD_CONN_RATE = 0.4;
    double ADD_NODE_RATE = 0.3;


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
               

                go.GetComponent<Prey>().Genome = NEAT.Genome.Cross(this.Genome, this.Genome, this.random);

                if (random.NextDouble() < MUTATION_RATE) go.GetComponent<Prey>().Genome.Mutation(random);
                if (random.NextDouble() < ADD_CONN_RATE) go.GetComponent<Prey>().Genome.ConnectionMutation(random, populationManager.connInnov);
                if (random.NextDouble() < ADD_NODE_RATE) go.GetComponent<Prey>().Genome.NodeMutation(random, populationManager.nodeInnov, populationManager.connInnov);
                go.name = "Prey";
                populationManager.AddToPreyPopulation(go);

            }
        }
        RaycastHit2D hitInput1 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis((rayCastNumber - 12) * 13f, transform.forward) * transform.right, 6f);
        inputs[rayCastNumber] = hitInput1.collider?.tag == "Predator" ? hitInput1.distance : 10;
        if (rayCastNumber == 24)
        { 
            this.outputs = this.Genome.Compute(inputs);
            rayCastNumber = 0;
        }
        else
            rayCastNumber++;

    }


    public void AddNodeGene(NEAT.NodeGenes node)
    {
        this.Genome.AddNodeGene(node);
    }

    public void ConnectionMutation(System.Random r, NEAT.InnovationGen ConnectionInnov)
    {
        this.Genome.ConnectionMutation(r, ConnectionInnov);
    }


}
