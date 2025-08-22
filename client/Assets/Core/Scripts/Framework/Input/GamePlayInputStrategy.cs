using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
namespace YOTO
{
    public class GamePlayInputStrategy : MInput.IGamePlayActionActions
    {

        private bool isAming = false;
        public void OnScroll(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                YOTOFramework.eventMgr.TriggerEvent<float>(YOTO.YOTOEventType.Scroll, context.ReadValue<Vector2>().y);
            }

        }

      

        public void OnLook(InputAction.CallbackContext context)
        {
            // Debug.Log("look");
            if (context.phase == InputActionPhase.Performed)
            {
                YOTOFramework.eventMgr.TriggerEvent<Vector2>(YOTOEventType.Look, context.ReadValue<Vector2>()); 
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
             if (context.phase == InputActionPhase.Performed)
            {
                YOTOFramework.eventMgr.TriggerEvent<Vector2>(YOTOEventType.Move, context.ReadValue<Vector2>()); 
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                YOTOFramework.eventMgr.TriggerEvent<Vector2>(YOTOEventType.Move, context.ReadValue<Vector2>());
            }
        }
        
        //这个只有在拖、移动的时候触发
        public void OnTouch(InputAction.CallbackContext context)
         {
         
             if (context.phase == InputActionPhase.Performed)
             {
                 YOTOFramework.eventMgr.TriggerEvent<Vector2>(YOTO.YOTOEventType.Touch, context.ReadValue<Vector2>());
             }
         
         
         }

        public void OnTouchAddition(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                 YOTOFramework.eventMgr.TriggerEvent(YOTO.YOTOEventType.TouchPress);

            }else if (context.phase == InputActionPhase.Canceled) // 松开触发事件
            {

                YOTOFramework.eventMgr.TriggerEvent(YOTO.YOTOEventType.TouchRelease);
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
             if (context.phase == InputActionPhase.Performed)
            {
                YOTOFramework.eventMgr.TriggerEvent(YOTO.YOTOEventType.PressLeftMouse);
            }
             else if (context.phase == InputActionPhase.Canceled) // 松开触发事件
             {
                 YOTOFramework.eventMgr.TriggerEvent(YOTO.YOTOEventType.FireRelease);
             }
        }


        //public void OnTouch(InputAction.CallbackContext context)
        //{
        //    if (context.phase == InputActionPhase.Performed)
        //    {
        //        YOTOFramework.eventMgr.TriggerEvent<Vector2>(YOTO.EventType.Touch, context.ReadValue<Vector2>());
        //    }
        //}
        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                // YOTOFramework.eventMgr.TriggerEvent(EventType.Sprint);
            }
        }

        public void OnUseItem(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                // YOTOFramework.eventMgr.TriggerEvent(EventType.UseItem);
            }
        }

        public void OnCooking(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                // YOTOFramework.eventMgr.TriggerEvent(EventType.Cooking);
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                YOTOFramework.eventMgr.TriggerEvent(YOTOEventType.TryReload);
            }
        }

        public void OnOne(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
               YOTOFramework.eventMgr.TriggerEvent<int>(YOTOEventType.KeyBoardNumClick,1);
            }
        }

        public void OnTwo(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                YOTOFramework.eventMgr.TriggerEvent<int>(YOTOEventType.KeyBoardNumClick,2);
            }
        }
    }
}