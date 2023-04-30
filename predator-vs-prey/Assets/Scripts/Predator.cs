using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;
using System;

public class Predator : Agent
{

    //SETTINGS
    
    PopulationManager populationManager;

    // GAMERULE 
    int preyEaten = 0;
    bool eaten = false;
    float eatenCd = 0.25f;


    // STATS
    public int children = 0;
    public int timesFollowedPrey = 0;
    float currentClosestPrey = 15f;
    float lastClosestPrey = 15f;






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
        energy = Mathf.Clamp(energy - Time.deltaTime, 0f, SimulationRules.maxEnergy);
        if (energy <= 0)
        {
            populationManager.UpdateAgentPopulation(gameObject, false, AGENTTYPE.PREDATOR);

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
        RaycastHit2D hitInput1 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis((rayCastNumber - 12) * 2f, transform.forward) * transform.right, 67f);
        inputs[rayCastNumber] = hitInput1.collider?.tag.StartsWith("Prey") == true ? (float)(hitInput1.distance / 67f) : 1f;
        if (inputs[rayCastNumber] < currentClosestPrey)
        {
            currentClosestPrey = (float)inputs[rayCastNumber];
        }
        if (rayCastNumber == 24)
        {
            if (currentClosestPrey < lastClosestPrey)
            {
                timesFollowedPrey++;
            }
            lastClosestPrey = currentClosestPrey;
            currentClosestPrey = 1f;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] < 1f)
                {
                    inputs[i] = 1 - inputs[i];
                }
                else
                {
                    inputs[i] = -1;
                }
            }

            this.outputs = this.Genome.Compute(inputs);
            outputSpeed = Mathf.Abs((float)outputs[0]);
            outputRotation = (float)outputs[1];
            rayCastNumber = 0;
        }
        else
        {
            rayCastNumber++;
        }

        // MOVEMENT
        float speed = outputSpeed * 10f;
        float rotation = outputRotation;
        transform.position += transform.right * speed * Time.deltaTime;
        energy -= Mathf.Abs(speed / 10f) * Time.deltaTime  ;
        transform.Rotate(new Vector3(0, 0, rotation * 360f) * Time.deltaTime);


    }



    //public void AddNodeGene(NEAT.NodeGenes node)
    //{
    //    this.Genome.AddNodeGene(node);
    //}

    //public void ConnectionMutation(System.Random r, NEAT.InnovationGen ConnectionInnov)
    //{
    //    this.Genome.ConnectionMutation(r, ConnectionInnov);
    //}


    protected override void CreateChild()
    {
        double MUTATION_RATE = 0.45;
        double ADD_CONN_RATE = 0.25;
        double ADD_NODE_RATE = 0.15;

        preyEaten = 0;
        eaten = true;
        GameObject go = Instantiate(gameObject);
        // RESET STATS OF CHILD
        go.GetComponent<Predator>().timeSurvived = 0f;
        go.GetComponent<Predator>().children = 0;
        go.GetComponent<Predator>().preyEaten = 0;
        go.GetComponent<Predator>().timesFollowedPrey = 0;
        go.GetComponent<Predator>().generation++;
        go.name = "Predator";

        // GENOME SETUP
        //go.GetComponent<Predator>().Genome = NEAT.Genome.copyGenome(this.Genome);
        go.GetComponent<Predator>().Genome = Genome.Cross(this.Genome, this.Genome.Specie.mascot, random);
        if (random.NextDouble() < MUTATION_RATE) go.GetComponent<Predator>().Genome.Mutation(random);
        if (random.NextDouble() < ADD_CONN_RATE) go.GetComponent<Predator>().Genome.ConnectionMutation(random, populationManager.predatorConnInnov);
        if (random.NextDouble() < ADD_NODE_RATE) go.GetComponent<Predator>().Genome.NodeMutation(random, populationManager.predatorNodeInnov, populationManager.predatorConnInnov);
        go.GetComponent<Predator>().selectSpecie(this.Genome.Specie);
        populationManager.UpdateAgentPopulation(go, true, AGENTTYPE.PREDATOR);
    }

    public override void NeatSetup()
    {
        Genome = new NEAT.Genome();
        for (int i = 0; i < inputs.Length; i++)
        {
            Genome.AddNodeGene(new NodeGenes(NodeGenes.TYPE.INPUT, populationManager.predatorNodeInnov.GetInnovation()));
        }
        Genome.AddNodeGene(new NodeGenes(NodeGenes.TYPE.OUTPUT, populationManager.predatorNodeInnov.GetInnovation()));
        Genome.AddNodeGene(new NodeGenes(NodeGenes.TYPE.OUTPUT, populationManager.predatorNodeInnov.GetInnovation()));
        Genome.ConnectionMutation(random, populationManager.predatorConnInnov);
        Genome.Mutation(random);
        this.createNewSpecie();
    }

    public override void selectSpecie(Specie sp)
    {
        const double C1 = 1.0;
        const double C2 = 1.0;
        const double C3 = 0.4;
        const double DT = 5f;

        bool CreateNew = true;
        double dist = Genome.CompatibilityDistance(Genome, sp.mascot, C1, C2, C3);
        if (dist < DT)
        {
            this.Genome.Specie = sp;
            this.Genome.Specie.addMember();
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

    protected override void createNewSpecie()
    {
        this.Genome.Specie = new Specie();
        this.Genome.Specie.setMascot(this.Genome);
        populationManager.predatorSpecies++;
        this.Genome.Specie.addMember();
    }

    public override void removeSpecie()
    {
       
        this.Genome.Specie.removeMember();
        if (this.Genome.Specie.memberCount == 0)
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
                energy = Mathf.Clamp(energy + 0.4f * SimulationRules.maxEnergy, 0f, SimulationRules.maxEnergy);
                if (preyEaten >= SimulationRules.eatenPreyToSplit)
                {
                    if (!eaten)
                    {
                        preyEaten = 0;
                        children++;
                    }
                    else
                    {
                        eatenCd = 0.25f;
                        preyEaten = SimulationRules.eatenPreyToSplit - 1;
                    }
                    if (populationManager.PredatorPopulation < SimulationRules.predatorPopulationMaxSize && !eaten)
                    {
                        this.CreateChild();
                    }
                }
            }
        }
    }
}
