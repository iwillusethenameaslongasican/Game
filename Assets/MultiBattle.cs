using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using code.unity.TankGame.Assets.proto.fight;
using Net;

public class MultiBattle : MonoBehaviour
{
    //单例
    public static MultiBattle instance;
    //坦克预设
    public GameObject[] tankPrefabs;
    //战场中的所有坦克
    public Dictionary<UInt32, BattleTank> list = new Dictionary<uint, BattleTank>();

	// Use this for initialization
	void Start()
	{
        //单例模式
        instance = this;
	}

	// Update is called once per frame
	void Update()
	{
		
	}

    //获取阵营 0表示错误
    public int GetCamp(GameObject tankObj){
        foreach(BattleTank mt in list.Values){
            if(mt.tank.gameObject == tankObj){
                return mt.camp;
            }
        }
        return 0;
    }

    //是否同一阵营
    public bool IsSameCamp(GameObject tank1, GameObject tank2){
        return GetCamp(tank1) == GetCamp(tank2);
    }

    //清理场景
    public void ClearBattle(){
        list.Clear();
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Tank");
        for (int i = 0; i < tanks.Length; i ++){
            Destroy(tanks[i]);
        }
    }

    //开始战斗
    public void StartBattle(UInt32 count, List<object_info> infos){
        //清理场景
        ClearBattle();
        //每一辆坦克
        for (int i = 0; i < count; i ++){
            UInt32 id = infos[i].id;
            Int32 team = infos[i].team;
            Int32 swopID = infos[i].swop_id;
            GenerateTank(id, team, swopID);
        }
        NetMgr.servConn.msgDist.AddListener("s2c_update_unit_reply", OnRecvUpdateUnitInfo);
        NetMgr.servConn.msgDist.AddListener("s2c_shooting_reply", OnRecvShooting);
        NetMgr.servConn.msgDist.AddListener("s2c_hit_reply", OnRecvHit);
        NetMgr.servConn.msgDist.AddListener("s2c_result_reply", OnRecvResult);
    }

    //产生坦克
    public void GenerateTank(UInt32 id, Int32 team, Int32 swopID){
        //获取出生点
        Transform sp = GameObject.Find("SwopPoints").transform;
        Transform swopTrans;
        if(team == 1){
            Transform teamSwop = sp.GetChild(0);
            swopTrans = teamSwop.GetChild(swopID - 1);
        }
        else{
            Transform teamSwop = sp.GetChild(1);
            swopTrans = teamSwop.GetChild(swopID - 1);
        }
        if(swopTrans == null){
            Debug.LogError("GenerateTank 出生点错误！");
            return;
        }
        //预设
        if(tankPrefabs.Length < 2){
            Debug.LogError("坦克预设数量不够！");
            return;
        }
        //产生坦克
        GameObject tankObj = (GameObject)Instantiate(tankPrefabs[team - 1]);
        tankObj.name = Convert.ToString(id);
        tankObj.transform.position = swopTrans.position;
        tankObj.transform.rotation = swopTrans.rotation;
        //列表处理
        BattleTank bt = new BattleTank();
        bt.tank = tankObj.GetComponent<Tank>();
        bt.camp = team;
        list.Add(id, bt);
        //玩家处理
        if (id == GameMgr.instance.id) {
            bt.tank.ctrlType = Tank.CtrlType.player;
            CameraFollow cf = Camera.main.gameObject.GetComponent<CameraFollow>();
            GameObject target = bt.tank.gameObject;
            cf.SetTarget(target);
        }
        else{
            bt.tank.ctrlType = Tank.CtrlType.net;
            bt.tank.InitNetCtrl();  //初始化网络同步
        }
    }

