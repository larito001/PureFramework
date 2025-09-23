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
    public delegate void TimerCallback();
    public class TimeMgr
    {
    
        private Dictionary<TimerCallback,Timer> timers = new Dictionary<TimerCallback,Timer>();
        private TimeScaler scaler = new TimeScaler();
        public void Init()
        {

        }
        public void RemoveTimer(TimerCallback callback)
        {
            if (timers.ContainsKey(callback))
            {
                timers[callback].Stop();
                timers.Remove(callback);
            }
        }
        public void DelayCall(TimerCallback callback, float delayTime)
        {
            Timer timer = new Timer();
            timer.Init(callback, TimerType.Time, delayTime, 1);
            timers.Add(callback,timer);
        }
        
        public void DelayCallFram(TimerCallback callback, int delayFrames)
        {
            Timer timer = new Timer();
            timer.Init(callback, TimerType.Frame, delayFrames, 1);
            timers.Add(callback,timer);
        }
        public void LoopCall(TimerCallback callback, float time,int loopCount=-1)
        {
            Timer timer = new Timer();
            timer.Init(callback, TimerType.Time, time, loopCount);  // -1表示无限循环
            timers.Add(callback,timer);
        }
        public void LoopCallFram(TimerCallback callback, int frames)
        {
            Timer timer = new Timer();
            timer.Init(callback, TimerType.Frame, frames, -1);  // -1表示无限循环
            timers.Add(callback,timer);
        }
        public void SetTimeScale(float scale)
        {
            scaler.SetTimeScale(scale);
        }
        public void Update(float deltaTime)
        {
            var shoot = timers.ToList();
            foreach (var keyValuePair in shoot)
            {
                if (timers.ContainsKey(keyValuePair.Key))
                {
                    keyValuePair.Value.Update(deltaTime);
                    if (timers[keyValuePair.Key].IsStop())
                    {
                        timers.Remove(keyValuePair.Key);    
                    }
                  
                }
         
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
        private TimerCallback callback;
        public void Init(TimerCallback callback, TimerType type, float stepTime,  int loopCount = -1)
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