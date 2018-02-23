using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using code.unity.TankGame.Assets.proto.protoId;
using code.unity.TankGame.Assets.proto.room;
using code.unity.TankGame.Assets.proto.fight;
using Net;

//房间面板
public class RoomPanel : PanelBase
{
    private List<Transform> prefabs = new List<Transform>();
    private Button closeBtn;
    private Button startBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RoomPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        //组件
        for (int i = 0; i < 6; i++)
        {
            string preName = "PlayerPrefab" + i.ToString();
            Transform prefab = skinTrans.Find(preName);
            prefabs.Add(prefab);
        }
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();

        //按钮事件
        closeBtn.onClick.AddListener(OnCloseClick);
        startBtn.onClick.AddListener(OnStartClick);

        //监听
        NetMgr.servConn.msgDist.AddListener("s2c_get_room_info_reply", OnRecvGetRoomInfo);
        NetMgr.servConn.msgDist.AddListener("s2c_fight_reply", OnRecvFight);
		//发送查询
        c2s_get_room_info_request request = new c2s_get_room_info_request();
        request.room_id = GameMgr.instance.roomId;
        byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_get_room_info_request, request);
		NetMgr.servConn.SendMessage(data);

	}

    public override void OnClosing()
    {
        NetMgr.servConn.msgDist.DelListener("s2c_get_room_info_reply", OnRecvGetRoomInfo);
        NetMgr.servConn.msgDist.DelListener("s2c_fight_reply", OnRecvFight);
	}
    #endregion

    //接收房间信息回包数据
    public void OnRecvGetRoomInfo(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_get_room_info_reply reply = PackCodec.Deserialize<s2c_get_room_info_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        UInt32 num = reply.num;
        List<role_info> roles = reply.roles;
        int i = 0;
        for (; i < num; i ++){
            UInt32 id = roles[i].id;
            Int32 team = roles[i].team;
            UInt32 win = roles[i].win;
            UInt32 fail = roles[i].fail;
            Int32 isOwner = roles[i].is_owner;

            //信息处理
            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            string str = "名字：" + id.ToString() + "\r\n";
            str += "阵营：" + (team == 1 ? "红" : "蓝") + "\r\n";
            str += "胜利：" + win.ToString() + " ";
            str += "失败：" + fail.ToString() + "\r\n";
            //判断是不是自己
            if(id == GameMgr.instance.id){
                str += "【我自己】\r\n";
            }
            if(isOwner == 1){
                str += "【房主】";
            }
            text.text = str;

            if(team == 1){
                trans.GetComponent<Image>().color = Color.red;
            }
            else {
                trans.GetComponent<Image>().color = Color.blue;
            }
        }
        //设置其他栏位为 等待玩家
        for (; i < 6; i ++){
            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            text.text = "【等待玩家】";
            trans.GetComponent<Image>().color = Color.gray;
        }
    }

    //点击退出房间按钮
    public void OnCloseClick(){
        c2s_leave_room_request request = new c2s_leave_room_request();
        request.room_id = GameMgr.instance.roomId;
        byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_leave_room_request, request);
		NetMgr.servConn.SendMessage(data, "s2c_leave_room_reply", OnCloseBack);
    }

    //接收退出房间回包数据
    public void OnCloseBack(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_leave_room_reply reply = PackCodec.Deserialize<s2c_leave_room_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        Int32 ret = reply.ret;
        Debug.Log("ret: " + ret);
        if(ret == 1){
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出成功！");
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            Close();
        }
        else{
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出失败！");
        }
    }

    //开始战斗按钮
    public void OnStartClick(){
        c2s_start_fight_request request = new c2s_start_fight_request();
		byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_start_fight_request, request);
        NetMgr.servConn.SendMessage(data, "s2c_start_fight_reply", OnStartBack);
    }

    //接收开始战斗回包数据
    public void OnStartBack(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_start_fight_reply reply = PackCodec.Deserialize<s2c_start_fight_reply>(buffer.ReadBytes(size - sizeof(Int32)));
		Int32 ret = reply.ret;
        if(ret == 0) {
            PanelMgr.instance.OpenPanel<TipPanel>("", "开始任务失败！两队至少都需要" +
                                                  "一名玩家，只有队长可以开始比赛！");
        }
    }

    //接收战斗回包数据
    public void OnRecvFight(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_fight_reply reply = PackCodec.Deserialize<s2c_fight_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        UInt32 count = reply.count;
        List<object_info> infos = reply.objects;
        //开启战场
        MultiBattle.instance.StartBattle(count, infos);
        PanelMgr.instance.ClosePanel("RoomListPanel");
        Close();
    }
}
