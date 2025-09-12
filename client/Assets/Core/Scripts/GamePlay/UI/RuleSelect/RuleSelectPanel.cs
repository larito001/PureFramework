using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class RuleSelectPanel : UIPageBase
{
    public CardCtrl card1Ctrl;
    public CardCtrl card2Ctrl;
    public CardCtrl card3Ctrl;
    public override void OnLoad()
    {
        
    }

    public override void OnShow()
    {
       var rules = StagePlugin.Instance.GetRules();
       card1Ctrl.SetCard(rules[0]);
       card2Ctrl.SetCard(rules[1]);
       card3Ctrl.SetCard(rules[2]);
       YOTOFramework.timeMgr.DelayCall(CloseSelf,5f);
    }

    public override void OnHide()
    {
        
    }
}
