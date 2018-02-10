using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour {

    // 单例
    public static Battle instance;
    // 战场中所有坦克
    public BattleTank[] battleTanks;
    // 坦克预设
    public GameObject[] tankPrefabs;

	// Use this for initialization
	void Start () {
        // 单例
        instance = this;
        // 开始战斗
        //StartTwoCampBattle(1, 1);ßßß
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // 开始战斗
    public void StartTwoCampBattle(int n1, int n2) // #1第一阵营坦克数量 #2第二阵营坦克数量
    {
        // 获取出生点容器
        Transform sp = GameObject.Find("SwopPoints").transform;
        Transform spCamp1 = sp.GetChild(0).transform;
        Transform spCamp2 = sp.GetChild(1).transform;
        // 判断
        if(spCamp1.childCount < n1 || spCamp2.childCount < n2) {
            Debug.LogError("出生点数量不够");
            return;
        }
        if(tankPrefabs.Length < 2) {
            Debug.LogError("坦克预设数量不够");
            return;
        }
        // 清理战场
        ClearBattle();
        // 产生坦克
        battleTanks = new BattleTank[n1 + n2];
        for (int i = 0; i < n1; i ++) { // 产生第一阵营坦克
            GenerateTank(1, i, spCamp1, i); 
        }
        for (int i = 0; i < n2; i ++) { // 产生第二阵营坦克
            GenerateTank(2, i, spCamp2, n1 + i);
        }
        // 把第一辆坦克设为玩家操控
        Tank tankCmp = battleTanks[0].tank;
        tankCmp.ctrlType = Tank.CtrlType.player;
        // 设置相机
        CameraFollow cf = Camera.main.GetComponent<CameraFollow>();
        GameObject target = tankCmp.gameObject;
        cf.SetTarget(target);
    }

    // 生成一辆坦克(#1阵营 #2在阵营中的编号 #3出生点容器 #4在战场中的编号，对应battleTanks的下标
    public void GenerateTank(int camp, int num, Transform spCamp, int index) {
        Transform trans = spCamp.GetChild(num);
        Vector3 pos = trans.position;
        Quaternion rot = trans.rotation;
        GameObject prefab = tankPrefabs[camp - 1];
        // 产生坦克
        GameObject tankObj = (GameObject)Instantiate(prefab, pos, rot);
        // 设置属性
        Tank tankCmp = tankObj.GetComponent<Tank>();
        tankCmp.ctrlType = Tank.CtrlType.computer;
        battleTanks[index] = new BattleTank();
        battleTanks[index].tank = tankCmp;
        battleTanks[index].camp = camp;
    }

    // 获取阵营， 0表示错误
    public int GetCamp(GameObject tankObj) {
        for (int i = 0; i < battleTanks.Length; i ++) {
            BattleTank battleTank = battleTanks[i];
            if(battleTank == null) {
                return 0;
            }
            if(battleTank.tank.gameObject == tankObj) {
                return battleTank.camp;
            }
        }
        return 0;
    }

    // 是否同一阵营
    public bool IsSameCamp(GameObject tank1, GameObject tank2) {
        return GetCamp(tank1) == GetCamp(tank2);
    }

    // 胜负判断
    public bool IsWin(int camp) {
        Debug.Log("length: " + battleTanks.Length);
        for (int i = 0; i < battleTanks.Length; i ++) {
            Tank tank = battleTanks[i].tank;
            if(battleTanks[i].camp != camp) {
                if(tank.hp > 0) {
                    return false;
                }
            }
        }
        Debug.Log("阵营" + camp + "获胜");
		// 弹出结算界面
		PanelMgr.instance.OpenPanel<WinPanel>("", camp);
        return true;
    }

    // 胜负判断
    public bool IsWin(GameObject tankObj) {
        int camp = GetCamp(tankObj);
        Debug.Log("camp " + camp);
        return IsWin(camp);
    }

    // 清理场景
    public void ClearBattle() {
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Tank");
        for (int i = 0; i < tanks.Length; i ++) {
            Destroy(tanks[i]);
        }
    }
}
