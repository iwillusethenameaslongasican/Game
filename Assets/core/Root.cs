using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Net;
using System;
using code.unity.TankGame.Assets.proto.player;

public class Root : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NetMgr.servConn.msgDist.AddListener("s2c_heartbeat_reply", OnRecvHeartbeat);
        PanelMgr.instance.OpenPanel<LoginPanel>("");
	}
	
	// Update is called once per frame
	void Update () {
        NetMgr.Update();
	}

    public void OnRecvHeartbeat(byte[] Byte){
		ByteBuffer buffer = new ByteBuffer(Byte);
		int size = Byte.Length;
		int typeId = buffer.ReadInt();
        s2c_heartbeat_reply reply = PackCodec.Deserialize<s2c_heartbeat_reply>(buffer.ReadBytes(size - sizeof(Int32)));
    }
}
