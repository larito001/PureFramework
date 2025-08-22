using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YOTO
{
    public class YOTOFramework : SingletonMono<YOTOFramework>
    {
        private bool isInit = false;
        public static  TimeMgr timeMgr = new TimeMgr();
        public static  Logger logger = new Logger();
        public static  ToolMgr toolMgr = new ToolMgr();
        public static  EventMgr eventMgr = new EventMgr();
        public static  StoreMgr storeMgr = new StoreMgr();
        public static  ResMgr resMgr = new ResMgr();
        public static  GameInputMgr gameInputMgr = new GameInputMgr();
        public static  SceneMgr sceneMgr = new SceneMgr();
        public static  CameraMgr cameraMgr = new CameraMgr();
        public static  UIMgr uIMgr = new UIMgr();
        public static  EntityMgr entityMgr = new EntityMgr();
        public static  NetMgr netMgr = new NetMgr();
        private static PluginManager PluginMgr = new PluginManager();
        private static GameDataManager GameDataManager = new GameDataManager();
        public void Init()
        {
            if (!isInit)
            {
                isInit = true;
                netMgr.Init();
                storeMgr.Init();
                GameDataManager.Init();
                PluginMgr.InitPlugins();
                toolMgr.Init();
                logger.Init();
                resMgr.Init();
                timeMgr.Init();
                gameInputMgr.Init();
                cameraMgr.Init();
                entityMgr.Init();
                uIMgr.Init();
                sceneMgr.Init();
  
            }

            Debug.Log("YTLOG初始化完成");
        }

      

        private void OnEnable()
        {
        }

        private void Start()
        {
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            entityMgr._FixedUpdate(dt);
            netMgr.FixUpdate(dt);
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            timeMgr.Update(dt);
            entityMgr._Update(dt);
            sceneMgr.Update(dt);
      
        }

        private void LateUpdate()
        {
            cameraMgr.LateUpdate();
        }

        private void OnDestroy()
        {
            Debug.Log("YTLOG销毁完成");
            logger.OnDisable();
            gameInputMgr.DisableGamePlayInput();
            isInit = true;
            // poolMgr=null;
      
            toolMgr=null;
            logger=null;
            resMgr=null;
            timeMgr=null;
            gameInputMgr=null;
            cameraMgr=null;
            entityMgr=null;
            uIMgr=null;
            sceneMgr=null;
            System.GC.Collect();
            PluginMgr.Unload();
            PluginMgr = null;
            GameDataManager.Unload();
            storeMgr=null;
            Unload();
        }

        private void OnGUI()
        {
            logger.OnGUI();
        }
    }
}