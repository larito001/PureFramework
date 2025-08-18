using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YOTO
{
    public class Logger
    {
        public bool isopen = true;
        const int maxLines = 30;
        const int maxLineLength = 500;
        private string _logStr = "";
        int LogNumber = 0;
        private readonly List<string> _lines = new List<string>();

        public int fontSize = 30;

        public void Init() 
        { 
#if UNITY_EDITOR
            isopen = false;
#else
            isopen = true;
#endif
            Application.logMessageReceived += Log; 
        }
        public void OnDisable() { Application.logMessageReceived -= Log; }

        public void Log(string logString, string stackTrace, LogType type)
        {
            foreach (var line in logString.Split('\n'))
            {
                if (line.Length <= maxLineLength)
                {
                    _lines.Add(line);
                    continue;
                }
                var lineCount = line.Length / maxLineLength + 1;
                for (int i = 0; i < lineCount; i++)
                {
                    if ((i + 1) * maxLineLength <= line.Length)
                    {
                        _lines.Add(line.Substring(i * maxLineLength, maxLineLength));
                    }
                    else
                    {
                        _lines.Add(line.Substring(i * maxLineLength, line.Length - i * maxLineLength));
                    }
                }
            }
            if (_lines.Count > maxLines)
            {
                _lines.RemoveRange(0, _lines.Count - maxLines);
            }
            _logStr = string.Join("\n", _lines);
            LogNumber = _lines.Count; // ����ʵ�ʵ�����
        }

        private Vector2 scrollPosition = Vector2.zero; // ���ڱ������λ��

        public void OnGUI()
        {
            if (!isopen) return;
            
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
               new Vector3(1, 1, 1.0f));
            
            Rect externalRect = new Rect(10, 10, 2000, 500);

            // ��̬�����ڲ����θ߶�
            float contentHeight = LogNumber * (fontSize + 5); // ÿ��ռ�ø߶ȸ��������С��̬����
            Rect internalRect = new Rect(0, 0, 2000, contentHeight);

            // ��ʼһ���ɹ�����ͼ
            scrollPosition = GUI.BeginScrollView(externalRect, scrollPosition, internalRect);

            // ���� Label
            GUIStyle titleStyle2 = new GUIStyle();
            titleStyle2.fontSize = fontSize;
            titleStyle2.normal.textColor = new Color(0, 0, 0, 1);

            // ���л�����־����
            float yOffset = 0;
         
            foreach (var line in _lines)
            {
                GUI.Label(new Rect(0, yOffset, 2000, fontSize + 5), line, titleStyle2);
                yOffset += fontSize + 5; // ÿ�е����߶�
            }

            // ���� ScrollView
            GUI.EndScrollView();
        }
    }
}
