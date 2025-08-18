using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class GameStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        YOTOFramework.Instance.Init();
        GameRoot.Instance.Init();
    }

  
}
