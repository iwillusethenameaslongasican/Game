using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Net;

// 消息分发
public class MsgDistribution{
    // 没帧处理消息的数量
    public int num = 15;
    // 消息列表
    public List<byte[]> msgList = new List<byte[]>();
    // 委托类型
    public delegate void Delegate(byte[] Byte);
    // 事件监听表
    private Dictionary<string, Delegate> eventDict = new Dictionary<string, Delegate>();
    // 一次性事件监听表
    private Dictionary<string, Delegate> onceDict = new Dictionary<string, Delegate>();

    // 在connection中调用
    public void Update() {
        // 每帧最多处理num条消息
        for (int i = 0; i < num; i ++) {
            if(msgList.Count > 0) {
                DispatchMsgEvent(msgList[0]);
                lock(msgList) {
                    msgList.RemoveAt(0);
                }
            }
            else {
                break;
            }
        }
    }

    // 消息分发
    public void DispatchMsgEvent(byte[] Byte) {
        ByteBuffer buffer = new ByteBuffer(Byte);
        int typeId = buffer.ReadInt();
        string name = ProtoHelper.IdToName(typeId);
        //Debug.Log("分发处理消息：" + name);
        if(eventDict.ContainsKey(name)) {
            eventDict[name](Byte);
        }
        if(onceDict.ContainsKey(name)) {
            onceDict[name](Byte);
            onceDict[name] = null;
            onceDict.Remove(name);
        }
    }

    // 添加监听事件
    public void AddListener(string name, Delegate cb) {
        if(eventDict.ContainsKey(name)) {
            eventDict[name] += cb;
        }
        else {
            eventDict[name] = cb;
        }
    }

    // 添加单次监听事件
    public void AddOnceListener(string name, Delegate cb) {
        if(onceDict.ContainsKey(name)) {
            onceDict[name] += cb;
        }
        else {
            onceDict[name] = cb;
        }
    }

    // 移除监听事件
    public void DelListener(string name, Delegate cb) {
        if(eventDict.ContainsKey(name)) {
            eventDict[name] -= cb;
            if(eventDict[name] == null) {
                eventDict.Remove(name);
            }
        }
    }

    // 移除单次监听事件
    public void DelOnceListener(string name, Delegate cb) {
        if(onceDict.ContainsKey(name)) {
            onceDict[name] -= cb;
            if(onceDict[name] == null) {
                onceDict.Remove(name);
            }
        }
    }
}
