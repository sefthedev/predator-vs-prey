using NEAT;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum AGENTTYPE { PREDATOR, PREY };

public class PopulationManager : MonoBehaviour
{
    
    //SETTINGS

    System.Random random = new System.Random();
    
    //PREFABS
    [SerializeField] GameObject predatorPrefab;
    [SerializeField] GameObject preyPrefab;
    //STATS

    private int predatorNodeCount = 0;
    private int predatorConnectionCount = 0;
    private int predatorGenCount = 0;
    private int preyNodeCount = 0;
    private int preyConnectionCount = 0;
    private int preyGenCount = 0;
    private float passedTime = 0.0f;
    public int predatorSpecies = 0;
    public int preySpecies = 0;

    //POPULATION
    List<GameObject> predatorPopulation = new List<GameObject>();
    List<GameObject> preyPopulation = new List<GameObject>();


    //INNOVATION
    public InnovationGen predatorNodeInnov = new InnovationGen ();
    public InnovationGen predatorConnInnov = new InnovationGen();
    public InnovationGen preyNodeInnov = new InnovationGen();
    public InnovationGen preyConnInnov = new InnovationGen();
    //int bestPredatorPoints = 0;
    //public Genome bestPredatorGenome;
    //int bestPreyPoints = 0;
    //public Genome bestPreyGenome;
    //WEIGHTS FOR SPECIE
    int durchlauf = 0;






