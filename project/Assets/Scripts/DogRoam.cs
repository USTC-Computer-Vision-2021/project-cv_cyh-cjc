using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogRoam : MonoBehaviour
{
    private GameObject tree;
	private GameObject cow1;
	private GameObject cow2;
    public float speed_walk = 2.0f;
    private float state_walk = 4;
    public float speed_turn = 0;
    public float angle;
    private float mark;
    public bool ifinside = true;
    private float startDelay = 0;
    private float repeatRate = 10;
    private Animator playerAnim;
    private bool flag = false;
	private int cowflag = 0;
	private bool treeflag = false;
	private GameObject target;
    public int state_target;
	

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("StateControl", startDelay, repeatRate);
        playerAnim = GetComponent<Animator>();
        tree = GameObject.Find("oakTree");
		cow2 = GameObject.Find("Animal_Cow_White (1)");
		cow1 = GameObject.Find("Animal_Cow_White");
		target = cow1;
    }

    // Update is called once per frame
    void Update()
    {
        treeflag = false;
        if(state_target>=0)
        {
            target = cow1;
        }
        else
        {
            target = cow2;
        }
        if (ifcoll() && ((cowflag == 1 && target == cow1) || (cowflag == 2 && target == cow2)))
        {
            Debug.Log("dog col");
            playerAnim.SetBool("Sit_b", true);
        }
        else if (!(ifout() && !ifcoll()))
        {
            Debug.Log("walk");
            playerAnim.SetBool("Sit_b", false);
            playerAnim.SetBool("Bark_b", false);
            Vector3 direction = target.transform.position - transform.position;
            float degree = Vector3.Angle(direction, transform.forward);
            transform.Translate(Vector3.forward * Time.deltaTime * speed_walk);
            if (degree > 20)
                transform.Rotate(Vector3.up, -Time.deltaTime * 50);
            else if (degree < -20)
                transform.Rotate(Vector3.up, Time.deltaTime * 50);
        }

		else
        {
            Debug.Log("dog divert");
            playerAnim.SetBool("Sit_b", false);
            playerAnim.SetBool("Bark_b", false);
            divert();
        }


    }
    void StateControl()
    {
        state_walk = Random.Range(0, 10);
        speed_turn = Random.Range(0, 15);
        angle = Random.Range(-1, 1);
        state_target = Random.Range(-1, 1);        
    }
    void divert()
    {
        //Debug.Log("divert");
        if (treeflag==false && cowflag==0)
		{
			if (mark > 0 && mark < 90)
			{
				transform.Rotate(Vector3.up, Time.deltaTime * 50);
			}
			else if (mark > 90 && mark < 180)
			{
				transform.Rotate(Vector3.up, -Time.deltaTime * 50);
			}
			else if(mark > 180 && mark < 270)
			{
				transform.Rotate(Vector3.up, Time.deltaTime * 50);
			}
			else
			{
				transform.Rotate(Vector3.up, -Time.deltaTime * 50);
			}
		}
		else if(treeflag==true)
		{
			Vector3 direction = tree.transform.position - transform.position;
			float degree = Vector3.Angle (direction, transform.forward);
			if (degree > 0)
				transform.Rotate(Vector3.up, -Time.deltaTime * 50);
			else
				transform.Rotate(Vector3.up, Time.deltaTime * 50);
		}
		else
		{
			Vector3 direction = transform.forward;
			if (cowflag == 1 && target != cow1)
				direction = cow1.transform.position - transform.position;
			else if (cowflag == 2 && target != cow2)
				direction = cow2.transform.position - transform.position;
			float degree = Vector3.Angle (direction, transform.forward);
			if (degree > 10)
				transform.Rotate(Vector3.up, -Time.deltaTime * 50);
			else if (degree < -10)
				transform.Rotate(Vector3.up, Time.deltaTime * 50);
		}
    }
    void test()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed_walk);
    }
	bool ifcoll()
	{
		float distance1 = (transform.position - cow1.transform.position).magnitude;
		float distance2 = (transform.position - cow2.transform.position).magnitude;
		float distance3 = (transform.position - tree.transform.position).magnitude;
		cowflag = 0;
		if (distance3 < 4)
		{
			treeflag = true;
			return true;
		}
		else 
			treeflag = false;
		cowflag = 0;
		if (distance1 < 7)
		{
			cowflag = 1;
			return true;
		}
		else if (distance2 < 7)
		{
			cowflag = 2;
			return true;
		}
		else
			return false;
	}
	
	
    bool ifout()
    {
        if (transform.position.z < -15 || transform.position.z > 15 || transform.position.x > 2 || transform.position.x < -19)
        {
            if(flag == false)
            {
                mark = transform.localEulerAngles.y;
				flag = true;
				return true;
            }
			if (transform.position.z < -15 && (transform.localEulerAngles.y <30 || transform.localEulerAngles.y > 330))
				return false;
			else if (transform.position.z > 15 && (transform.localEulerAngles.y > 150 && transform.localEulerAngles.y < 210))
				return false;
			else if (transform.position.x > 2 && (transform.localEulerAngles.y > 240 && transform.localEulerAngles.y < 300))
				return false;
			else if (transform.position.x < -19 && (transform.localEulerAngles.y > 60 && transform.localEulerAngles.y < 120))
				return false;
			else
				return true;         
        }
        else
        {
            flag = false;
            return false;
        }
    }

}
