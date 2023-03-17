using System.Collections;
using System.Collections.Generic;
using NEAT;
using UnityEngine;
public abstract class Agent : MonoBehaviour
{
    protected float xBorder = 40;
    protected float yBorder = 40;
    protected System.Random random = new System.Random();
    protected static readonly float maxEnergy = 15f;
    protected float energy = maxEnergy;
    public int generation = 0;
    public float timeSurvived = 0f;
    protected int rayCastNumber = 0;
    public double[] inputs = new double[25];
    public double[] outputs = new double[2];
    public Genome Genome { get; set; }
    public Specie Specie { get; set; }

    protected abstract void CreateChild();
    public abstract void NeatSetup();
    public  abstract void selectSpecie(Specie sp);
    protected abstract void createNewSpecie();
    public abstract void removeSpecie();

}
