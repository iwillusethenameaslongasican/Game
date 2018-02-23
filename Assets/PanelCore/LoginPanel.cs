using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using code.unity.TankGame.Assets.proto.player;
using code.unity.TankGame.Assets.proto.protoId;
using Net;

//登陆面板
public class LoginPanel : PanelBase
{
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button regBtn;

#region 生命周期
    // 初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "LoginPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        loginBtn = skinTrans.Find("LoginBtn").GetComponent<Button>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();

        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegClick);
    }
#endregion

    //申请按钮
    public void OnRegClick() {
        PanelMgr.instance.OpenPanel<RegPanel>("");
        Close();
    }

    //登陆按钮
    public void OnLoginClick() {
        // 用户名和密码不为空
        if(idInput.text == null || pwInput.text == null) {
            PanelMgr.instance.OpenPanel<TipPanel>("", "用户名和密码不能为空!");
            return;
        }
        // 如果尚未连接，则发起连接
        if(NetMgr.servConn.status != Connection.Status.Connected) {
            string host = "127.0.0.1";
            int port = 6666;
            if(!NetMgr.servConn.Connect(host, port)){
                PanelMgr.instance.OpenPanel<TipPanel>("", "连接服务器失败！");
            }
        }
        c2s_login_request request = new c2s_login_request();
		request.username = idInput.text;
		request.password = pwInput.text;
        byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_login_request, request);
        NetMgr.servConn.SendMessage(data, "s2c_login_reply", OnLoginBack);
    }

    //接收登陆回包数据
    public void OnLoginBack(byte[] Byte) {
        ByteBuffer buffer = new ByteBuffer(Byte);
        int size = Byte.Length;
        int typeId = buffer.ReadInt();
        s2c_login_reply reply = PackCodec.Deserialize<s2c_login_reply>(buffer.ReadBytes(size - sizeof(Int32)));
        UInt32 ret = reply.result_code;
        if(ret == 1) {
            PanelMgr.instance.OpenPanel<TipPanel>("", "登陆成功！");
            GameMgr.instance.id = reply.id;
            // 进入标题界面
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            PanelMgr.instance.OpenPanel<ChatPanel>("");
            Close();
        }
        else {
            PanelMgr.instance.OpenPanel<TipPanel>("", "登陆失败，请检查用户名和密码！");
        }
    }
}
