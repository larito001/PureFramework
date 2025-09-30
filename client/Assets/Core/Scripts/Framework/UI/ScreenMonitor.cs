
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class ScreenMonitor
    {
        public delegate void ScreenResizeDelegate(int width, int height);
        public event ScreenResizeDelegate OnScreenResize;

        private int mLastWidth;
        private int mLastHeight;

        public ScreenMonitor()
        {
//            ResetScreen();
        }

        public void Update()
	    {
		    if (mLastWidth != Screen.width || mLastHeight != Screen.height)
            {
                if (OnScreenResize != null)
                    OnScreenResize(Screen.width, Screen.height);
                ResetScreen();
            }
	    }

        private void ResetScreen()
        {
            mLastWidth = Screen.width;
            mLastHeight = Screen.height;
        }
    }
