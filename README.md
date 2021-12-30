# 基于Vuforia和Unity的AR实现
成员及分工
* 陈扬弘 PB18061377 
  * 调研资料
  * 场景设计
  * 代码实现
* 陈嘉铖 PB18061376
  * 调研资料
  * 动作设计
  * 代码实现
## 问题描述
我们选择的是实现AR。由于上课时老师提供的Artoolkit年代久远，已经难以使用，因此我们选择使用Vuforia作为AR的使用接口。AR的3D场景生成由Unity实现。我们实现的场景是一个乡村牧场，牧场里有奶牛、狗和家鸡。家鸡被圈养着，时不时地吃地上的虫；奶牛随机选择行走或者停止吃草；而狗也是随机选择步行或吠，在靠近奶牛时则会坐下歇息。动物的行动有一定限制，一旦将要达到场景边界或者即将与其他物体碰撞，则转向或者停止运动。AR的实现分为两个部分。第一部分是识别出预先设定好的图片并估计平面在现实的三维中的位姿，第二部分是在识别的图片（平面）上生成3维场景。项目中第一部分的实现由Vuforia引擎提供的接口实现，第二部分通过在Unity中设置场景，编写控制代码实现。

## 原理分析
增强现实，即AR，是通过电脑技术，将虚拟的信息应用到真实的世界，真实的环境和虚拟的物体实时地叠加到同一个画面或空间同时存在的一项技术。
其实现分为两类，一类是基于Marker（指定目标图片）的技术；一类是基于GPS地理信息的技术。在该项目中，我们使用的是前者。

### 1.基于Marker的技术
1）先设置识别标识marker

2）摄像头对现实场景进行拍摄，对marker进行识别和姿态评估，并确定其位置

