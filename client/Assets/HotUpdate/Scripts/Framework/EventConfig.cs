using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YOTO
{    
    public enum EventType
    {
       //input :
       GameStart,
       Move,

       Sprint,
       //UI
       Loading,
       //touch
       Touch,
       TouchPress,
       TouchRelease,
       TouchMove,
       Scroll,
       Fire,
       PressLeftMouse,
       FireRelease,
       RefreshMousePos,
       TryReload,
       KeyBoardNumClick,
       Look,
       
       //刷新资源页面
       RefreshResInfo,
    }


}
