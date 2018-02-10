using UnityEngine;
using System.Collections;

// 网络管理
public class NetMgr
{
    public static Connection servConn = new Connection();
    // update
    public static void Update(){
        servConn.Update();
    }

}