![](https://github.com/USTC-Computer-Vision-2021/project-cv_cyh-cjc/blob/main/.github/AR_.jpg)

3）利用3D投影几何的知识，从模板坐标系变换到真实的屏幕坐标系并展示出来；这需要模板坐标系先旋转平移到摄像机坐标系（利用摄像机外参矩阵变换），再从摄像机坐标系映射到屏幕坐标系（利用摄像机内参矩阵变换）

![](https://github.com/USTC-Computer-Vision-2021/project-cv_cyh-cjc/blob/main/.github/AR_1.jpg)

### 2.基于GPS地理信息的技术
1）通过GPS获取用户的地理位置

2）然后从某些数据源（比如wiki，google）等处获取该位置附近物体（如周围的餐馆，银行，学校等）的POI信息

3）再通过移动设备的电子指南针和加速度传感器获取用户手持设备的方向和倾斜角度，通过这些信息建立目标物体在现实场景中的平面基准

4）最后再进行坐标变换显示，原理与基于Marker的类似



## 代码实现
项目中的3D模型、场景和动画在Unity的asset store（https://assetstore.unity.com/
）
和Unity的官方教程（https://learn.unity.com/tutorial
）
中提供。代码部分由参考了教程中的例子后自己实现。  

奶牛的运动控制由RandomRoam.cs实现。代码中的类RandomRoam继承了Unity中的预定义的类。void Start()在动画开始时被调用，用于初始化一些参数，例如该物体的动画组件playerAnim，其他物体（树，另一只奶牛和一只狗）。InvokeRepeating("StateControl", startDelay, repeatRate);
用于以给定时间间隔调用StateControl()函数决定物体的运动状态。
```
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
    
```
奶牛有三种运动状态，分别是随机行走，吃草和避让。避让在奶牛即将出界或者撞到其他物体时触发类似于触发中断程序，随机行走和吃草由函数void StateControl()控制。这个函数在void Start()中被调用并为 void Update()中的变量赋值。
变量state_walk决定行走或者停下，speed_turn决定转弯速度，angle决定转弯时是顺时针还是逆时针旋转。

```C#
    void StateControl()
    {
        state_walk = Random.Range(0, 10);
        speed_turn = Random.Range(0, 15);
        angle = Random.Range(-1, 1);
    }
```

函数void Update在动画运行的每一帧被调用，这个函数直接控制奶牛的运动和动画。ifout()和ifcoll()用于判断奶牛是否出界或是否将发生碰撞。如果触发任意一个限制，则执行divert()转向避让，转向的逻辑在自定义函数void divert()中实现。如果这两个限制都没有触发，则奶牛处于行走或者停下状态中。状态由随机变量state_walk决定。如果state_walk≥5则行走，同时关闭吃草的动画playerAnim.SetBool("Eat_b", false)。直线运动由Unity提供的transform.Translate()实现，转向由transform.Rotate()实现。行走的速度、方向都是在StateControl()赋值的随机变量。如果不行走，则停下并触发吃草的动画playerAnim.SetBool("Eat_b", true)。
```C#
    void Update()
    {
        x = transform.position.x;
        z = transform.position.z;
        treeflag = false;
        if (!ifout()&&!ifcoll())
        {
            if (state_walk >= 0)
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
            divert();
        }
    }
```  
void divert()分出界(treeflag==false && cowflag==0)、撞树(treeflag==true)、撞动物3种情况考虑。在转向时考虑两个物体之间的角度关系使转向的效果更自然。变量mark记录出界瞬间时的方向，在函数ifout()中更新。

```C#
    void divert()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed_walk);
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
	    Vector3 direction = tree.transform.position - transform.position;
	    float degree = Vector3.Angle (direction, transform.forward);
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
	    float degree = Vector3.Angle (direction, transform.forward);
	    if (degree > 0 && degree < 120)
		transform.Rotate(Vector3.up, -Time.deltaTime * 50);
	    else if (degree < 0 && degree > -120)
		transform.Rotate(Vector3.up, Time.deltaTime * 50);
	}
    }
```
函数bool ifout()根据物体的位置和方向判定动物是否出界。flag类似于锁存器，保证mark记录的是出界瞬间的方向。
```C#
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
            if (transform.position.z < -15 && (transform.localEulerAngles.y < 30 || transform.localEulerAngles.y > 330))
            {
                return false;
            }
            else if (transform.position.z > 15 && (transform.localEulerAngles.y > 150 && transform.localEulerAngles.y < 210))
            {
                return false;
            }
            else if (transform.position.x > 2 && (transform.localEulerAngles.y > 240 && transform.localEulerAngles.y < 300))
            {
                return false;
            }
            else if (transform.position.x < -19 && (transform.localEulerAngles.y > 60 && transform.localEulerAngles.y < 120))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            flag = false;
            return false;
        }
    }
```    
函数bool ifcoll()用于判断奶牛是否将要发生碰撞以及将和什么物体发生碰撞，判断的结果影响divert()中的转向的效果。
```C#
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
            cowflag = 1;
            return true;
        }
        else
        {
            cowflag = 2;
            return true;
        }
    }

```
鸡的控制由chicken.cs实现，里面只有Eat和站着不动两个状态，代码比较简单就不解释了。
狗的控制由DogRoam.cs实现，狗将随机选择其中一头牛作为运动的目标向其靠近，当达到一定距离内之后坐下。其中变量state_target在void StateControl()中被赋值，StateControl()函数与控制奶牛的脚本中的StateControl()类似，只是多了对state_target赋随机值。
```C#
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
```

## 运行效果
Unity中设置的场景动画

![](https://github.com/USTC-Computer-Vision-2021/project-cv_cyh-cjc/blob/main/.github/Unity_onPC.gif)

安装安卓应用并对着我们设置的marker实物或图片拍摄即可演示

![](https://github.com/USTC-Computer-Vision-2021/project-cv_cyh-cjc/blob/main/.github/our_marker.jpg)

![](https://github.com/USTC-Computer-Vision-2021/project-cv_cyh-cjc/blob/main/.github/effect.gif)

## 工程结构
    .
    ├── code(/Assets/Scipts)
    │   ├── chicken.cs
    │   └── DogRoam.cs
    │   └── RandomRoam.cs
    ├── Scenes
    │   ├── Main Camera
    │   └── AR Camera
    │   └── ImageTarget(Marker(our_marker.jpg))
    │       ├── farm
    └── output
        └── dazuoye.apk


## 运行说明
    Unity Hub==2.4.5
    Unity==2018.4.36f1(Android Build Support,Vuforia Augmented Reality)
