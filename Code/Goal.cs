using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    GameManager gm;
    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter(Collider col)
    {
       if(col.gameObject.tag == "Player")
       {
            if(gm.currentPhase == 3)
            {
                gm.ScorePhase();
            }

            Collider[] allColiders = Physics.OverlapSphere(transform.position, 1f);

            foreach(Collider collider in allColiders)
            {
                if(collider.GetComponent<Rigidbody>() != null)
                {
                    collider.GetComponent<Rigidbody>().AddExplosionForce(3f, transform.position, 1f);
                }
            }
       }
    }
}
