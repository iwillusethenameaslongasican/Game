using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using code.unity.TankGame.Assets.proto.protoId;
using code.unity.TankGame.Assets.proto.friend;
using Net;
using System;

//查找面板
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

    public void OnAddBack(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_add_friend_reply reply = PackCodec.Deserialize<s2c_add_friend_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        Int32 ret = reply.result;
		if (ret == 1)
		{
			PanelMgr.instance.OpenPanel<TipPanel>("", "添加成功！");
		}
		else
		{
			PanelMgr.instance.OpenPanel<TipPanel>("", "添加失败，用户名id不存在或好友已添加！");
		}
    }
}
