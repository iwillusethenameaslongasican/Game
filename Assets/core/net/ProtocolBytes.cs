using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using Net;

public class ProtocolBytes : ProtocolBase
{
    // 传输的字节流
    public byte[] bytes;

    // 解码器
    public override ProtocolBase Decode(byte[] readbuff, int start, int length)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.bytes = new byte[length];
        Array.Copy(readbuff, start, protocol.bytes, 0, length);
        return protocol;
    }

    // 编码器
    public override byte[] Encode()
    {
        return bytes;
    }

    // 协议名称
    public override string GetName()
    {
        return GetString(0);
    }

    // 协议描述
    public override string GetDesc()
    {
        string str = "";
        if (bytes == null) return "";
        for (int i = 0; i < bytes.Length; i ++) {
            int b = (int)bytes[i];
            str += b.ToString() + " ";
        }
        return str;
    }

    ////---------- 字节流辅助函数 -------------------

    // 添加字符串
    public void AddString(string str) {
        Int32 len = str.Length;
        byte[] lenBytes = BitConverter.GetBytes((len));
        byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(str);
        if(bytes == null) {
            bytes = lenBytes.Concat(strBytes).ToArray();
        }
        else {
            bytes = bytes.Concat(lenBytes).Concat(strBytes).ToArray();
        }
    }

    // 从字节数组的start处来说读取字符串
    public string GetString(int start, ref int end) {
        if(bytes == null) {
            return "";
        }
        if(bytes.Length < start + sizeof(Int32)) {
            return "";
        }
        Int32 strLen = BitConverter.ToInt32(bytes, start);
        if(bytes.Length < start + sizeof(Int32) + strLen) {
            return "";
        }
        string str = System.Text.Encoding.UTF8.GetString(bytes, start + sizeof(Int32),
                                                         strLen);
        end = start + sizeof(Int32) + strLen;
        return str;
    }
    public string GetString(int start) {
        int end = 0;
        return GetString(start, ref end);
    }

    // 添加整数
    public void AddInt(int num) {
        byte[] numBytes = BitConverter.GetBytes(num);
        if(bytes == null) {
            bytes = numBytes;
        }
        else {
            bytes = bytes.Concat(numBytes).ToArray();
        }
    }

    // 获取整数
    public int GetInt(int start, ref int end) {
        if(bytes == null) {
            return 0;
        }
        if(bytes.Length < start + sizeof(Int32)) {
            return 0;
        }
        end = start + sizeof(Int32);
        return BitConverter.ToInt32(bytes, start);
    }
    public int GetInt(int start) {
        int end = 0;
        return GetInt(start, ref end);
    }

    // 添加浮点数
    public void AddFloat(float num) {
        byte[] numBytes = BitConverter.GetBytes(num);
        if(bytes == null) {
            bytes = numBytes;
        }
        else {
            bytes = bytes.Concat(numBytes).ToArray();
        }
    }

    // 取得浮点数
    public float GetFloat(int start, ref int end) {
        if(bytes == null) {
            return 0;
        }
        if(bytes.Length < start + sizeof(float)) {
            return 0;
        }
        end = start + sizeof(float);
        return end;
    }
    public float GetFloat(int start) {
        int end = 0;
        return GetFloat(start, ref end);
    }

	private byte[] CopyToBig(byte[] bBig, byte[] bSmall)
	{
		byte[] tmp = new byte[bBig.Length + bSmall.Length];
		System.Buffer.BlockCopy(bBig, 0, tmp, 0, bBig.Length);
		System.Buffer.BlockCopy(bSmall, 0, tmp, bBig.Length, bSmall.Length);
		return tmp;
	}
}
