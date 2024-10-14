using UnityEngine;
using Cysharp.Threading.Tasks;

public class SoundManager : Singleton<SoundManager>, IDonDestroy
{
    #region 
    private GameObject sfx;
    private GameObject bgm;
    private AudioSource sfxSource;
    private AudioSource bgmSource;
    GameObject audioParent = null;
    public Transform AudioParent
    {
        get
        {
            if (audioParent == null)
            {
                audioParent = GameObject.Find("[Audio]");
                if (audioParent == null)
                {
                    audioParent = new GameObject("[Audio]");
                    audioParent.transform.SetParent(GameParent);
                }
            }
            return audioParent.transform;
        }
    }

    GameObject gameParent = null;
    public Transform GameParent
    {
        get
        {
            if (gameParent == null)
            {
                gameParent = GameObject.Find("[GameObject]");
                if (gameParent == null)
                {
                    gameParent = new GameObject("[GameObject]");
                }
            }
            return gameParent.transform;
        }
    }
    GameObject ObjSFX
    {
        get
        {
            if (sfx == null)
            {
                sfx = new GameObject("SFX");
                sfx.transform.SetParent(AudioParent);
                sfx.AddComponent<AudioSource>();
            }

            return sfx;
        }
    }
    GameObject ObjBGM
    {
        get
        {
            if (bgm == null)
            {
                bgm = new GameObject("BGM");
                bgm.transform.SetParent(AudioParent);
                bgm.AddComponent<AudioSource>();
            }

            return bgm;
        }
    }
    AudioSource SFX
    {
        get
        {
            if (sfxSource == null)
            {
                sfxSource = ObjSFX.GetComponent<AudioSource>();
            }

            return sfxSource;
        }
    }
    AudioSource BGM
    {
        get
        {
            if (bgmSource == null)
            {
                bgmSource = ObjBGM.GetComponent<AudioSource>();
            }

            return bgmSource;
        }
    }
    #endregion


    public async UniTask PlaySFX(AudioClip clip, float customVolume = 1f, bool useAwait = false)
    {
        SFX.clip = clip;
        SFX.loop = false;
        SFX.volume = customVolume;
        SFX.dopplerLevel = 0;
        SFX.reverbZoneMix = 0;
        SFX.Play();

        if (useAwait)
        {
            await UniTask.Delay((int)(clip.length * 1000));
        }
    }

    public void PlayBGM(AudioClip clip, float customVolume = 0.3f)
    {
        BGM.clip = clip;
        BGM.loop = true;
        BGM.volume = customVolume;
        BGM.dopplerLevel = 0;
        BGM.reverbZoneMix = 0;
        BGM.time = 0;
        BGM.Play();
    }

    public void PlayBGM()
    {
        if (BGM.clip != null && !BGM.isPlaying) BGM.Play();
    }

    public void PauseBGM()
    {
        if (BGM.clip != null && BGM.isPlaying) BGM.Pause();
    }
}