using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyRaycast : MonoBehaviour
{
    public float[] inputs;
    int rayCastNumber = 0;


    // Start is called before the first frame update
    void Start()
    {
        Physics.autoSyncTransforms = true;
    }

    // Update is called once per frame
    void Update()
    {

       
        
        RaycastHit2D hitInput1 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis((rayCastNumber-12) * 13f, transform.forward) * transform.right, 6f);
        if (rayCastNumber == 24)
            rayCastNumber = 0;
        else
            rayCastNumber++;
        

        //Debug.Log(hitInput1.distance);
        //if (hitInput1.collider != null)
        //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(0 * 13f, transform.forward) * transform.right * hitInput1.distance, Color.black);
        //else
        //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(0 * 13f, transform.forward) * transform.right * 6f, Color.black);

        //for (int i = 1; i <= 12; i++)
        //{

        //    RaycastHit2D hitInput2 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis(i * 13f, transform.forward) * transform.right, 6f);
        //    //Debug.Log(hitInput1.distance);
        //    //if (hitInput1.collider != null)
        //    //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(i * 13f, transform.forward) * transform.right * hitInput1.distance, Color.black);
        //    //else
        //    //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(i * 13f, transform.forward) * transform.right * 6f, Color.black);

        //    hitInput1 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis(-i * 13f, transform.forward) * transform.right, 6f);
        //    //Debug.Log(hitInput1.distance);
        //    //if (hitInput1.collider != null)
        //    //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(-i * 13f, transform.forward) * transform.right * hitInput1.distance, Color.black);
        //    //else
        //    //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(-i * 13f, transform.forward) * transform.right * 6f, Color.black);


        //}

    }

    private void FixedUpdate()

    {
        
       

    }
}
