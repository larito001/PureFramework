using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
namespace YOTO
{
    public enum TimerType
    {
        Time,
        Frame
    }
    public class TimeMgr
    {
        private Dictionary<UnityAction,Timer> timers = new Dictionary<UnityAction,Timer>();
        private TimeScaler scaler = new TimeScaler();
        public void Init()
        {

        }
        public void RemoveTimer(UnityAction callback)
        {
            if (timers.ContainsKey(callback))
            {
                timers[callback].Stop();
            }
        }
        public void DelayCall(UnityAction callback, float delayTime)
        {
            Timer timer = new Timer();
            timer.Init(callback, TimerType.Time, delayTime, 1);
            timers.Add(callback,timer);
        }
        public void DelayCallFram(UnityAction callback, int delayFrames)
        {
            Timer timer = new Timer();
            timer.Init(callback, TimerType.Frame, delayFrames, 1);
            timers.Add(callback,timer);
        }
        public void LoopCall(UnityAction callback, float time,int loopCount=-1)
        {
            Timer timer = new Timer();
            timer.Init(callback, TimerType.Time, time, loopCount);  // -1表示无限循环
            timers.Add(callback,timer);
        }
        public void LoopCallFram(UnityAction callback, int frames)
        {
            Timer timer = new Timer();
            timer.Init(callback, TimerType.Frame, frames, -1);  // -1表示无限循环
            timers.Add(callback,timer);
        }
        public void SetTimeScale(float scale)
        {
            scaler.SetTimeScale(scale);
        }
        List<UnityAction> removeList = new List<UnityAction>();
        public void Update(float deltaTime)
        {
           
            foreach (var keyValuePair in timers.ToList())
            {
                if ( keyValuePair.Value.IsStop())
                {
                    removeList.Add(keyValuePair.Key);
                }
                else
                {
                    keyValuePair.Value.Update(deltaTime);
                }
              
            }

            for (int i = removeList.Count - 1; i >= 0; i--)
            {
                timers.Remove(removeList[i]);
                removeList.RemoveAt(i);
            }
        }
    }
    class Timer { 
        private TimerType type;
        private bool  isStoped;
        private bool isPause;
        private int loopCount;
        private float elapsedTime;
        private float stepTime;
        private UnityAction callback;
        public void Init(UnityAction callback, TimerType type, float stepTime,  int loopCount = -1)
        {
            this.type = type;
            this.loopCount = loopCount;
            this.stepTime = stepTime;
            this.callback = callback;
            isStoped = false;
        }
        public bool IsStop()
        {
            return isStoped;
        }
        public bool isComplete()
        {
            return loopCount == 0;
        }
        public void ChangeStepTime(float stepTime)
        {
            this.stepTime = stepTime;
        }
        public void Pause()
        {
            isPause = true;
        }
        public void Resume()
        {
            isPause = false;
        }
        public void Stop()
        {
            isStoped = true;
        }
        public void Update(float deltaTime)
        {
            if (isPause) return;
            if(isStoped) return;
            if (loopCount == 0) return;
            if (type==TimerType.Time)
            {
                elapsedTime += deltaTime;
                if(elapsedTime>= stepTime)
                {
                    elapsedTime -= stepTime;
                    if (callback!=null)
                    {
                        try {
                            callback();
                        } catch(Exception e)
                        {
                            Debug.Log("Timerִд:"+e);
                        }
                    }
                    loopCount--;
                }

            }
            else if (type == TimerType.Frame)
            {
                ++elapsedTime;
                if (elapsedTime>=stepTime)
                {
                    elapsedTime = 0;
                    try
                    {
                        callback();
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Timerִд:" + e);
                    }
                    loopCount--;
                }
            }
            if (loopCount==0)
            {
                Stop();
            }
        }
    }
    public class TimeScaler
    {
        public void SetTimeScale(float scale)
        {

        }
    }
}