using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoam : MonoBehaviour
{
    private GameObject tree;
    private GameObject cowother;
    private GameObject dog;
    public float speed_walk = 1.2f;
    private float state_walk = 4;
    public float speed_turn = 0;
    public float angle;
    public float mark;
    public bool ifinside = true;
    private float startDelay = 0;
    private float repeatRate = 5;
    private Animator playerAnim;
    private bool flag = false;
    public int cowflag = 0;
    public bool treeflag = false;
    public float x;
    public float z;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("StateControl", startDelay, repeatRate);
        playerAnim = GetComponent<Animator>();
        tree = GameObject.Find("oakTree");
        if (transform.position == GameObject.Find("Animal_Cow_White").transform.position)
            cowother = GameObject.Find("Animal_Cow_White (1)");
        else
            cowother = GameObject.Find("Animal_Cow_White");
        dog = GameObject.Find("Dog_SaintBernard_01");
    }

    // Update is called once per frame
    void Update()
    {
        x = transform.position.x;
        z = transform.position.z;
        treeflag = false;
        //if(!ifout())
        if (!ifout() && !ifcoll())
        {
            if (state_walk >= 5)
            {
                playerAnim.SetBool("Eat_b", false);
                transform.Translate(Vector3.forward * Time.deltaTime * speed_walk);
                transform.Rotate(Vector3.up, angle * Time.deltaTime * speed_turn);
            }
            else
            {
                playerAnim.SetBool("Eat_b", true);
            }
        }
        else
        {
            //Debug.Log("divert");
            playerAnim.SetBool("Eat_b", false);
            divert();
        }


    }
    void StateControl()
    {
        state_walk = Random.Range(0, 10);
        speed_turn = Random.Range(0, 15);
        angle = Random.Range(-1, 1);
        //transform.Rotate(Vector3.up, Time.deltaTime * 20);

    }

    void divert()
    {
        //Debug.Log("divert");
        transform.Translate(Vector3.forward * Time.deltaTime * speed_walk);
        if (ifout())
        {
            if (mark > 0 && mark < 90)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * 50);
            }
            else if (mark > 90 && mark < 180)
            {
                transform.Rotate(Vector3.up, -Time.deltaTime * 50);
            }
            else if (mark > 180 && mark < 270)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * 50);
            }
            else
            {
                transform.Rotate(Vector3.up, -Time.deltaTime * 50);
            }
        }
        else if (treeflag == true)
        {
            Vector3 direction = tree.transform.position - transform.position;
            float degree = Vector3.Angle(direction, transform.forward);
            if (degree > 0 && degree < 120)
                transform.Rotate(Vector3.up, -Time.deltaTime * 50);
            else if (degree < 0 && degree > -120)
                transform.Rotate(Vector3.up, Time.deltaTime * 50);
        }
        else
        {
            Vector3 direction;
            if (cowflag == 1)
                direction = cowother.transform.position - transform.position;
            else
                direction = dog.transform.position - transform.position;
            float degree = Vector3.Angle(direction, transform.forward);
            if (degree > 0 && degree < 120)
                transform.Rotate(Vector3.up, -Time.deltaTime * 50);
            else if (degree < 0 && degree > -120)
                transform.Rotate(Vector3.up, Time.deltaTime * 50);
        }
    }

    bool ifcoll()
    {
        float distance = (transform.position - cowother.transform.position).magnitude;
        float distance2 = (transform.position - dog.transform.position).magnitude;
        float distance3 = (transform.position - tree.transform.position).magnitude;
        cowflag = 0;
        if (distance3 < 8)
        {
            treeflag = true;
            return true;
        }
        else
            treeflag = false;
        if (distance > 9 && distance2 > 7)
        {
            return false;
        }
        else if (distance < 9)
        {
            //Debug.Log("collision cows");
            cowflag = 1;
            return true;
        }
        else
        {
            //Debug.Log("collision dog");
            cowflag = 2;
            return true;
        }
    }
    

    bool ifout()
    {
        if (transform.position.z < -15 || transform.position.z > 15 || transform.position.x > 2 || transform.position.x < -19)
        {
            if (flag == false)
            {
                mark = transform.localEulerAngles.y % 360;
                flag = true;
                Debug.Log("out");
                return true;
            }
            if (transform.position.z < -15 && (transform.localEulerAngles.y%360 < 30 || transform.localEulerAngles.y%360 > 330))
            {
                Debug.Log("in1");
                return false;
            }
            else if (transform.position.z > 15 && (transform.localEulerAngles.y % 360 > 150 && transform.localEulerAngles.y % 360 < 210))
            {
                Debug.Log("in2");
                return false;
            }
            else if (transform.position.x > 2 && (transform.localEulerAngles.y % 360 > 240 && transform.localEulerAngles.y % 360 < 300))
            {
                Debug.Log("in3");
                return false;
            }
            else if (transform.position.x < -19 && (transform.localEulerAngles.y % 360 > 60 && transform.localEulerAngles.y % 360 < 120))
            {
                Debug.Log("in4");
                return false;
            }
            else
            {
                Debug.Log("out");
                return true;
            }
        }
        else
        {
            Debug.Log("in5");
            flag = false;
            return false;
        }
    }

}
