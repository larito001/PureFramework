using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class StartPanel : UIPageBase
{
    public Button startBtn;
    public Button createBtn;
    // textMeshpro的input
    public TMP_InputField IPInput;
    public TMP_InputField NameInput;
    public TMP_InputField PortInput;
    public override void OnLoad()
    {
        PortInput. text = "9999";  
        IPInput.text = "127.0.0.1";
        NameInput.text = "testName";
        startBtn.onClick.AddListener(() =>
        {
            LoginPlugin.Instance.Name=NameInput.text;
            YOTOFramework.netMgr.JoinHost(IPInput.text, ushort.Parse(PortInput.text));
            // CloseSelf();
      
        });
        createBtn.onClick .AddListener(() =>
        {
            LoginPlugin.Instance.Name=NameInput.text;
            YOTOFramework.netMgr.CreateHost(ushort.Parse(PortInput. text));
        });
    
  
    }

    public override void OnShow()
    {

    }

    public override void OnHide()
    {

    }
}
