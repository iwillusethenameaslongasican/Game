using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using code.unity.TankGame.Assets.proto.protoId;
using code.unity.TankGame.Assets.proto.room;
using code.unity.TankGame.Assets.proto.player;
using Net;
using System;

public class RoomListPanel : PanelBase
{
    private Text idText;
    private Text winText;
    private Text lostText;
    private Transform content;
    private GameObject roomPrefab;
    private Button closeBtn;
    private Button newBtn;
    private Button reflashBtn;
    private Button friendBtn;
    private Button msgBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RoomListPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        Transform listTrans = skinTrans.Find("ListImage");
        Transform winTrans = skinTrans.Find("WinImage");

        // 获取成绩栏附件
        idText = winTrans.Find("IDText").GetComponent<Text>();
        winText = winTrans.Find("WinText").GetComponent<Text>();
        lostText = winTrans.Find("LostText").GetComponent<Text>();
        friendBtn = winTrans.Find("FriendBtn").GetComponent<Button>();
        msgBtn = winTrans.Find("WinBtn").GetComponent<Button>();

        friendBtn.onClick.AddListener(OnFriendClick);
        //msgBtn.onClick.AddListener(OnMsgClick);

        // 获取列表栏部件
        Transform scroolRect = listTrans.Find("ScrollRect");
        content = scroolRect.Find("Content");
        roomPrefab = content.Find("RoomPrefab").gameObject;
        roomPrefab.SetActive(false);

        closeBtn = listTrans.Find("CloseBtn").GetComponent<Button>();
        newBtn = listTrans.Find("NewBtn").GetComponent<Button>();
        reflashBtn = listTrans.Find("ReflashBtn").GetComponent<Button>();
        //按钮事件
        reflashBtn.onClick.AddListener(OnReflashClick);
        newBtn.onClick.AddListener(OnNewClick);
        closeBtn.onClick.AddListener(OnCloseClick);

        //监听
        NetMgr.servConn.msgDist.AddListener("s2c_get_achieve_reply", OnRecvGetAchieve);
        NetMgr.servConn.msgDist.AddListener("s2c_get_room_list_reply", OnRecvGetRoomList);

