using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using YOTO;


public class GameMap : UIPageBase
{
    public List<MapButton> btnList = new List<MapButton>();
    public override void OnLoad()
    {
        for (var i = 0; i < btnList.Count; i++)
        {
            btnList[i].SetCallBack(EnterLevel);
        }
    }

    private void EnterLevel(int level)
    {
        YOTOFramework.uIMgr.Show(UIEnum.StartLoadingPanel);
        YOTOFramework.timeMgr.DelayCall(() =>
        {
            YOTOFramework.sceneMgr.LoadScene<NormalScene>(new NormalScene.NormalSceneParam()
            {
                level = level
            }); 
            YOTOFramework.sceneMgr.cameraCtrl.UsePlayerCamera();
        },1);
        YOTOFramework.timeMgr.DelayCall(() =>
        {
            CloseSelf();
            YOTOFramework.uIMgr.Hide(UIEnum.StartLoadingPanel);
        },8);
    }
    public override void OnShow()
    {

    }

    public override void OnHide()
    {
    
    }
}
