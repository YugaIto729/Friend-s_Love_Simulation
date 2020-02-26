using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public enum AM_State
    {
        STANDBY, PLAY, FADE, VOLUME, STOP, CONTINUE, SOUND
    }
    public AM_State AM_state = AM_State.STANDBY;

    public AudioClip[] audioClip;

    private TalkEventManager TeManager;
    private AudioSource audioSource_bgm;
    private bool isFade = false;
    private float timeBuff = 0;


    [SerializeField]
    private AudioSource[] audioSources_se;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        TeManager = TalkEventManager.instance;
        audioSource_bgm = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AM_state != AM_State.STANDBY)
        {
            switch (AM_state)
            {
                case AM_State.STANDBY:
                    break;
                case AM_State.PLAY:
                    PlayBGM_Update();
                    break;
                case AM_State.FADE:
                    Fade_Update();
                    break;
                case AM_State.VOLUME:
                    break;
                case AM_State.STOP:
                    Stop_Update();
                    break;
                case AM_State.CONTINUE:
                    Continue_Update();
                    break;
                case AM_State.SOUND:
                    Sound_Update();
                    break;
            }
        }
    }

    private void PlayBGM_Update()
    {
        var eo = TeManager.Get_CullentEvent();

        Play_BGM(eo.register2, eo.register3);

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        AM_state = AM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void Fade_Update()
    {
        var eo = TeManager.Get_CullentEvent();

        if (!isFade)
        {
            StartCoroutine(C_Fade_Update(eo.register2, eo.register3, eo.seconds, eo.register1));
        }

        if (eo.register1 == 1)
        {
            End_Fade();
        }
    }

    private IEnumerator C_Fade_Update(int index, float pitch, float seconds, int mode)
    {
        isFade = true;

        if (seconds > 0)
        {

            for (int i = 0; i <= seconds * 50; i++)
            {
                audioSource_bgm.volume = 1.0f - (i / (seconds * 50));
                yield return new WaitForSeconds(0.01f);
            }

            Play_BGM(index, pitch);

            for (int i = 0; i <= seconds * 50; i++)
            {
                audioSource_bgm.volume = i / (seconds * 50);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            Play_BGM(index, pitch);
            audioSource_bgm.volume = 1;
        }

        if (mode == 0)
        {
            End_Fade();
        }

        isFade = false;
    } 

    private void End_Fade()
    {
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        AM_state = AM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void Stop_Update()
    {
        timeBuff = audioSource_bgm.time;
        audioSource_bgm.Stop();

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        AM_state = AM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void Continue_Update()
    {
        audioSource_bgm.time = timeBuff;
        audioSource_bgm.Play();

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        AM_state = AM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    public void Play_BGM(int index, float pitch)
    {
        AudioClip clip;

        if (audioClip.Length > index)
        {
            clip = audioClip[index];
        }
        else
        {
            clip = audioClip[0];
        }

        audioSource_bgm.time = 0;
        audioSource_bgm.clip = clip;
        audioSource_bgm.pitch = pitch;
        audioSource_bgm.Play();

    }

    public void Change_BGM(int index, float seconds, float pitch)
    {
        if (!isFade)
        {
            StartCoroutine(C_Change_BGM(index, seconds, pitch));
        }
    }

    private IEnumerator C_Change_BGM(int index, float seconds, float pitch)
    {
        isFade = true;

        if (seconds > 0)
        {

            for (int i = 0; i <= seconds * 500; i++)
            {
                audioSource_bgm.volume = 1.0f - (i / (seconds * 500));
                yield return new WaitForSeconds(0.001f);
            }

            Play_BGM(index, pitch);

            for (int i = 0; i <= seconds * 500; i++)
            {
                audioSource_bgm.volume = i / (seconds * 500);
                yield return new WaitForSeconds(0.001f);
            }
        }
        else
        {
            Play_BGM(index, pitch);
            audioSource_bgm.volume = 1;
        }
        isFade = false;
    }

    private bool Check_Playing(AudioSource audio)
    {
        if (!audio.isPlaying)
        {
            return true;
        }
        return false;
    }

    private void Sound_Update()
    {
        var eo = TeManager.Get_CullentEvent();

        SE_Play(eo.register2, eo.register3, eo.register4);

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        AM_state = AM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    public void SE_Play(int index, float pitch, float volume)
    {
        AudioSource audioSource = null;

        //使ってないAudioSourceを探索する
        foreach (var audio in audioSources_se)
        {
            if (!audio.isPlaying)
            {
                audioSource = audio;
            }
        }

        if (audioSource != null)
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(SE_clip(index), volume);
        }
    }


    private AudioClip SE_clip(int index)
    {
        if (audioClip.Length > index)
        {
            return audioClip[index];
        }

        return audioClip[0];
    }
}
