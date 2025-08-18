// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// public abstract class PoolBaseObject
// {
//     public bool  __isRecycle { get; set; }
//     public bool isRecycle { get { return __isRecycle; } private set { } }
//     public abstract void ResetAll();
//     public abstract void OnStart();
//
// }
// public abstract class PoolBaseGameObject : MonoBehaviour
// {
//     /// <summary>
//     /// ע�⣬�벻Ҫ��ʽ�޸�
//     /// </summary>
//     public bool __isRecycle { get; set; }
//     /// <summary>
//     ///��ȡ״̬����Ҫ��ʹ��֮ǰ������Ƿ��Ѿ��������ˣ����û�����գ��ͷ��ĵ��á�
//     ///��ֹǰ���Ѿ������˵��ֵ���һ�ε��������
//     /// </summary>
//     public bool isRecycle { get { return  __isRecycle; } private set { } }
//     /// <summary>
//     /// ��ע�⣬�˷������ڷ��롢ȡ�������������ʱ���ã�������֤���ᱨ�������ڲ����غ����ֹ��������ء�
//     /// </summary>
//     public abstract void ResetAll();
//     public abstract void OnStart();
// }