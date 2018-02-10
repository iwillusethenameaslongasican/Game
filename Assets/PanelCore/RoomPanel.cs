using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using code.unity.TankGame.Assets.proto.protoId;
using code.unity.TankGame.Assets.proto.room;
using code.unity.TankGame.Assets.proto.fight;
using code.unity.TankGame.Assets.proto.chat;
using Net;

public class RoomPanel : PanelBase
{
    private List<Transform> prefabs = new List<Transform>();
    private Button closeBtn;
    private Button startBtn;
    private Button sendBtn;
    private Text content;
    private InputField inputField;

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
        sendBtn = skinTrans.Find("SendBtn").GetComponent<Button>();
        //按钮事件
        closeBtn.onClick.AddListener(OnCloseClick);
        startBtn.onClick.AddListener(OnStartClick);
        sendBtn.onClick.AddListener(OnSendClick);

        inputField = skinTrans.Find("InputField").GetComponent<InputField>();
        ScrollRect scrollRect = skinTrans.Find("Scroll View").GetComponent<ScrollRect>();
        content = scrollRect.content.Find("Text").GetComponent<Text>();
        //监听
        NetMgr.servConn.msgDist.AddListener("s2c_get_room_info_reply", OnRecvGetRoomInfo);
        NetMgr.servConn.msgDist.AddListener("s2c_fight_reply", OnRecvFight);
        NetMgr.servConn.msgDist.AddListener("s2c_group_chat_reply", OnRecvGroupChat);

		//发送查询
		//发送查询
        c2s_get_room_info_request request = new c2s_get_room_info_request();
        request.room_id = GameMgr.instance.roomId;
        byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_get_room_info_request, request);
		//Debug.Log("CreateData之后发送 " + NetMgr.servConn.GetDesc(data));
		NetMgr.servConn.SendMessage(data);

	}

    public override void OnClosing()
    {
        NetMgr.servConn.msgDist.DelListener("s2c_get_room_info_reply", OnRecvGetRoomInfo);
        NetMgr.servConn.msgDist.DelListener("s2c_fight_reply", OnRecvFight);
    }
    #endregion

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

            Debug.Log("win: " + win.ToString());
            Debug.Log("fail: " + fail.ToString());
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

    public void OnRecvFight(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_fight_reply reply = PackCodec.Deserialize<s2c_fight_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        UInt32 count = reply.count;
        List<object_info> infos = reply.objects;
        MultiBattle.instance.StartBattle(count, infos);
        PanelMgr.instance.ClosePanel("RoomListPanel");
        Close();
    }

    public void OnSendClick(){
        string text = inputField.text;
		c2s_group_chat_request request = new c2s_group_chat_request();
        request.content = text;
		byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_group_chat_request, request);
		NetMgr.servConn.SendMessage(data);
    }

    public void OnRecvGroupChat(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_group_chat_reply reply = PackCodec.Deserialize<s2c_group_chat_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        UInt32 id = reply.id;
        string text = reply.content;
        time_section timeSection = reply.time;
        UInt32 year = timeSection.year;
        UInt32 month = timeSection.month;
        UInt32 day = timeSection.day;
        UInt32 hour = timeSection.hour;
        UInt32 minu = timeSection.minu;
        UInt32 sec = timeSection.sec;
        string time = year + "/" + month + "/" + day + " " + hour + ":" + minu + ":" + sec;
        content.text += "<color=#000000ff>用户: </color>";
        if (id == GameMgr.instance.id)
        {
            content.text += "<color=#ff0000ff>" + id + "(自己)          " + "</color>";
        }
        else
        {
            content.text += "<color=#0000ff>" + id + "  " + "</color>";
        }
        content.text += "<color=#00ffffff>" + time + "\r\n" + "</color>";
        content.text += "<color=#000000ff>" + text + "\r\n" + "</color>";
    }
}
