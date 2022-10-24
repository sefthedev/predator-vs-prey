using UnityEngine;

public class PredatorRaycast : MonoBehaviour
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
        RaycastHit2D hitInput1 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis((rayCastNumber - 12) * 1.6f, transform.forward) * transform.right, 10f);
        if (rayCastNumber == 24)
            rayCastNumber = 0;
        else
            rayCastNumber++;

        //Debug.Log(hitInput1.distance);
        //if (hitInput1.collider != null)
        //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(0 * 1.6f, transform.forward) * transform.right * hitInput1.distance, Color.white);
        //else
        //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(0 * 1.6f, transform.forward) * transform.right * 10f, Color.white);

        //for (int i = 1; i <= 12; i++)
        //{

        //    RaycastHit2D hitInput2 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis(i * 1.6f, transform.forward) * transform.right, 10f);
        //    //Debug.Log(hitInput1.distance);
        //    //if (hitInput1.collider != null)
        //    //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(i * 1.6f, transform.forward) * transform.right * hitInput1.distance, Color.white);
        //    //else
        //    //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(i * 1.6f, transform.forward) * transform.right * 10f, Color.white);

        //    hitInput2 = Physics2D.Raycast(this.transform.position, Quaternion.AngleAxis(-i * 1.6f, transform.forward) * transform.right, 10f);
        //    //Debug.Log(hitInput1.distance);
        //    //if (hitInput1.collider != null)
        //    //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(-i * 1.6f, transform.forward) * transform.right * hitInput1.distance, Color.white);
        //    //else
        //    //    Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(-i * 1.6f, transform.forward) * transform.right * 10f, Color.white);


        //}


    }

    private void FixedUpdate()

    {
      
    }
}
