using UnityEngine;
using System;
using System.Net.Sockets;
using System.IO;
using Net;
using ProtoBuf;
using code.unity.TankGame.Assets.proto.protoId;
using code.unity.TankGame.Assets.proto.player;

// 网络连接
public class Connection
{
    // 常量 缓冲区大小
    const int BUFFER_SIZE = 1024;
    // Socket
    private Socket socket;
    // buff
    private byte[] readBuff = new byte[BUFFER_SIZE];
    // 当前读缓冲区的长度
    private int buffCount = 0;
    // 粘包分包
    private Int32 msgLength = 0;

    // 心跳时间
    public float lastTickTime = 0;
    public float heartBeatTime = 2;
    // 消息分发
    public MsgDistribution msgDist = new MsgDistribution();

    // 状态
    public enum Status
    {
        None,
        Connected, // 已连接
    };
    public Status status = Status.None;

    // update 在NetMgr中调用
    public void Update() {
        msgDist.Update();
        // 心跳
        if(status == Status.Connected) {
            if(Time.time - lastTickTime > heartBeatTime) {
                c2s_heartbeat_request request2 = new c2s_heartbeat_request();
				byte[] date = NetMgr.servConn.CreateData((int)ProtoId.c2s_heartbeat_request, request2);
				NetMgr.servConn.SendMessage(date);
                lastTickTime = Time.time;
            }
        }
    }

    // 连接服务端
    public bool Connect(string host, int port) {
        try {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // connect
            socket.Connect(host, port);
            // BeginReceive
            socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount,
                                SocketFlags.None, ReceiveCb, readBuff);
            Debug.Log("连接成功");
            // 状态
            status = Status.Connected;
            return true;
        }
        catch(Exception e) {
            Debug.Log("连接失败：" + e.Message);
            return false;
        }
    }

    // 关闭连接
    public bool Close() {
        try {
            socket.Close();
            return true;
        }
        catch(Exception e) {
            Debug.Log("关闭失败：" + e.Message);
            return false;
        }
    }

    // 接受回调
    private void ReceiveCb(IAsyncResult ar) {
        try {
            // 处理缓冲区
            int count = socket.EndReceive(ar);
            //Debug.Log("count：" + count);
            buffCount += count;
            // 处理数据
            ProcessData();
            // 再次调用BeginReceive接收消息
            socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount,
                                SocketFlags.None, ReceiveCb, readBuff);
        }
        catch(Exception e) {
            Debug.Log("ReceiveCb 失败：" + e.Message);
            status = Status.None;
        }
    }

    // 消息处理
    private void ProcessData() {
  //      // 粘包分包
        if (buffCount < sizeof(ushort)) {
            return;
        }

		ByteBuffer buff = new ByteBuffer(readBuff); 
		int datalength = buff.ReadShort();
		if (buffCount < sizeof(ushort) + datalength)
		{
			return;
		}
		byte[] pbdata = new byte[datalength];
        Array.Copy(readBuff, sizeof(ushort), pbdata, 0, datalength);

        lock(msgDist.msgList) {
            msgDist.msgList.Add(pbdata);
        }
        // 清除已处理的消息
        int count = buffCount - sizeof(ushort) - datalength;
        Array.Copy(readBuff, sizeof(ushort) + datalength, readBuff, 0, count);
        buffCount = count;
        if (buffCount > 0) {
            ProcessData();
        }
    }



	/// <summary>
	/// 发送数据给服务器
	/// </summary>
	public void SendMessage(string data)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteString(data);
		byte[] msg = WriteMessage(buffer.ToBytes());
		SendMessage(msg);
	}

	/// <summary>
	/// 发送数据
	/// </summary>
	/// <param name="data"></param>
	public void SendMessage(byte[] data)
	{
		if (status == Status.None)
			return;
		try
		{
			socket.Send(data);
		}
		catch
		{
			status = Status.None;
			socket.Shutdown(SocketShutdown.Both);
			socket.Close();
		}
	}

	/// <summary>
	/// 发送数据给服务器
	/// </summary>
	public void SendMessage(string data, string cbName, MsgDistribution.Delegate cb)
	{
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteString(data);
            byte[] msg = WriteMessage(buffer.ToBytes());
            SendMessage(msg, cbName, cb);
	}

	/// <summary>
	/// 发送数据
	/// </summary>
	/// <param name="data"></param>
	public void SendMessage(byte[] data, string cbName, MsgDistribution.Delegate cb)
	{
		if (status == Status.None)
			return;
		try
		{
			msgDist.AddOnceListener(cbName, cb);
			socket.Send(data);
		}
		catch
		{
            msgDist.DelOnceListener(cbName, cb);
            status = Status.None;
			socket.Shutdown(SocketShutdown.Both);
			socket.Close();
		}
	}

	public byte[] CreateData(int typeId, IExtensible pbuf)
	{
		byte[] pbdata = PackCodec.Serialize(pbuf);
		ByteBuffer buff = new ByteBuffer();
		buff.WriteInt(typeId);
		buff.WriteBytes(pbdata);
		return WriteMessage(buff.ToBytes());
	}

   
	/// <summary>
	/// 数据转换，网络发送需要两部分数据，一是数据长度，二是主体数据
	/// </summary>
	/// <param name="message"></param>
	/// <returns></returns>
	private static byte[] WriteMessage(byte[] message)
	{
		MemoryStream ms = null;
		using (ms = new MemoryStream())
		{
			ms.Position = 0;
			BinaryWriter writer = new BinaryWriter(ms);
			ushort msglen = (ushort)message.Length;
			writer.Write(msglen);
			writer.Write(message);
			writer.Flush();
			return ms.ToArray();
		}
	}

    public string GetDesc(byte[] Byte)
    {
        string str = "";
        if (Byte == null) return "";
        for (int i = 0; i < Byte.Length; i ++) {
            int b = (int)Byte[i];
            str += b.ToString() + " ";
        }
        return str;
    }

}
