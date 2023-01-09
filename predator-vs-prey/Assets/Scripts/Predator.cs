using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;

public class Predator : MonoBehaviour
{

    //SETTINGS
    float xBorder = 40;
    float yBorder = 40;
    System.Random random = new System.Random();
    PopulationManager populationManager;

    // GAMERULE 
    public int preyEaten = 0;
    int eatenPreyToSplit = 4;
    public bool eaten = false;
    public float eatenCd = 0.25f;
    static float maxEnergy = 15f;
    static float energyConsumption = 1f;
    public float energy = maxEnergy;

    // STATS
    public int generation = 0;
    public int children = 0;
    public float timeSurvived = 0f;
    public int timesFollowed = 0;
    public double currentClosestPrey = 15f;
    public double pastClosestPrey = 15f;

    //LOGIC
    int rayCastNumber = 0;
    // ANN
    public double[] inputs = new double[25];
    public double[] outputs = new double[2];
    public Genome Genome { get; set; }
    public Specie Specie { get; set; }
    double MUTATION_RATE = 0.45;
    double ADD_CONN_RATE = 0.25;
    double ADD_NODE_RATE = 0.15;
    const double C1 = 1.0;
    const double C2 = 1.0;
    const double C3 = 0.4;
    const double DT = 10;

    //MOVEMENT
    public float outputSpeed = 0;
    public float outputRotation = 0;




    private void Awake()
    {
        populationManager = GameObject.Find("PopulationManager").GetComponent<PopulationManager>();
    }
    void Start()
    {
    }

