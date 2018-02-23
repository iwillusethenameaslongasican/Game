using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//提示面板
public class TipPanel : PanelBase
{
    private Text text;
    private Button btn;
    string str = "";

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "TipPanel";
        layer = PanelLayer.Tips;
        if(args.Length == 1) {
            str = (string)args[0];
        }
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        //文字
        text = skinTrans.Find("Text").GetComponent<Text>();
        text.text = str;
        //关闭按钮
        btn = skinTrans.Find("Btn").GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);
    }
    #endregion

    // 按下 知道了 按钮
    public void OnBtnClick(){
        Close();
    }

}
