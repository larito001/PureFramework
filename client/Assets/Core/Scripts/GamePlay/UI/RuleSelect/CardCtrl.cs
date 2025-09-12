using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class CardCtrl : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;
    private GameRule _rule;
    public Button btn;
    public void SetCard(GameRule  rule)
    {
        _rule=rule;
        title.text = rule.roleName;
        content.text = rule.roleDetail;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        StagePlugin.Instance.SetRule(_rule.ruleId);
        YOTOFramework.uIMgr.Hide(UIEnum.RuleSelectPanel);
    }
}
