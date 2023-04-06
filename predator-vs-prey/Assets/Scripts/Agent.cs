using System.Collections;
using System.Collections.Generic;
using NEAT;
using UnityEngine;
public abstract class Agent : MonoBehaviour
{
    protected System.Random random = new System.Random();

    protected float energy = SimulationRules.maxEnergy;
    public int generation = 0;
    public float timeSurvived = 0f;
    protected int rayCastNumber = 0;
    public double[] inputs = new double[25];
    public double[] outputs = new double[2];
    public Genome Genome { get; set; }
    //MOVEMENT
    protected float outputSpeed = 0;
    protected float outputRotation = 0;
    //public Specie Specie { get; set; }


    protected void FixedUpdate()
    {
        // SCREEN WRAPPING LOGIC
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
    protected abstract void CreateChild();
    public abstract void NeatSetup();


}
