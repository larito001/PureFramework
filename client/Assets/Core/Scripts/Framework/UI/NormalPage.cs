// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// /// <summary>
// /// 普通打开方式，新UI加载完成后再隐藏旧UI
// /// </summary>
// public class NormalPage : UITypeBase
// {
//     GameObject root;
//
//     public override void Init(GameObject root)
//     {
//         this.root = root;
//     }
//
//     public override void Hide(UIEnum uiEnum)
//     {
//         if (!handlers.ContainsKey(uiEnum))
//         {
//             return;
//         }
//
//         if (handlers.TryGetValue(uiEnum, out UIPageHandler handler))
//         {
//             handler.OnHide();
//             uiList.Remove(uiEnum);
//         }
//     }
//
//     public override void Show(UIEnum uiEnum)
//     {
//         UIPageHandler newHandler;
//         if (!handlers.TryGetValue(uiEnum, out newHandler) || newHandler == null)
//         {
//             Debug.LogError($"[NormalPage] Show: Failed to get handler for {uiEnum}");
//             return;
//         }
//
//         // 设置加载完成的回调
//         newHandler.SetLoadCallback(() =>
//         {
//             // // 获取当前显示的UI（如果有的话）
//             // UIEnum previousUI = uiList.Count > 1 ? uiList[uiList.Count - 2] : UIEnum.None;
//             // var current = uiEnum;
//             // // 在新UI加载完成后，先隐藏之前的UI，但不隐藏自己
//             // if (previousUI != UIEnum.None && previousUI != current && handlers.ContainsKey(previousUI))
//             // {
//             //     Hide(previousUI);
//             // }
//         });
//
//         // 添加新UI到列表
//         uiList.Add(uiEnum);
//         
//         // 开始加载新UI
//         newHandler.Load(root.transform);
//     }
//     public override void Clear()
//     {
//
//       
//         foreach (var uiPageHandler in handlers)
//         {
//          
//             uiPageHandler.Value.Distory();
//         }
//         uiList.Clear();
//         handlers.Clear();
//     
//     }
// }
