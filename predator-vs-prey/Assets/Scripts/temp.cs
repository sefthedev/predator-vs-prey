using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class temp : MonoBehaviour
{
    public GameObject one;
    public GameObject two;
    public GameObject three;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 25; i++)
        {

            RaycastHit2D hitInput1 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis((i - 12) * 3f, transform.forward) * transform.right, 33f);
            Debug.DrawRay(this.transform.position, Quaternion.AngleAxis((i - 12) * 3f, transform.forward) * transform.right * 67f, Color.blue);
        }
    }
}
