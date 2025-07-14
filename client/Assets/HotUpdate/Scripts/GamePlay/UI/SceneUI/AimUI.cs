using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class AimUI : UIPageBase
{
    public override void OnLoad()
    {
        YOTOFramework.eventMgr.AddEventListener<Vector2>(YOTO.EventType.Touch, Touch);
        Cursor.visible = false;

    }

    private void Touch(Vector2 pos)
    {
        this.transform.position = pos;
    }

    public override void OnShow()
    {
        this.canvasGroup.blocksRaycasts = false;
    }

    public override void OnHide()
    {
        Cursor.visible = true;
        YOTOFramework.eventMgr.RemoveEventListener<Vector2>(YOTO.EventType.Touch, Touch);
    }
}
