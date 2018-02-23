using UnityEngine;
using System;
using UnityEngine.UI;
using code.unity.TankGame.Assets.proto.player;
using code.unity.TankGame.Assets.proto.protoId;
using Net;

//申请按钮
public class RegPanel : PanelBase
{
    private InputField idInput;
    private InputField pwInput;
    private InputField repInput;
    private Button regBtn;
    private Button closeBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RegPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        repInput = skinTrans.Find("RepInput").GetComponent<InputField>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();

        regBtn.onClick.AddListener(OnRegClick);
        closeBtn.onClick.AddListener(OnCloseClick);
    }
    #endregion

    public void OnCloseClick() {
        PanelMgr.instance.OpenPanel<LoginPanel>("");
        Close();
    }

    public void OnRegClick() {
        // 用户名，密码不为空
        if(idInput.text == null || pwInput.text == null) {
			PanelMgr.instance.OpenPanel<TipPanel>("", "用户名和密码不能为空!");
			return;
        }
        if (pwInput.text != repInput.text)
		{
			PanelMgr.instance.OpenPanel<TipPanel>("", "两次输入的密码不同!");
			return;
		}
        // 如果未连接，则先连接
        if(NetMgr.servConn.status != Connection.Status.Connected) {
            string host = "127.0.0.1";
            int port = 6666;
			if (!NetMgr.servConn.Connect(host, port))
			{
				PanelMgr.instance.OpenPanel<TipPanel>("", "连接服务器失败！");
			}
        }
		// 发送
		c2s_register_request request= new c2s_register_request();
        request.username = idInput.text;
		request.password = pwInput.text;
		byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_register_request, request);
		NetMgr.servConn.SendMessage(data, "s2c_register_reply", OnRegBack);

    }

    public void OnRegBack(byte[] Byte) {
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_register_reply reply = PackCodec.Deserialize<s2c_register_reply>(buffer.ReadBytes(size - sizeof(Int32)));
		UInt32 ret = reply.result_code;
        Debug.Log("ret: " + ret);
        if(ret == 1) {
            PanelMgr.instance.OpenPanel<TipPanel>("", "注册成功！");
            PanelMgr.instance.OpenPanel<LoginPanel>("");
            Close();
        }
        else {
            PanelMgr.instance.OpenPanel<TipPanel>("", "注册失败，请更换用户名！");
        }
    }
}
