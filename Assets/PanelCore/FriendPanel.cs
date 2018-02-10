using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using code.unity.TankGame.Assets.proto.protoId;
using code.unity.TankGame.Assets.proto.friend;
using Net;
using System;

public class FriendPanel : PanelBase
{
	private Transform content;
	private GameObject friendPrefab;
    private Button addBtn;
    private Button closeBtn;
    private Button reflashBtn;

	#region 生命周期
	public override void Init(params object[] args)
	{
		base.Init(args);
		skinPath = "FriendPanel";
		layer = PanelLayer.Panel;
	}

	public override void OnShowing()
	{
		base.OnShowing();
		Transform skinTrans = skin.transform;
		Transform listTrans = skinTrans.Find("ListImage");

		// 获取列表栏部件
		Transform scroolRect = listTrans.Find("ScrollRect");
		content = scroolRect.Find("Content");
        friendPrefab = content.Find("FriendPrefab").gameObject;
        friendPrefab.SetActive(false);

		closeBtn = listTrans.Find("CloseBtn").GetComponent<Button>();
		reflashBtn = listTrans.Find("ReflashBtn").GetComponent<Button>();
        addBtn = listTrans.Find("AddBtn").GetComponent<Button>();
		
        //按钮事件
		reflashBtn.onClick.AddListener(OnReflashClick);
		closeBtn.onClick.AddListener(OnCloseClick);
        addBtn.onClick.AddListener(OnAddClick);

		c2s_friend_list_request request = new c2s_friend_list_request();
		byte[] date = NetMgr.servConn.CreateData((int)ProtoId.c2s_friend_list_request, request);
		NetMgr.servConn.SendMessage(date, "s2c_friend_list_reply", OnReflashBack);

	}

	public override void OnClosing()
	{
		base.OnClosing();
	}
	#endregion

    public void OnReflashClick(){
        c2s_friend_list_request request = new c2s_friend_list_request();
		byte[] date = NetMgr.servConn.CreateData((int)ProtoId.c2s_friend_list_request, request);
		NetMgr.servConn.SendMessage(date, "s2c_friend_list_reply", OnReflashBack);
    }

    public void OnReflashBack(byte[] Byte){
		ClearFriendUnit();
        ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_friend_list_reply reply = PackCodec.Deserialize<s2c_friend_list_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        UInt32 num = reply.num;
        List<friend_info> infos = reply.infos;
		for (int i = 0; i < num; i++)
		{
            UInt32 Id = infos[i].id;
            string username = infos[i].username;
            UInt32 image = infos[i].image;
            UInt32 status = infos[i].status;
            GenerateFriendUnit(i, Id, username, image, status);
		}
    }

	public void ClearFriendUnit()
	{
		for (int i = 0; i < content.childCount; i++)
		{
			if (content.GetChild(i).name.Contains("Clone"))
			{
				Destroy(content.GetChild(i).gameObject);
			}
		}
	}

    public void GenerateFriendUnit(int i, UInt32 friendId, string username, UInt32 image, UInt32 status)
	{
		// 添加房间
		content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (i + 1) * 110);
		GameObject o = Instantiate(friendPrefab);
		o.transform.SetParent(content);
		o.SetActive(true);

		//房间信息
		Transform trans = o.transform;
		Text nameText = trans.Find("nameText").GetComponent<Text>();
        Image headImg = trans.Find("HeadImg").GetComponent<Image>();
		Text statusText = trans.Find("StatusText").GetComponent<Text>();

        nameText.text = "ID: " + username;
        
		if (status == 1)
		{
            statusText.color = Color.red;
			statusText.text = "状态：离线";
		}
		else
		{
            statusText.color = Color.green;
			statusText.text = "状态：在线";
		}

		//按钮事件
		Button chatBtn = trans.Find("ChatBtn").GetComponent<Button>();
        Button infoBtn = trans.Find("InfoBtn").GetComponent<Button>();
        Button delBtn = trans.Find("DelBtn").GetComponent<Button>();
		// 改变按钮的名字，以便给OnJoinBtnClick传参
        chatBtn.name = friendId.ToString();
        infoBtn.name = friendId.ToString();
        delBtn.name = friendId.ToString();
		//chatBtn.onClick.AddListener(delegate ()
		//{
		//	OnChatBtnClick(btn.name);
		//});
  //      infoBtn.onClick.AddListener(delegate ()
		//{
  //          OnInfoBtnClick(btn.name);
		//});
		//delBtn.onClick.AddListener(delegate ()
		//{
		//	OnDelBtnClick(btn.name);
		//});
	}

    public void OnCloseClick(){
        Close();
    }

    public void OnAddClick(){
        PanelMgr.instance.OpenPanel<SearchPanel>("", "");
    }
}
