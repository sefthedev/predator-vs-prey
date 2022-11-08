using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;

public class Predator : MonoBehaviour
{
    int preyEaten = 0;
    [SerializeField] int eatenPreyToSplit = 3;
    [SerializeField] static float maxEnergy = 15f;
    [SerializeField] static float energyConsumption = 1f;
    float energy = maxEnergy;
    int rayCastNumber = 0;
    public double[] inputs = new double[25];
    public double[] outputs = new double[2];
    public NEAT.Genome Genome { get; set; }
    System.Random random = new System.Random();

    double MUTATION_RATE = 0.7;
    double ADD_CONN_RATE = 0.4;
    double ADD_NODE_RATE = 0.3;

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
        {
            this.outputs = this.Genome.Compute(inputs);
            rayCastNumber = 0;
        }
        else
            rayCastNumber++;
        

    }

    public void AddNodeGene(NEAT.NodeGenes node) {
        this.Genome.AddNodeGene(node);
    }

    public void ConnectionMutation(System.Random r, NEAT.InnovationGen ConnectionInnov) {
        this.Genome.ConnectionMutation(r, ConnectionInnov);
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
                        go.GetComponent<Predator>().Genome = NEAT.Genome.Cross(this.Genome, this.Genome, this.random);
                        
                        if (random.NextDouble() < MUTATION_RATE) go.GetComponent<Predator>().Genome.Mutation(random);
                        if (random.NextDouble() < ADD_CONN_RATE) go.GetComponent<Predator>().Genome.ConnectionMutation(random, populationManager.connInnov);
                        if (random.NextDouble() < ADD_NODE_RATE) go.GetComponent<Predator>().Genome.NodeMutation(random, populationManager.nodeInnov, populationManager.connInnov);
                        go.name = "Predator";
                        populationManager.AddToPredatorPopulation(go);
                    }
                }
            }
        }   
    }
}
