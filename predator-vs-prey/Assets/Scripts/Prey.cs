using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;

public class Prey : Agent
{


    //SETTINGS
    PopulationManager populationManager;

    // GAMERULE 


    // STATS
    float distanceTraveled = 0f;
    public int timesEscaped = 0;

    // LOGIC
    Vector3 pos;
    float splitTime = 0f;
    bool escaping = false;
    bool inDanger = false;
    bool exhausted = false;
    public bool removeObject = false;



    //MOVEMENT
    float outputSpeed = 0;
    float outputRotation = 0;


    private void Awake()
    {
        populationManager = GameObject.Find("PopulationManager").GetComponent<PopulationManager>();
    }

    void Start()
    {
        pos = transform.position;
    }

    void Update()
    {
        if (removeObject)
        {
            this.gameObject.SetActive(false);
            populationManager.UpdateAgentPopulation(gameObject, false, AGENTTYPE.PREY);
            this.removeSpecie();
            Destroy(this.gameObject);
        }
        // STATS TRACKING
        timeSurvived += Time.deltaTime;
        if (transform.position == pos)
        {
            if (energy < SimulationRules.maxEnergy)
            {
                energy = Mathf.Clamp(energy + Time.deltaTime, 0f, SimulationRules.maxEnergy);
            }
        }
        else
        {
            distanceTraveled += Vector3.Distance(pos, transform.position);
            pos = transform.position;
        }
        if (exhausted && energy > (SimulationRules.maxEnergy / 2f))
        {
            exhausted = false;
        }
        splitTime += Time.deltaTime;
        if (splitTime > SimulationRules.timeToSplit)
        {
            splitTime = 0f;
            if (populationManager.PreyPopulation < SimulationRules.preyPopulationMaxSize)
            {
                this.CreateChild();
            }
        }

        // RAYCAST
        RaycastHit2D hitInput1 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis((rayCastNumber - 12) * 13f, transform.forward) * transform.right, 33f); 
        if (hitInput1.collider?.tag == "Predator")
        {
            inputs[rayCastNumber] = (float)(hitInput1.distance / 33f);
            if (!escaping)
            {
                escaping = true;
            }
        }
        else
        {
            inputs[rayCastNumber] = 1f;
        }
        //inputs[rayCastNumber] = hitInput1.collider?.tag == "Predator" ? ((hitInput1.distance / 10) * 2) - 1 : 1;
        if (rayCastNumber == 24)
        {
            this.outputs = Genome.Compute(inputs);
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] < 1f)
                {
                    inputs[i] = 1 - inputs[i];
                }
                else 
                {
                    inputs[i] = -1f;
                }
            }
            this.outputs = this.Genome.Compute(inputs);
            outputSpeed = (float)outputs[0];
            outputRotation = (float)outputs[1];
            rayCastNumber = 0;

            if (inDanger && !escaping)
            {
                inDanger = false;
                timesEscaped++;
            }
            else
            {
                if (escaping && !inDanger)
                {
                    inDanger = true;
                }
                escaping = false;
            }
        }
        else
        {
            rayCastNumber++;
        }

        //MOVEMENT
        float speed = outputSpeed;
        float rotation = outputRotation;

        if ((energy - Mathf.Abs(speed) * Time.deltaTime) > 0)
        {
            if (!exhausted)
            {
                transform.position += transform.right * speed * 12f * Time.deltaTime;

                energy -= Mathf.Abs(speed) * Time.deltaTime ;
            }
        }
        else
        {
            exhausted = true;
        }
        transform.Rotate(new Vector3(0, 0, rotation * 720) * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // SCREN WRAPPING
        Vector3 newPosition = transform.position;
        if (newPosition.x > SimulationRules.xBorder || newPosition.x < -SimulationRules.xBorder)
        {
            newPosition.x = newPosition.x > SimulationRules.xBorder ? -SimulationRules.xBorder : SimulationRules.xBorder;
        }

        if (newPosition.y > SimulationRules.yBorder || newPosition.y < -SimulationRules.yBorder)
        {
            newPosition.y = newPosition.y > SimulationRules.yBorder ? -SimulationRules.yBorder : SimulationRules.yBorder;
        }
        transform.position = newPosition;
    }

    protected override void CreateChild()
    {
        double MUTATION_RATE = 0.45;
        double ADD_CONN_RATE = 0.25;
        double ADD_NODE_RATE = 0.15;

        GameObject go = Instantiate(gameObject);
        // RESETTING STATS
        go.GetComponent<Prey>().distanceTraveled = 0f;
        go.GetComponent<Prey>().timesEscaped = 0;
        go.GetComponent<Prey>().timeSurvived = 0f;
        go.GetComponent<Prey>().splitTime += (float)random.NextDouble();
        go.GetComponent<Prey>().generation++;
        go.name = "Prey";

        //GENOME
        //go.GetComponent<Prey>().Genome = NEAT.Genome.copyGenome(this.Genome);

        go.GetComponent<Prey>().Genome = Genome.Cross(this.Genome, this.Genome.Specie.mascot, random);
        if (random.NextDouble() < MUTATION_RATE) go.GetComponent<Prey>().Genome.Mutation(random);
        if (random.NextDouble() < ADD_CONN_RATE) go.GetComponent<Prey>().Genome.ConnectionMutation(random, populationManager.preyConnInnov);
        if (random.NextDouble() < ADD_NODE_RATE) go.GetComponent<Prey>().Genome.NodeMutation(random, populationManager.preyNodeInnov, populationManager.preyConnInnov);
        //if (random.NextDouble() < (1 - 1f / ((100f / ((float)(this.generation + 1)) / 50f)))+MUTATION_RATE) go.GetComponent<Prey>().Genome.Mutation(random);
        //if (random.NextDouble() < (1 - 1f / ((100f / ((float)(this.generation + 1)) / 30f))) + ADD_CONN_RATE) go.GetComponent<Prey>().Genome.ConnectionMutation(random, populationManager.connInnov);
        //if (random.NextDouble() < (1 - 1f / ((100f /((float)(this.generation + 1)) / 20f)))+ ADD_NODE_RATE) go.GetComponent<Prey>().Genome.NodeMutation(random, populationManager.nodeInnov, populationManager.connInnov
        go.GetComponent<Prey>().selectSpecie(this.Genome.Specie);
        populationManager.UpdateAgentPopulation(go, true, AGENTTYPE.PREY);
    }

    public override void NeatSetup()
    {
        Genome = new NEAT.Genome();
        for (int i = 0; i < inputs.Length; i++)
        {
            Genome.AddNodeGene(new NodeGenes(NodeGenes.TYPE.INPUT, populationManager.preyNodeInnov.GetInnovation()));
        }
        Genome.AddNodeGene(new NodeGenes(NodeGenes.TYPE.OUTPUT, populationManager.preyNodeInnov.GetInnovation()));
        Genome.AddNodeGene(new NodeGenes(NodeGenes.TYPE.OUTPUT, populationManager.preyNodeInnov.GetInnovation()));

        Genome.ConnectionMutation(random, populationManager.preyConnInnov);
        Genome.ConnectionMutation(random, populationManager.preyConnInnov);
        Genome.Mutation(random);
        Genome.Mutation(random);
        this.createNewSpecie();
    }

    public override void selectSpecie(Specie sp)
    {
        const double C1 = 1.0;
        const double C2 = 1.0;
        const double C3 = 0.4;
        const double DT = 5;

        bool CreateNew = true;
        double dist = Genome.CompatibilityDistance(Genome, sp.mascot, C1, C2, C3);
        if (dist < DT)
        {
            this.Genome.Specie = sp;
            this.Genome.Specie.addMember();
            CreateNew = false;
        }

        if (CreateNew == true)
        {
            this.createNewSpecie();
        }
    }

    protected override void createNewSpecie()
    {
        this.Genome.Specie = new Specie();
        this.Genome.Specie.setMascot(this.Genome);
        populationManager.preySpecies++;
        this.Genome.Specie.addMember();
    }

    public override void removeSpecie()
    {
        
        this.Genome.Specie.removeMember();
        if (this.Genome.Specie.memberCount == 0)
            populationManager.preySpecies--;
    }

}
