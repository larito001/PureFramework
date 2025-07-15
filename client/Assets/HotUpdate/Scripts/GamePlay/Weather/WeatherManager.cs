using System.Collections;
using System.Collections.Generic;
using log4net.Core;
using UnityEngine;
using YOTO;

public enum DayTimeType
{
    Morning=0,
    MidDay,
    Afternoon,
    NightTime,
    Drak
}
public enum Weather
{
    Normal=0,
    Storm,
    Shower
}


public class WeatherManager : SingletonMono<WeatherManager>
{
    private Light dirLight;
    private Coroutine transitionCoroutine;
    private RainParticleBase showerEffect;
    private RainParticleBase stormEffect;
    public void Init()
    {
        dirLight = GameObject.Find("DirLight").GetComponent<Light>();
        
        YOTOFramework.resMgr.LoadGameObject("Assets/HotUpdate/prefabs/Realistic Rain FX/Prefabs/Distort/shower.prefab",
            LoadShowerComplete);
        YOTOFramework.resMgr.LoadGameObject("Assets/HotUpdate/prefabs/Realistic Rain FX/Prefabs/Distort/Storm.prefab",
             LoadStromComplete);
    }

    public override void Unload()
    {
        GameObject.Destroy(showerEffect);
        GameObject.Destroy(stormEffect);
        base.Unload();
    }



    public void ChangeWeather(Weather weather)
    {
        if (showerEffect != null) showerEffect.StopAndFadeOut();
        if (stormEffect != null) stormEffect.StopAndFadeOut();

        switch (weather)
        {
            case Weather.Normal:
                break;
            case Weather.Shower:
                if (showerEffect != null) showerEffect.PlayRain();
                break;
            case Weather.Storm:
                if (stormEffect != null) stormEffect.PlayRain();
                break;
        }
    }
    public void ChangeDayTime(DayTimeType time)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        Color targetColor = Color.white;
        Vector3 targetDirection = Vector3.zero;

        switch (time)
        {
            case DayTimeType.Morning:
                targetColor = new Color(1.0f, 0.7f, 0.5f);
                targetDirection = new Vector3(30f, 30f, 0f);
                break;
            case DayTimeType.MidDay:
                targetColor = new Color(1.0f, 0.95f, 0.85f);
                targetDirection = new Vector3(90f, 0f, 0f);
                break;
            case DayTimeType.Afternoon:
                targetColor = new Color(1.0f, 0.85f, 0.6f);
                targetDirection = new Vector3(150f, 30f, 0f);
                break;
            case DayTimeType.NightTime:
                targetColor = new Color(0.1f, 0.1f, 0.2f);
                targetDirection = new Vector3(220f, 0f, 0f);
                break;
            case DayTimeType.Drak:
                targetColor = new Color(0.0f, 0.0f, 0.05f);
                targetDirection = new Vector3(270f, 0f, 0f);
                break;
        }

        transitionCoroutine = StartCoroutine(TransitionLight(targetColor, targetDirection, 5f));
    }
    private void LoadShowerComplete(GameObject obj )
    {
        showerEffect = GameObject.Instantiate(obj).GetComponent<RainParticleBase>();
        showerEffect.transform.position += new Vector3(0, 4, 0);
    }
    private void LoadStromComplete(GameObject obj)
    {
        stormEffect = GameObject.Instantiate(obj).GetComponent<RainParticleBase>();
        stormEffect.transform.position += new Vector3(0, 4, 0);
    }
    private IEnumerator TransitionLight(Color targetColor, Vector3 targetEuler, float duration)
    {
        Color startColor = dirLight.color;
        Quaternion startRotation = dirLight.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            dirLight.color = Color.Lerp(startColor, targetColor, t);
            dirLight.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        dirLight.color = targetColor;
        dirLight.transform.rotation = targetRotation;
    }
    
}
