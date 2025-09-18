using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using YOTO;

public class AnimatorCtrl : MonoBehaviour
{

    public Animator animator;
    public Rig rig; 
    
    public void OnDead()
    {
        

    }
    public void OnDrink()
    {
        rig.weight = 0;
        animator.SetTrigger("Drink");
        YOTOFramework.timeMgr.DelayCall(() =>
        {
            if (rig != null)
            {
                rig.weight = 1;   
            }
          
        }, 1f);
    }
    
}
