// using System.Collections.Generic;
// using UnityEngine;
//
// /// <summary>
// /// 栈式UI管理器
// /// </summary>
// public class StackPage : UITypeBase
// {
//     private GameObject root;
//     private Stack<UIEnum> uiStack = new Stack<UIEnum>();
//
//     public override void Init(GameObject root)
//     {
//         this.root = root;
//     }
//
//     public override void Show(UIEnum uIEnum)
//     {
//         if (!handlers.ContainsKey(uIEnum))
//         {
//             return;
//         }
//
//         // 如果栈不为空，隐藏当前顶部UI
//         if (uiStack.Count > 0)
//         {
//             UIEnum topUI = uiStack.Peek();
//             if (handlers.TryGetValue(topUI, out UIPageHandler topHandler))
//             {
//                 topHandler.OnHide();
//             }
//         }
//
//         // 显示新UI并入栈
//         if (handlers.TryGetValue(uIEnum, out UIPageHandler handler))
//         {
//             handler.Load(root.transform);
//             uiStack.Push(uIEnum);
//         }
//     }
//
//     public override void Hide(UIEnum uIEnum)
//     {
//         if (!handlers.ContainsKey(uIEnum) || uiStack.Count == 0)
//         {
//             return;
//         }
//
//         // 如果要隐藏的不是栈顶UI，直接返回
//         if (!uiStack.Peek().Equals(uIEnum))
//         {
//             return;
//         }
//
//         // 移除栈顶UI
//         uiStack.Pop();
//         if (handlers.TryGetValue(uIEnum, out UIPageHandler handler))
//         {
//             handler.OnHide();
//         }
//
//         // 显示新的栈顶UI
//         if (uiStack.Count > 0)
//         {
//             UIEnum topUI = uiStack.Peek();
//             if (handlers.TryGetValue(topUI, out UIPageHandler topHandler))
//             {
//                 topHandler.Load(root.transform);
//             }
//         }
//     }
//
//     public override void Clear()
//     {       
//         foreach (var uiPageHandler in handlers)
//         {
//                 uiPageHandler.Value.Distory();
//         }
//         uiStack.Clear();
//         handlers.Clear();
//      
//     }
// } 