    public void OnRecvUpdateUnitInfo(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_update_unit_reply reply = PackCodec.Deserialize<s2c_update_unit_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        UInt32 id = reply.id;
        Vector3 nPos;
        Vector3 nRot;
        nPos.x = (float)(reply.pos_x * 1.0 / 1000);
        nPos.y = (float)(reply.pos_y * 1.0 / 1000);
        nPos.z = (float)(reply.pos_z * 1.0 / 1000);
        nRot.x = (float)(reply.rot_x * 1.0 / 1000);
        nRot.y = (float)(reply.rot_y * 1.0 / 1000);
        nRot.z = (float)(reply.rot_z * 1.0 / 1000);
        float turretY = (float)(reply.gun_rot * 1.0 / 1000);
        float gunX = (float)(reply.gun_roll * 1.0 / 1000);
        //处理
        Debug.Log("OnRecvUpdateUnitInfo " + id);
        if(!list.ContainsKey(id)){
            Debug.Log("OnRecvUpdateUnitInfo bt == null");
            return;
        }
        BattleTank bt = list[id];
        //跳过自己的同步信息
        if(id == GameMgr.instance.id){
            return;
        }
        if(bt == null){
            Debug.Log("bt == null ");
            return;
        }
        //更新预测位置和旋转
        bt.tank.NetForecastInfo(nPos, nRot);
        //更新炮塔和炮管的旋转角度
        bt.tank.NetTurretTarget(turretY, gunX);
    }

    public void OnRecvShooting(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_shooting_reply reply = PackCodec.Deserialize<s2c_shooting_reply>(buffer.ReadBytes(size - sizeof(Int32)));
		UInt32 id = reply.id;
        Vector3 pos;
        Vector3 rot;
        pos.x = (float)(reply.pos_x * 1.0 / 1000);
        pos.y = (float)(reply.pos_y * 1.0 / 1000);
        pos.z = (float)(reply.pos_z * 1.0 / 1000);
        rot.x = (float)(reply.rot_x * 1.0 / 1000);
        rot.y = (float)(reply.rot_y * 1.0 / 1000);
        rot.z = (float)(reply.rot_z * 1.0 / 1000);
        //处理
        if(!list.ContainsKey(id)){
            Debug.Log("OnRecvShooting bt == null");
            return;
        }
        BattleTank bt = list[id];
        //跳过自己的同步信息
        if(id == GameMgr.instance.id){
            return;
        }
        bt.tank.NetShoot(pos, rot);
    }

    public void OnRecvHit(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_hit_reply reply = PackCodec.Deserialize<s2c_hit_reply>(buffer.ReadBytes(size - sizeof(Int32)));
		UInt32 attId = reply.id;
        UInt32 defId = reply.enemy_id;
        float hurt = (float)(reply.damage * 1.0 / 1000);
        //取得BattleTank
        if(!list.ContainsKey(attId)){
            Debug.Log("OnRecvHit attBt == null " + attId);
            return;
        }
        BattleTank attBt = list[attId];

        if(!list.ContainsKey(defId)){
            Debug.Log("OnRecvHit defBt == null " + defId);
            return;
        }

        BattleTank defBt = list[defId];
        //被击中的坦克
        defBt.tank.NetBeAttacked(hurt, attBt.tank.gameObject);
    }

    public void OnRecvResult(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_result_reply reply = PackCodec.Deserialize<s2c_result_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        Int32 team = reply.camp;
        //弹出胜负面板
        UInt32 id = GameMgr.instance.id;
        BattleTank bt = list[id];
        if(bt.camp == team){
            PanelMgr.instance.OpenPanel<WinPanel>("", 1);
        }
        else{
            PanelMgr.instance.OpenPanel<WinPanel>("", 0);
        }
		//取消监听
        NetMgr.servConn.msgDist.DelListener("s2c_update_unit_reply", OnRecvUpdateUnitInfo);
		NetMgr.servConn.msgDist.DelListener("s2c_shooting_reply", OnRecvShooting);
		NetMgr.servConn.msgDist.DelListener("s2c_hit_reply", OnRecvHit);
		NetMgr.servConn.msgDist.DelListener("s2c_result_reply", OnRecvResult);
    }
}







