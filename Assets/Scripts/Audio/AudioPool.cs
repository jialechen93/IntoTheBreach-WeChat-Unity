using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 音效对象池 - 避免频繁创建销毁AudioSource
/// 微信小游戏性能优化
/// </summary>
public class AudioPool : MonoBehaviour
{
    public static AudioPool Instance;
    
    [Header("设置")]
    public int poolSize = 10;
    public GameObject audioSourcePrefab;
    
    private Queue<AudioSource> availableSources = new Queue<AudioSource>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 预生成
        if (audioSourcePrefab == null)
        {
            // 如果没给预制件，自己生成
            for (int i = 0; i < poolSize; i++)
            {
                CreateNewSource();
            }
        }
        else
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(audioSourcePrefab, transform);
                AudioSource source = obj.GetComponent<AudioSource>();
                if (source == null) source = obj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                obj.SetActive(false);
                availableSources.Enqueue(source);
            }
        }
    }

    private AudioSource CreateNewSource()
    {
        GameObject obj = new GameObject("PooledAudioSource");
        obj.transform.SetParent(transform);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        obj.SetActive(false);
        availableSources.Enqueue(source);
        return source;
    }

    /// <summary>
    /// 获取一个可用的AudioSource
    /// </summary>
    public AudioSource Get()
    {
        if (availableSources.Count == 0)
        {
            return CreateNewSource();
        }
        
        AudioSource source = availableSources.Dequeue();
        source.gameObject.SetActive(true);
        return source;
    }

    /// <summary>
    /// 播放一个OneShot音效
    /// </summary>
    public void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null) return;
        
        AudioSource source = Get();
        source.clip = clip;
        source.volume = volume;
        source.Play();
        
        // 播放完返还
        StartCoroutine(ReturnAfterPlay(source));
    }

    private System.Collections.IEnumerator ReturnAfterPlay(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length + 0.1f);
        Return(source);
    }

    /// <summary>
    /// 返还AudioSource
    /// </summary>
    public void Return(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        availableSources.Enqueue(source);
    }
}
