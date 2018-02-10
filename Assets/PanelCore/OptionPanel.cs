using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : PanelBase
{
    private Button startBtn;
    private Button closeBtn;
    private Dropdown dropDown1;
    private Dropdown dropDown2;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "OptionPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        // 开始按钮
        startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
        startBtn.onClick.AddListener(OnStartClick);
        // 关闭按钮
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        closeBtn.onClick.AddListener(OnCloseClick);
        //下拉数量框
        dropDown1 = skinTrans.Find("Dropdown1").GetComponent<Dropdown>();
        dropDown2 = skinTrans.Find("Dropdown2").GetComponent<Dropdown>();
    }
    #endregion

    public void OnStartClick() {
        int n1 = dropDown1.value + 1;
        int n2 = dropDown2.value + 1;
        Battle.instance.StartTwoCampBattle(n1, n2);
        Close();
    }

    public void OnCloseClick() {
        Close();
    }
}