    void Start()
    {
        // SETTINGS
        Application.runInBackground = true;
        for (int i = 0; i < SimulationRules.predatorPopulationMaxSize; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-SimulationRules.xBorder, SimulationRules.xBorder), Random.Range(-SimulationRules.yBorder, SimulationRules.yBorder), 0);
            Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 365));
            GameObject go = Instantiate(predatorPrefab, pos, rot);
            go.GetComponent<Predator>().NeatSetup();
            this.UpdateAgentPopulation(go, true, AGENTTYPE.PREDATOR);
        }

        for (int i = 0; i < SimulationRules.preyPopulationMaxSize; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-SimulationRules.xBorder, SimulationRules.xBorder), Random.Range(-SimulationRules.yBorder, SimulationRules.yBorder), 0);
            Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 365));
            GameObject go = Instantiate(preyPrefab, pos, rot);
            go.GetComponent<Prey>().NeatSetup();
            this.UpdateAgentPopulation(go, true, AGENTTYPE.PREY);
        }
        string filePath = Application.dataPath + "/predatorVprey.csv";


        using (StreamWriter sw = new StreamWriter(filePath))
        {
            sw.WriteLine("Time,AvgPredGen,AvgPredNodes,AvgPredConnections,AvgPreyGen,AvgPreyNodes,AvgPreyConnections,predatorPop,predatorSpecies,preyPopulation,preySpecies");

        }
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine($"{passedTime},{System.Math.Round((float)predatorGenCount / predatorPopulation.Count)},{System.Math.Round((float)predatorNodeCount / predatorPopulation.Count)}," +
                $"{System.Math.Round((float)predatorConnectionCount / predatorPopulation.Count)},{System.Math.Round((float)preyGenCount / preyPopulation.Count)},{System.Math.Round((float)preyNodeCount / preyPopulation.Count)},{System.Math.Round((float)preyConnectionCount / preyPopulation.Count)}," +
                $"{this.predatorPopulation.Count},{this.predatorSpecies},{this.preyPopulation.Count},{this.preySpecies}");
        }
        passedTime = (float)System.Math.Round(passedTime + 0.05f, 2);
        InvokeRepeating("WriteStats", 3.0f, 3.0f);

    }


    public void UpdateAgentPopulation(GameObject go, bool addAgent, AGENTTYPE agentType)
    {
        if (agentType == AGENTTYPE.PREDATOR && addAgent)
        {
            predatorConnectionCount += go.GetComponent<Predator>().Genome.connections.Count;
            predatorNodeCount += go.GetComponent<Predator>().Genome.nodes.Count;
            predatorGenCount += go.GetComponent<Predator>().generation;
            predatorPopulation.Add(go);
        }
        if (agentType == AGENTTYPE.PREDATOR && !addAgent)
        {
            predatorPopulation.Remove(go);
            //if (go.GetComponent<Predator>().Genome.Specie.bestScore < (go.GetComponent<Predator>().children * 0.1f + go.GetComponent<Predator>().timesFollowedPrey * 0.5f + go.GetComponent<Predator>().timeSurvived * 0.1f))
            if (go.GetComponent<Predator>().Genome.Specie.bestScore < (go.GetComponent<Predator>().children * 0.1f + go.GetComponent<Predator>().timeSurvived * 0.1f))
            {
                go.GetComponent<Predator>().Genome.Specie.bestScore = go.GetComponent<Predator>().children * 0.1f + go.GetComponent<Predator>().timeSurvived * 0.1f;
                go.GetComponent<Predator>().Genome.Specie.setMascot(Genome.copyGenome(go.GetComponent<Predator>().Genome));
            }
            predatorConnectionCount -= go.GetComponent<Predator>().Genome.connections.Count;
            predatorNodeCount -= go.GetComponent<Predator>().Genome.nodes.Count;
            predatorGenCount -= go.GetComponent<Predator>().generation;

            go.GetComponent<Predator>().removeSpecie();
            Destroy(go);
        }

        if (agentType == AGENTTYPE.PREY && addAgent)
        {
            preyConnectionCount += go.GetComponent<Prey>().Genome.connections.Count;
            preyNodeCount += go.GetComponent<Prey>().Genome.nodes.Count;
            preyGenCount += go.GetComponent<Prey>().generation;
            preyPopulation.Add(go);
        }

        if (agentType == AGENTTYPE.PREY && !addAgent)
        {
            if (go.GetComponent<Prey>().Genome.Specie.bestScore < (go.GetComponent<Prey>().timesEscaped * 0.5f + go.GetComponent<Prey>().timeSurvived * 0.1f))
            {
                go.GetComponent<Prey>().Genome.Specie.bestScore = go.GetComponent<Prey>().timesEscaped * 0.5f + go.GetComponent<Prey>().timeSurvived * 0.1f;
                go.GetComponent<Prey>().Genome.Specie.setMascot(Genome.copyGenome(go.GetComponent<Prey>().Genome));
            }
            preyConnectionCount -= go.GetComponent<Prey>().Genome.connections.Count;
            preyNodeCount -= go.GetComponent<Prey>().Genome.nodes.Count;
            preyGenCount -= go.GetComponent<Prey>().generation;
            preyPopulation.Remove(go);
            Destroy(go);
        }
    }
   

    void Update()
    {
        if (durchlauf >= preyPopulation.Count) {
            durchlauf = 0;
        }
        if (!preyPopulation[durchlauf].activeSelf)
        {
            Debug.Log("Im inactive and i am inside PreyPopulation");
            StartCoroutine(DoSomethingAfterTenSeconds(preyPopulation[durchlauf]));
        }
        durchlauf++;
    }

    IEnumerator DoSomethingAfterTenSeconds(GameObject go)
    {
        yield return new WaitForSeconds(10);
        UpdateAgentPopulation(go, false, AGENTTYPE.PREY);
        go.GetComponent<Prey>().removeSpecie();
        if (preyPopulation.Contains(go)) {
            Debug.Log("I still exist");
        }
    }

    private void FixedUpdate()
    {
        Debug.Log("||||||||||||  average  Pred/Prey connectionCount:     " + System.Math.Round((float)predatorConnectionCount/predatorPopulation.Count) + "/" + System.Math.Round((float)preyConnectionCount / preyPopulation.Count) +
            "||||||||||||   average Pred/Prey nodeCount:     " + System.Math.Round((float)predatorNodeCount / predatorPopulation.Count)+ "/" + System.Math.Round((float)preyNodeCount / preyPopulation.Count) +
            "||||||||||||   average Pred/Prey Generation:     " + System.Math.Round((float)predatorGenCount/predatorPopulation.Count) + "/" + System.Math.Round((float)preyGenCount/preyPopulation.Count)+
            "||||||||||||   predatorPopulation:     " + this.predatorPopulation.Count + "||||||||||||   preyPopulation:     " + this.preyPopulation.Count +
            "||||||||||||   predatorSpeciesCount:     " + this.predatorSpecies + "||||||||||||   preySpeciesCount:     " + this.preySpecies);
    }

    private void WriteStats()
    {
        string filePath = Application.dataPath + "/predatorVprey.csv";


        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine($"{passedTime},{System.Math.Round((float)predatorGenCount / predatorPopulation.Count)},{System.Math.Round((float)predatorNodeCount / predatorPopulation.Count)}," +
                $"{System.Math.Round((float)predatorConnectionCount / predatorPopulation.Count)},{System.Math.Round((float)preyGenCount / preyPopulation.Count)},{System.Math.Round((float)preyNodeCount / preyPopulation.Count)},{System.Math.Round((float)preyConnectionCount / preyPopulation.Count)}," +
                $"{this.predatorPopulation.Count},{this.predatorSpecies},{this.preyPopulation.Count},{this.preySpecies}");
        }

        passedTime = (float)System.Math.Round(passedTime + 0.05f, 2);

    }

    public int PreyPopulation
    {
        get { return preyPopulation.Count; }
    }


    public int PredatorPopulation
    {
        get { return predatorPopulation.Count; }
    }



}

