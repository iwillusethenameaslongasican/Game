using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {
    // 所控制的坦克
    public Tank tank;

    // 锁定的坦克
    private GameObject target;
    // 视野范围
    public float sightDistance = 50;
    // 上一次搜寻时间
    private float lastSearchTargetTime = 0;
    // 搜寻间隔
    private float searchTargetInterval = 3;

    // 路径
    private Path path = new Path();


    // 上次更新路径时间
    public float lastUpdateWaypointTime = 0;
    //  更新路径间隔
    public float updateWaypointInterval = 10;

    // 初始化路点
    void InitWayPoint() {
        GameObject obj = GameObject.Find("WaypointContainer");
        if(obj != null && obj.transform.GetChild(0) != null) {
            Vector3 targetPos = obj.transform.GetChild(0).position;
            path.InitByNavMeshPath(transform.position, targetPos);
        }
    }
    // 状态枚举
    public enum State {
        Patrol, // 巡逻
        Attack, // 攻击
    }
    // 默认状态为巡逻
    private State state = State.Patrol;
	// Use this for initialization
	void Start () {
        InitWayPoint();
	}
	
	// Update is called once per frame
	void Update () {
        if(tank.ctrlType != Tank.CtrlType.computer) {
            return;
        }
        // 寻路测试
        if(path.IsReach(transform)) {
            path.NextWayPoint();
        }

        // 搜寻目标
        TargetUpdate();

        // 状态处理
        if(state == State.Patrol) {
            PatrolUpdate();
        }
        else if(state == State.Attack) {
            AttackUpdate();
        }
	}

    //void OnDrawGizmos()
    //{
    //    Debug.Log("bbbbbbbbbbbbbbbbbbb");
    //    path.DrawWayPoints();    
    //}

    // 更改状态
    public void ChangeState(State state) {
        this.state = state;
        if(state == State.Patrol) {
            // 做一些初始化工作
            PatrolStart();
        }
        else if(state == State.Attack) {
            // 做一些初始化工作
            AttackStart();
        }
    }

    // 巡逻开始
    public void PatrolStart() {
        
    }

    // 攻击开始
    public void AttackStart() {
        Vector3 targetPos = target.transform.position;
        path.InitByNavMeshPath(transform.position, targetPos);
    }

    // 巡逻中
    public void PatrolUpdate() {
        // 发现敌人
        if(target != null) {
            ChangeState(State.Attack);
        }
        // 时间间隔
        float interval = Time.time - lastUpdateWaypointTime;
        if(interval < updateWaypointInterval) {
            return;
        }
        lastUpdateWaypointTime = Time.time;
        // 处理巡逻点
        if (path.waypoints == null || path.isFinish) {
            GameObject obj = GameObject.Find("WaypointContainer");
            int count = obj.transform.childCount;
            if(count <= 0) {
                return;
            }
            int index = Random.Range(0, count);
            Vector3 targetPos = obj.transform.GetChild(index).position;
            path.InitByNavMeshPath(transform.position, targetPos);
        }
    }

    // 攻击中
    public void AttackUpdate() {
        // 目标丢失
        if(target == null) {
            ChangeState(State.Patrol);
            return;
        }
        // 时间间隔
        float interval = Time.time - lastUpdateWaypointTime;
        if(interval < updateWaypointInterval) {
            return;
        }
        lastUpdateWaypointTime = Time.time;
        // 更新路径
        Vector3 targetPos = target.transform.position;
        path.InitByNavMeshPath(transform.position, targetPos);
    }

    // 搜寻目标
    void TargetUpdate() {
        // cd时间
        float interval = Time.time - lastSearchTargetTime;
        if(interval < searchTargetInterval) {
            return;
        }
        lastSearchTargetTime = Time.time;
        // 没有目标的情况，寻找目标
        if(target == null) {
            NoTarget();
        }
        else // 已有目标的情况，判断是否丢失目标
        {
            HasTarget();
        }
    }

    // 已有目标的情况
    void HasTarget() {
        Tank targetTank = target.GetComponent<Tank>();
        Vector3 pos = transform.position;
        Vector3 targetPos = targetTank.transform.position;

        if(targetTank.ctrlType == Tank.CtrlType.none) {
            Debug.Log("目标死亡，丢失目标");
            target = null;
        }
        else if(Vector3.Distance(pos, targetPos) > sightDistance) {
            Debug.Log("距离过远，目标丢失");
            target = null;
        }
    }

    // 没有目标的话，搜寻视野中的目标
    void NoTarget() {
        float minHp = float.MaxValue;
        // 遍历所有坦克
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Tank");
        for (int i = 0; i < targets.Length; i++) {
            // Tank组建
            Tank tankLocal = targets[i].GetComponent<Tank>();
            if(tankLocal == null) {
                continue;
            }
            // 队友
            if(Battle.instance.IsSameCamp(gameObject, targets[i])) {
                continue;
            }
            // 自己
            if(targets[i] == gameObject) {
                continue;
            }
            // 死亡
            if(tankLocal.ctrlType == Tank.CtrlType.none) {
                continue;
            }
            // 判断距离
            Vector3 pos = transform.position;
            Vector3 targetPos = targets[i].transform.position;
            if(Vector3.Distance(pos, targetPos) > sightDistance) {
                continue;
            }
            // 判断生命值
            if(tankLocal.hp < minHp) {
                minHp = tankLocal.hp;
                target = tankLocal.gameObject;
            }
        }

        // 调试
        if(target != null) {
            Debug.Log("获取目标 " + target.name);
        }
    }

    // 被攻击
    public void OnAttacked(GameObject attackTank) {
        // 队友
        if(Battle.instance.IsSameCamp(gameObject, attackTank)) {
            return;
        }
        target = attackTank;
    }

    // 获取炮管和炮塔的目标角度
    public Vector3 GetTurretTarget() {
        // 没有目标，则炮塔朝着坦克前方
        if(target == null) {
            float y = transform.eulerAngles.y;
            Vector3 rot = new Vector3(0, y, 0);
            return rot;
        }
        // 有目标，则炮塔对准目标
        else {
            Vector3 pos = transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 dir = targetPos - pos;
            return Quaternion.LookRotation(dir).eulerAngles;
        }
    }

    // 是否发射炮弹
    public bool IsShoot() {
        if(target == null) {
            return false;
        }

        // 目标角度差
        float turretRoll = tank.turret.eulerAngles.y;
        float angle = turretRoll - GetTurretTarget().y;
        if(angle < 0) {
            angle += 360;
        }
        // 30度内发射炮弹
        if(angle < 30 || angle > 330) {
            return true;
        }
        else {
            return false;
        }
    }

    // 获取转向角
    public float GetSteering() {
        if(tank == null) {
            return 0;
        }

        Vector3 dir = transform.InverseTransformPoint(path.waypoint);
        // 向左转
        if(dir.x > path.deviation / 5) {
            return tank.maxSteeringAngle;
        }
        // 向右转
        else if(dir.x < -path.deviation / 5) {
            return -tank.maxSteeringAngle;
        }
        // 直走
        else {
            return 0;
        }
    }

    //  获取马力
    public float GetMotor() {
        if(tank == null) {
            return 0;
        }
        Vector3 dir = transform.InverseTransformPoint(path.waypoint);
        float x = dir.x;
        float z = dir.z;
        float r = 6;
        // 后退区域
        if(z < 0 && Mathf.Abs(x) < -z && Mathf.Abs(x) < r) {
            return -tank.maxMotorTorque;
        }
        else {
            return tank.maxMotorTorque;
        }
    }

    // 获取刹车
    public float GetBrakeTorque() {
        if(tank == null) {
            return 0;
        }
        // 走完路径，刹车
        if (path.isFinish) {
            return tank.maxBrakeTorque;
        }
        else {
            return 0;
        }
    }
}