        //发送查询
        c2s_get_room_list_request request1 = new c2s_get_room_list_request();
        byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_get_room_list_request, request1);
		NetMgr.servConn.SendMessage(data);

        c2s_get_achieve_request request2 = new c2s_get_achieve_request();
        request2.id = GameMgr.instance.id;
        byte[] date = NetMgr.servConn.CreateData((int)ProtoId.c2s_get_achieve_request, request2);
        NetMgr.servConn.SendMessage(date);
    }

    public override void OnClosing()
    {
        base.OnClosing();
        NetMgr.servConn.msgDist.DelListener("s2c_get_achieve_reply", OnRecvGetAchieve);
        NetMgr.servConn.msgDist.DelListener("s2c_get_room_list_reply", OnRecvGetRoomList);
    }
    #endregion

    //收到GetAchieve协议
    public void OnRecvGetAchieve(byte[] Byte) {
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
		//Debug.Log("收到GetAchieve协议: " + NetMgr.servConn.GetDesc(buffer.ReadBytes(size - sizeof(Int32))));
        s2c_get_achieve_reply reply = PackCodec.Deserialize<s2c_get_achieve_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        UInt32 id = reply.id;
        UInt32 win = reply.win;
        UInt32 fail = reply.fail;
        // 处理
        idText.text = "指挥官：" + id.ToString();
        winText.text = win.ToString();
        lostText.text = fail.ToString();
    }

    //收到GetRoomList协议
    public void OnRecvGetRoomList(byte[] Byte){
        //清理
        ClearRoomUnit();

		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_get_room_list_reply reply = PackCodec.Deserialize<s2c_get_room_list_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        UInt32 num = reply.num;
        List<room_info> rooms = reply.rooms;
        for (int i = 0; i < num; i ++) {
            UInt32 Id = rooms[i].room_id;
            UInt32 count = rooms[i].num;
            Int32 status = rooms[i].status;
            GenerateRoomUnit(i, Id, count, status);
        }
    }

    public void ClearRoomUnit() {
        for (int i = 0; i < content.childCount; i ++) {
            if(content.GetChild(i).name.Contains("Clone")) {
                Destroy(content.GetChild(i).gameObject);
            }
        }
    }

    // 创建一个房间单元
    // 参数i，房间序号（从0开始）
    // 参数roomId, 房间唯一id
    // 参数num, 房间里的玩家数量
    // 参数status, 房间状态，1-准备着 2-战斗中
    public void GenerateRoomUnit(int i, UInt32 roomId, UInt32 num, Int32 status){
        // 添加房间
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (i + 1) * 110);
        GameObject o = Instantiate(roomPrefab);
        o.transform.SetParent(content);
        o.SetActive(true);

        //房间信息
        Transform trans = o.transform;
        Text nameText = trans.Find("nameText").GetComponent<Text>();
        Text countText = trans.Find("CountText").GetComponent<Text>();
        Text statusText = trans.Find("StatusText").GetComponent<Text>();

        nameText.text = "序号: " + (i + 1).ToString();
        countText.text = "人数：" + num.ToString();
        if(status == 0) {
            statusText.color = Color.black;
            statusText.text = "状态：准备中 ";
        }
        else {
            statusText.color = Color.red;
            statusText.text = "状态：开战中 ";
        }

        //按钮事件
        Button btn = trans.Find("JoinButton").GetComponent<Button>();
        // 改变按钮的名字，以便给OnJoinBtnClick传参
        btn.name = roomId.ToString();
        btn.onClick.AddListener(delegate()
        {
            OnJoinBtnClick(btn.name);
        });
    }

    // 刷新按钮
    public void OnReflashClick(){
        c2s_get_room_list_request request = new c2s_get_room_list_request();
        byte[] date = NetMgr.servConn.CreateData((int)ProtoId.c2s_get_room_list_request, request);
		NetMgr.servConn.SendMessage(date);
    }

    //加入按钮
    public void OnJoinBtnClick(string roomId){
        c2s_enter_room_request request = new c2s_enter_room_request();
        request.room_id = Int32.Parse(roomId);
        byte[] date = NetMgr.servConn.CreateData((int)ProtoId.c2s_enter_room_request, request);
		NetMgr.servConn.SendMessage(date, "s2c_enter_room_reply", OnJoinBtnBack);
        Debug.Log("请求进入房间：" + roomId);
    }

    //加入按钮返回
    public void OnJoinBtnBack(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_enter_room_reply reply = PackCodec.Deserialize<s2c_enter_room_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        Int32 ret = reply.ret;
        //处理
        if(ret == 1) {
            PanelMgr.instance.OpenPanel<TipPanel>("", "成功进入房间！");
            GameMgr.instance.roomId = reply.room_id;
            PanelMgr.instance.OpenPanel<RoomPanel>("");
        }
        else {
            PanelMgr.instance.OpenPanel<TipPanel>("", "进入房间失败！");
        }
    }

    //新建按钮
    public void OnNewClick(){
        c2s_create_room_request request = new c2s_create_room_request();
        byte[] date = NetMgr.servConn.CreateData((int)ProtoId.c2s_create_room_request, request);
		NetMgr.servConn.SendMessage(date, "s2c_create_room_reply", OnNewBack);
    }

    //新建按钮返回
    public void OnNewBack(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_create_room_reply reply = PackCodec.Deserialize<s2c_create_room_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        Int32 ret = reply.ret;
        if(ret == 1){
            PanelMgr.instance.OpenPanel<TipPanel>("", "创建成功！");
            GameMgr.instance.roomId = reply.room_id;
            PanelMgr.instance.OpenPanel<RoomPanel>("");
        }
        else{
            PanelMgr.instance.OpenPanel<TipPanel>("", "创建房间失败");
        }
    }

    //退出按钮
    public void OnCloseClick() {
        c2s_logout_request request = new c2s_logout_request();
        byte[] date = NetMgr.servConn.CreateData((int)ProtoId.c2s_logout_request, request);
		NetMgr.servConn.SendMessage(date, "s2c_logout_reply", OnCloseBack);
    }

    //退出返回
    public void OnCloseBack(byte[] Byte){
        PanelMgr.instance.OpenPanel<TipPanel>("", "登出成功！");
        PanelMgr.instance.ClosePanel("RoomPanel");
        PanelMgr.instance.OpenPanel<LoginPanel>("","");
        NetMgr.servConn.Close();
    }

    public void OnFriendClick(){
        PanelMgr.instance.OpenPanel<FriendPanel>("", "");
    }


}
