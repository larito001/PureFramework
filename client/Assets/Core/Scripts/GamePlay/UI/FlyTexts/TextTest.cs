using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTest : MonoBehaviour
{
    public UGUIFloatingTextBatch[] uGUIFloatingTextBatch;
    public int frame;
    // Start is called before the first frame update
    void Start()
    {
        uGUIFloatingTextBatch = this.GetComponentsInChildren<UGUIFloatingTextBatch>();
    }
    private int frameNum = 0;
    int index = 0;
    // Update is called once per frame
    void Update()
    {
        frameNum++;
        if (frameNum > frame)
        {
            float randomX = UnityEngine.Random.Range(-500f, 500f); // X 方向的随机位置
            float randomY = UnityEngine.Random.Range(-300f, 300f); // Y 方向的随机位置
            Vector3 randomPosition = new Vector3(randomX, randomY, 0); // 生成随机位置
            uGUIFloatingTextBatch[index].GenerateMesh();

            index++;
            if (index >= uGUIFloatingTextBatch.Length)
            {
                index = 0;
            }
            frameNum = 0;
        }

    }
}
