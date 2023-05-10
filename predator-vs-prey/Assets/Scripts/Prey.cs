using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;
using System;

public class Prey : Agent
{


    //SETTINGS
    PopulationManager populationManager;

    // GAMERULE 


    // STATS
    public int timesEscaped = 0;

    // LOGIC
    Vector3 pos;
    float splitTime = 0f;
    bool escaping = false;
    bool inDanger = false;
    bool exhausted = false;
    public bool removeObject = false;


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
            populationManager.UpdateAgentPopulation(gameObject, false, AGENTTYPE.PREY);
            return;
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
                transform.position += transform.right * speed * 10f * Time.deltaTime;

                energy -= Mathf.Abs(speed) * Time.deltaTime ;
            }
        }
        else
        {
            exhausted = true;
        }
        transform.Rotate(new Vector3(0, 0, rotation * 360f) * Time.deltaTime);
    }


    protected override void CreateChild()
    {
        //double MUTATION_RATE = 0.045 + 0.4 * Math.Pow(0.5, this.generation / 200);
        //double ADD_CONN_RATE = 0.025 + 0.2 * Math.Pow(0.5, this.generation / 200);
        //double ADD_NODE_RATE = 0.015 + 0.1 * Math.Pow(0.5, this.generation / 200);


        double MUTATION_RATE = 0.1 + (0.1 - 0.1 * Math.Pow(Math.E, -0.005 * generation));
        double ADD_CONN_RATE = 0.04 + 0.1 * Math.Pow(0.5, this.generation / 200);
        double ADD_NODE_RATE = 0.02 + 0.1 * Math.Pow(0.5, this.generation / 200);

        GameObject go = Instantiate(gameObject);
        // RESETTING STATS
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
        Genome.Mutation(random);
        this.createNewSpecie();
    }

    public override void selectSpecie(Specie sp)
    {
        const double C1 = 1.0;
        const double C2 = 1.0;
        const double C3 = 0.4;
        double DT = 5 + (this.generation / 1500f);

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
