using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;
using EventType = YOTO.EventType;

public class StartPanel : UIPageBase
{
    public Button startBtn;
    public Button createBtn;
    public Button RemoveBtn;
    // textMeshpro的input
    public TMP_InputField IPInput;
    public TMP_InputField PortInput;
    public override void OnLoad()
    {
        PortInput. text = "9999";  
        IPInput.text = "127.0.0.1";
        startBtn.onClick.AddListener(() =>
        {
            YOTOFramework.netMgr.JoinHost(IPInput.text, ushort.Parse(PortInput.text));
            // CloseSelf();
        });
        createBtn.onClick .AddListener(() =>
        {
            YOTOFramework.netMgr.CreateHost(ushort.Parse(PortInput. text));
        });
        RemoveBtn.onClick.AddListener(() =>
        {
            YOTOFramework.netMgr.LeaveHost();
        });
  
    }

    public override void OnShow()
    {

    }

    public override void OnHide()
    {

    }
}
