using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chicken : MonoBehaviour
{
    private float state;
    private float startDelay = 0;
    private float repeatRate = 5;
    private Animator playerAnim;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("StateControl", startDelay, repeatRate);
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state>4)
        {
            playerAnim.SetBool("Eat_b", true);
        }
        else
        {
            playerAnim.SetBool("Eat_b", false);
        }
    }
    void StateControl()
    {
        state = Random.Range(0, 10);
    }
}
