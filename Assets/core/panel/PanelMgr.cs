using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PanelMgr : MonoBehaviour {
    // 单例
    public static PanelMgr instance;
    // 画板
    public GameObject canvas;
    // 面板 用于存放已打开的面板
    public Dictionary<string, PanelBase> dict;
    // 层级 存放多个层级所对应的父物体
    private Dictionary<PanelLayer, Transform> layerDict;

    // 开始
    public void Awake()
    {
        instance = this;
        InitLayer();
        dict = new Dictionary<string, PanelBase>();
    }

    // 初始化层
    private void InitLayer() {
        // 画布
        canvas = GameObject.Find("Canvas");
        if(canvas == null) {
            Debug.LogError("PanelMgr.InitLayer fail, canvas is null");
        }
        // 各个层级
        layerDict = new Dictionary<PanelLayer, Transform>();

        foreach(PanelLayer pl in Enum.GetValues(typeof(PanelLayer))) {
            string PLName = pl.ToString();
            Transform tf = canvas.transform.Find(PLName);
            layerDict.Add(pl, tf);
        }
    }

    // 打开面板
    public void OpenPanel<T>(string skinPath, params object[] args) 
        where T : PanelBase {
        // 已经打开
        string PBName = typeof(T).ToString();
        if(dict.ContainsKey(PBName)) {
            return;
        }
        // 面板脚本
        PanelBase panel = canvas.AddComponent<T>();
        panel.Init(args);
        dict.Add(PBName, panel);
        // 加载皮肤
        skinPath = (skinPath != "" ? skinPath : panel.skinPath);
        GameObject skin = Resources.Load<GameObject>(skinPath);
        if(skin == null) {
            Debug.LogError("panelMgr.OpenPanel fail, skin is null, skinPath" +
                           " = " + skinPath);
        }
        panel.skin = (GameObject)Instantiate(skin);
        // 坐标
        Transform skinTrans = panel.skin.transform;
        PanelLayer layer = panel.layer;
        Transform parent = layerDict[layer];
        skinTrans.SetParent(parent, false);
        // panel的生命周期
        panel.OnShowing();
        // anmimation
        panel.OnShowed();

    }

    // 关闭面板
    public void ClosePanel(string name) {
        PanelBase panel = (PanelBase)dict[name];
        if(panel == null) {
            return;
        }

        panel.OnClosing();
        dict.Remove(name);
        panel.OnClosed();
        GameObject.Destroy(panel.skin);
        Component.Destroy(panel);
    }
}

// 分层类型
public enum PanelLayer
{
    // 面板
    Panel,
    // 提示
    Tips,
}
