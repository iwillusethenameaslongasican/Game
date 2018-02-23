using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using code.unity.TankGame.Assets.proto.protoId;
using code.unity.TankGame.Assets.proto.chat;
using Net;

//聊天面板
public class ChatPanel : PanelBase
{
	private Button sendBtn;
	private Text content;
    private static InputField inputField;
    private static Dropdown dropDown;
    private static UInt32 friendId;
    private static GameObject IdPanel;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "ChatPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;

        sendBtn = skinTrans.Find("SendBtn").GetComponent<Button>();
        sendBtn.onClick.AddListener(OnSendClick);
		inputField = skinTrans.Find("InputField").GetComponent<InputField>();
		ScrollRect scrollRect = skinTrans.Find("Scroll View").GetComponent<ScrollRect>();
		content = scrollRect.content.Find("Text").GetComponent<Text>();

        IdPanel = skinTrans.Find("IdPanel").gameObject;
		IdPanel.SetActive(false);

        dropDown = skinTrans.Find("Dropdown").GetComponent<Dropdown>();
		dropDown.onValueChanged.AddListener(delegate
		{
			DropdownValueChanged(dropDown);
		});

		//监听
		NetMgr.servConn.msgDist.AddListener("s2c_chat_reply", OnRecvChat);
		NetMgr.servConn.msgDist.AddListener("s2c_group_chat_reply", OnRecvGroupChat);
        NetMgr.servConn.msgDist.AddListener("s2c_world_chat_reply", OnRecvWorldChat);
    }

    public override void OnClosing()
    {
		//监听
        NetMgr.servConn.msgDist.DelListener("s2c_chat_reply", OnRecvChat);
		NetMgr.servConn.msgDist.DelListener("s2c_group_chat_reply", OnRecvGroupChat);
        NetMgr.servConn.msgDist.DelListener("s2c_world_chat_reply", OnRecvWorldChat);
    }
	#endregion

    //点击发送按钮
	public void OnSendClick()
	{
        //Debug.Log("dropDown.value: " + dropDown.value);
        //私聊
        if (dropDown.value == 0){
                if (friendId == 0)
                {
                    PanelMgr.instance.OpenPanel<TipPanel>("", "未输入对方id！无法发送！");
                    return;
                }
                else
                {
                    string text = inputField.text;
                    c2s_chat_request request = new c2s_chat_request();
                    request.id = friendId;
                    request.content = text;
                    byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_chat_request, request);
                    NetMgr.servConn.SendMessage(data);
                }
            }//队伍内聊天
        else if (dropDown.value == 1){
            {
                string text = inputField.text;
                c2s_group_chat_request request = new c2s_group_chat_request();
                request.content = text;
                byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_group_chat_request, request);
                NetMgr.servConn.SendMessage(data);
            }
        }//群聊
		else if (dropDown.value == 2)
		{
			{
				string text = inputField.text;
				c2s_world_chat_request request = new c2s_world_chat_request();
				request.content = text;
				byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_world_chat_request, request);
				NetMgr.servConn.SendMessage(data);
			}
		}
	}

    //接收到组聊回包数据
	public void OnRecvGroupChat(byte[] Byte)
	{
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
		s2c_group_chat_reply reply = PackCodec.Deserialize<s2c_group_chat_reply>(buffer.ReadBytes(size - sizeof(Int32)));
		UInt32 id = reply.id;
        if (id == 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "玩家当前不在队伍中，发送失败！");
        }
        else
        {
            string text = reply.content;
            time_section timeSection = reply.time;
            UInt32 year = timeSection.year;
            UInt32 month = timeSection.month;
            UInt32 day = timeSection.day;
            UInt32 hour = timeSection.hour;
            UInt32 minu = timeSection.minu;
            UInt32 sec = timeSection.sec;
            string time = year + "/" + month + "/" + day + " " + hour + ":" + minu + ":" + sec;
            content.text += "<color=#008000ff>队伍  </color>";
            content.text += "<color=#000000ff>用户: </color>";
            Debug.Log("content: " + content.text);
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
   
    //接收群聊回包数据
	public void OnRecvWorldChat(byte[] Byte)
	{
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_world_chat_reply reply = PackCodec.Deserialize<s2c_world_chat_reply>(buffer.ReadBytes(size - sizeof(Int32)));
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
		content.text += "<color=#ff0000ff>世界  </color>";
		content.text += "<color=#000000ff>用户: </color>";
		Debug.Log("content: " + content.text);
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

    //接收私聊回包数据
	public void OnRecvChat(byte[] Byte)
	{
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
		s2c_chat_reply reply = PackCodec.Deserialize<s2c_chat_reply>(buffer.ReadBytes(size - sizeof(Int32)));
		UInt32 id = reply.id;
        if (id == 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "对方当前不在线，发送失败！");
        }
        else
        {
            string text = reply.content;
            time_section timeSection = reply.time;
            UInt32 year = timeSection.year;
            UInt32 month = timeSection.month;
            UInt32 day = timeSection.day;
            UInt32 hour = timeSection.hour;
            UInt32 minu = timeSection.minu;
            UInt32 sec = timeSection.sec;
            string time = year + "/" + month + "/" + day + " " + hour + ":" + minu + ":" + sec;
            content.text += "<color=#00ff00ff>私聊  </color>";
            content.text += "<color=#000000ff>用户: </color>";
            Debug.Log("content: " + content.text);
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

    //当下拉框值改变时调用
	void DropdownValueChanged(Dropdown change)
	{
        //如果当前为私聊
        if(change.value == 0){
            IdPanel.SetActive(true);
            Transform trans = IdPanel.transform;
			//按钮事件
			Button Btn = trans.Find("Button").GetComponent<Button>();
            InputField input = trans.Find("InputField").GetComponent<InputField>();
			Button closeBtn = trans.Find("CloseBtn").GetComponent<Button>();
			closeBtn.onClick.AddListener(OnCloseClick);
			
			Btn.onClick.AddListener(delegate ()
			{
                OnBtnClick(input.text);
			});
        }
	}

    //取得对方id
    public void OnBtnClick(string text){
        if(text != ""){
            friendId = UInt32.Parse(text);
			IdPanel.SetActive(false);
            inputField.ActivateInputField();
        }
        else{
            PanelMgr.instance.OpenPanel<TipPanel>("", "id不能为空");
        }
    }

    public void OnCloseClick(){
        IdPanel.SetActive(false); 
    }

    public static void SetFriendId(UInt32 id){
        friendId = id;
        dropDown.value = 0;
        IdPanel.SetActive(false);
        inputField.ActivateInputField();
    }
}