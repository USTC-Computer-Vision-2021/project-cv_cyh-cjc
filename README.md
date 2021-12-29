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
    我们选择的是实现AR。由于上课时老师提供的Aartoolkit年代久远，已经难以使用，
因此我们选择使用Vuforia作为AR的使用接口。AR的3D场景生成由Unity实现。
我们实现的场景是一个乡村牧场，牧场里有奶牛和狗随机游走，
奶牛随机选择行走或者停止吃草。动物的行动有一定限制，一旦走到达到场景边界或者即将与其他物体碰撞，
则转向或者停止运动。
AR的实现分为两个部分。第一部分是识别出预先设定好的图片并估计平面在现实的三维中的位姿，第二部分是在识别的图片（平面）上生成3维场景。
项目中第一部分的实现由Vuforia引擎提供的接口实现，第二部分通过在Unity中设置场景，编写控制代码实现。

## 代码实现
### 状态控制
奶牛有三种运动状态，分别是随机行走，吃草和避让。避让在奶牛即将出界或者撞到其他物体时触发类似于触发中断程序，随机行走和吃草由函数void StateControl()控制。这个函数在void Start()中被调用。
变量state_walk决定行走或者停下，speed_turn决定转弯速度，angle决定转弯时是顺时针还是逆时针旋转。

```C#
    void StateControl()
    {
        state_walk = Random.Range(0, 10);
        speed_turn = Random.Range(0, 15);
        angle = Random.Range(-1, 1);
    }
```
void Start()在动画开始时被调用，用于初始化一些参数，例如该物体的动画组件playerAnim，其他物体（树，另一只奶牛和一只狗）。InvokeRepeating("StateControl", startDelay, repeatRate);
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
