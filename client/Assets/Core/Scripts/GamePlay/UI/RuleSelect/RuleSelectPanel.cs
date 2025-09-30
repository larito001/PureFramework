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
        card1Ctrl.transform.localScale=new Vector3();
        card2Ctrl.transform.localScale=new Vector3();
        card3Ctrl.transform.localScale=new Vector3();
    }

    public override void OnShow()
    {
       var rules = StagePlugin.Instance.GetRules();
       card1Ctrl.SetCard(rules[0]);
       card2Ctrl.SetCard(rules[1]);
       card3Ctrl.SetCard(rules[2]);
       card1Ctrl.PlayEntranceAnimation(0 ); 
       card2Ctrl.PlayEntranceAnimation(0);
       card3Ctrl.PlayEntranceAnimation(0 );
       YOTOFramework.timeMgr.DelayCall(CloseSelf,5f);
    }

    private void OnCardFinsh()
    {
        card1Ctrl.PlayExitAnimation();
        card2Ctrl.PlayExitAnimation();
        card3Ctrl.PlayExitAnimation();
    }

    public override void OnHide()
    {
        OnCardFinsh();
    }

    public override void OnResize()
    {
        
    }
}
