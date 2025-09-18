using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YOTO
{    
    public enum YOTOEventType
    {

       //input :
       Move,
       Touch,
       PressLeftMouse,
       FireRelease,
       RefreshMousePos,
       Look,
       Space,
       
       //刷新
       RefreshRoleList,
       RefreshProgress,
       RefreshPlayerProperty,
       GameTimerNotify,
       LootTimerNotify,
       VotEndNotify,
    }


}