    void Update()
    {
        // STATS TRACKING
        timeSurvived += Time.deltaTime;
        energy = Mathf.Clamp(energy - energyConsumption * Time.deltaTime, 0f, maxEnergy);
        if (energy <= 0)
        {
            populationManager.RemoveFromPredatorPopulation(gameObject);

        }
        if (eaten)
        {
            eatenCd = eatenCd - Time.deltaTime;
            if (eatenCd <= 0f)
            {
                eatenCd = 0.25f;
                eaten = false;
            }
        }

        // RAYCAST
        RaycastHit2D hitInput1 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis((rayCastNumber - 12) * 3f, transform.forward) * transform.right, 67f);
        inputs[rayCastNumber] = hitInput1.collider?.tag.StartsWith("Prey") == true ? (float)(hitInput1.distance / 67f) : 1f;
        if (inputs[rayCastNumber] < currentClosestPrey) 
        {
            currentClosestPrey = inputs[rayCastNumber];
        }
        if (rayCastNumber == 24)
        {
            if (currentClosestPrey < pastClosestPrey)
            {
                timesFollowed++;
            }
            pastClosestPrey = currentClosestPrey;
            currentClosestPrey = 1d;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] < 1f)
                {
                    inputs[i] = 1 - inputs[i];
                }
                else {
                    inputs[i] = -1;
                }
            }

            this.outputs = this.Genome.Compute(inputs);
            outputSpeed = (float)outputs[0] * (float)outputs[0];
            outputRotation = (float)outputs[1];
            rayCastNumber = 0;
        }
        else
        {
            rayCastNumber++;
        }

        // MOVEMENT
        float speed = outputSpeed;
        float rotation = outputRotation;
        transform.position += transform.right * speed * 12f * Time.deltaTime;
        energy -= Mathf.Abs(speed) * Time.deltaTime  ;
        transform.Rotate(new Vector3(0, 0, rotation * 720) * Time.deltaTime);


    }

    private void FixedUpdate()
    {
        // SCREEN WRAPPING LOGIC
        Vector3 newPosition = transform.position;
        if (newPosition.x > xBorder || newPosition.x < -xBorder)
        {
            newPosition.x = newPosition.x > xBorder ? -xBorder : xBorder;
        }

        if (newPosition.y > yBorder || newPosition.y < -yBorder)
        {
            newPosition.y = newPosition.y > yBorder ? -yBorder : yBorder;
        }
        transform.position = newPosition;
    }

    //public void AddNodeGene(NEAT.NodeGenes node)
    //{
    //    this.Genome.AddNodeGene(node);
    //}

    //public void ConnectionMutation(System.Random r, NEAT.InnovationGen ConnectionInnov)
    //{
    //    this.Genome.ConnectionMutation(r, ConnectionInnov);
    //}


    void CreateChild()
    {
        preyEaten = 0;
        eaten = true;
        GameObject go = Instantiate(gameObject);
        // RESET STATS OF CHILD
        go.GetComponent<Predator>().timeSurvived = 0f;
        go.GetComponent<Predator>().children = 0;
        go.GetComponent<Predator>().preyEaten = 0;
        go.GetComponent<Predator>().timesFollowed = 0;
        go.GetComponent<Predator>().generation++;
        go.name = "Predator";

        // GENOME SETUP
        //go.GetComponent<Predator>().Genome = NEAT.Genome.copyGenome(this.Genome);
        go.GetComponent<Predator>().Genome = Genome.Cross(this.Genome, this.Specie.mascot, random);
        if (random.NextDouble() < MUTATION_RATE) go.GetComponent<Predator>().Genome.Mutation(random);
        if (random.NextDouble() < ADD_CONN_RATE) go.GetComponent<Predator>().Genome.ConnectionMutation(random, populationManager.predatorConnInnov);
        if (random.NextDouble() < ADD_NODE_RATE) go.GetComponent<Predator>().Genome.NodeMutation(random, populationManager.predatorNodeInnov, populationManager.predatorConnInnov);
        go.GetComponent<Predator>().selectSpecie(this.Specie);
        populationManager.AddToPredatorPopulation(go);
    }

    public void NeatSetup()
    {
        Genome = new NEAT.Genome();
        for (int i = 0; i < inputs.Length; i++)
        {
            Genome.AddNodeGene(new NodeGenes(NodeGenes.TYPE.INPUT, populationManager.predatorNodeInnov.GetInnovation()));
        }
        Genome.AddNodeGene(new NodeGenes(NodeGenes.TYPE.OUTPUT, populationManager.predatorNodeInnov.GetInnovation()));
        Genome.AddNodeGene(new NodeGenes(NodeGenes.TYPE.OUTPUT, populationManager.predatorNodeInnov.GetInnovation()));
        Genome.ConnectionMutation(random, populationManager.predatorConnInnov);
        Genome.ConnectionMutation(random, populationManager.predatorConnInnov);
        Genome.ConnectionMutation(random, populationManager.predatorConnInnov);
        Genome.ConnectionMutation(random, populationManager.predatorConnInnov);
        Genome.ConnectionMutation(random, populationManager.predatorConnInnov);
        Genome.ConnectionMutation(random, populationManager.predatorConnInnov);
        Genome.Mutation(random);
        Genome.Mutation(random);
        Genome.Mutation(random);
        Genome.Mutation(random);
        Genome.Mutation(random);
        this.createNewSpecie();
    }

    public void selectSpecie(Specie sp)
    {
        bool CreateNew = true;
        double dist = Genome.CompatibilityDistance(Genome, sp.mascot, C1, C2, C3);
        if (dist < DT)
        {
            this.Specie = sp;
            this.Specie.addGO(gameObject);
            CreateNew = false;
        }
        //if (specie.size == 0 || specie.shouldBeRemoved)
        //{
        //    populationManager.predatorSpecies.Remove(specie);
        //}

        if (CreateNew == true)
        {
            this.createNewSpecie();
        }
    }

    private void createNewSpecie()
    {
        this.Specie = new Specie();
        this.Specie.setMascot(this.Genome);
        populationManager.predatorSpecies++;
        this.Specie.addGO(gameObject);
    }

    public void removeSpecie()
    {
       
        this.Specie.removeGO(gameObject);
        if (this.Specie.gos.Count == 0)
            populationManager.predatorSpecies--;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.tag == "Trunk")
        {
            if (collision.gameObject.name.StartsWith("Prey"))
            {
                collision.gameObject.GetComponent<Prey>().removeObject = true;
                preyEaten++;
                energy = Mathf.Clamp(energy + 0.6f * maxEnergy, 0f, maxEnergy);
                if (preyEaten >= eatenPreyToSplit)
                {
                    if (!eaten)
                    {
                        preyEaten = 0;
                        children++;
                    }
                    else
                    {
                        eatenCd = 0.3f;
                        preyEaten = eatenPreyToSplit - 1;
                    }
                    if (populationManager.PredatorPopulation < populationManager.preadatorPopulationMaxSize && !eaten)
                    {
                        this.CreateChild();
                    }
                }
            }
        }
    }
}
