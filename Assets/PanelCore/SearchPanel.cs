using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using code.unity.TankGame.Assets.proto.protoId;
using code.unity.TankGame.Assets.proto.friend;
using Net;
using System;

public class SearchPanel : PanelBase
{
	private InputField idInput;
    private Button addBtn;
    private Button closeBtn;

	#region 生命周期
	public override void Init(params object[] args)
	{
		base.Init(args);
		skinPath = "SearchPanel";
		layer = PanelLayer.Panel;
	}

	public override void OnShowing()
	{
		base.OnShowing();
		Transform skinTrans = skin.transform;
		idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        addBtn = skinTrans.Find("AddBtn").GetComponent<Button>();
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();

        addBtn.onClick.AddListener(OnAddClick);
		closeBtn.onClick.AddListener(OnCloseClick);
	}

	public override void OnClosing()
	{
		base.OnClosing();
	}
	#endregion

	public void OnCloseClick()
	{
		Close();
	}

	public void OnAddClick()
	{
        c2s_add_friend_request request = new c2s_add_friend_request();
        request.id = UInt32.Parse(idInput.text);
        byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_add_friend_request, request);
		NetMgr.servConn.SendMessage(data, "s2c_add_friend_reply", OnAddBack);
	}
}
