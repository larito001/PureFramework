using System;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class SoundMgr
{
    private static SoundMgr _instance;
    public static SoundMgr Instance => _instance ??= new SoundMgr();

    private AudioSource _bgmSource;
    private readonly List<AudioSource> _sfxSources = new List<AudioSource>();

    private AudioClip _currentBgm;

    /// <summary>
    /// 初始化声音管理器
    /// </summary>
    public void Init()
    {
        // 创建 BGM Source
        _bgmSource = YOTOFramework.Instance.gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;

        // 初始化一个音效池
        for (int i = 0; i < 5; i++)
        {
            var sfx = YOTOFramework.Instance.gameObject.AddComponent<AudioSource>();
            sfx.playOnAwake = false;
            _sfxSources.Add(sfx);
        }

        PlayBGM("Sound/BGM1",0.3f);
    }

    /// <summary>
    /// 播放背景音乐（异步加载并自动播放）
    /// </summary>
    public void PlayBGM(string path, float volume = 1f)
    {
        YOTOFramework.resMgr.LoadAudio(path, (clip) =>
        {
            if (clip == null) return;

            // 释放旧的 BGM
            if (_currentBgm != null)
            {
                YOTOFramework.resMgr.ReleasePack("Sound/BGM1",_currentBgm);
            }

            _currentBgm = clip;
            _bgmSource.clip = clip;
            _bgmSource.volume = volume;
            _bgmSource.Play();
        });
    }

    /// <summary>
    /// 停止背景音乐并释放
    /// </summary>
    public void StopBGM()
    {
        _bgmSource?.Stop();

        if (_currentBgm != null)
        {
            YOTOFramework.resMgr.ReleasePack("Sound/BGM1",_currentBgm);
            _currentBgm = null;
            _bgmSource.clip = null;
        }
    }

    /// <summary>
    /// 播放音效（异步加载并播放一次，播放完自动释放）
    /// </summary>
    public void PlaySFX(string path, float volume = 1f)
    {
        YOTOFramework.resMgr.LoadAudio(path, (clip) =>
        {
            if (clip == null) return;

            AudioSource src = GetFreeSfxSource();
            src.clip = clip;
            src.volume = volume;
            src.Play();

            // 播放完成后自动释放
            YOTOFramework.Instance.StartCoroutine(ReleaseWhenDone(src, clip,path));
        });
    }

    private System.Collections.IEnumerator ReleaseWhenDone(AudioSource src, AudioClip clip,string path)
    {
        yield return new WaitWhile(() => src.isPlaying);
        src.clip = null;
        YOTOFramework.resMgr.ReleasePack(path,clip);
    }

    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAllSFX()
    {
        foreach (var s in _sfxSources)
        {
            if (s.isPlaying) s.Stop();
            if (s.clip != null)
            {
                YOTOFramework.resMgr.ReleasePack("Sound/BGM1",s.clip);
                s.clip = null;
            }
        }
    }

    public void SetBgmVolume(float volume)
    {
        if (_bgmSource != null)
            _bgmSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        foreach (var s in _sfxSources)
        {
            s.volume = volume;
        }
    }

    private AudioSource GetFreeSfxSource()
    {
        foreach (var s in _sfxSources)
        {
            if (!s.isPlaying) return s;
        }
        // 扩展一个
        var newSfx = _bgmSource.gameObject.AddComponent<AudioSource>();
        newSfx.playOnAwake = false;
        _sfxSources.Add(newSfx);
        return newSfx;
    }
}
