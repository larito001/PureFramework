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
    private bool isDead;
    public void OnDead()
    {
        isDead = true;
        rig.weight = 0;
        animator.SetBool("Die",true);
    }
    public void OnDrink()
    {
        rig.weight = 0;
        animator.SetTrigger("Drink");
        YOTOFramework.timeMgr.DelayCall(() =>
        {
            if (rig != null&&!isDead)
            {
                rig.weight = 1;   
            }
          
        }, 6f);
    }
    
}
