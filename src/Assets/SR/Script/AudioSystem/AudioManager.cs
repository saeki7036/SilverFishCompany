using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] GameObject AudioPlayObj;
    [SerializeField] AudioClip BGM;
    AudioSource BgmSource;
    AudioPlay audioPlay;

    public List<AudioPlay> audioPlays = new List<AudioPlay>();

    public Dictionary<AudioClip, float> lastPlayTimes = new Dictionary<AudioClip, float>();
    public float minInterval = 0.1f; // ���ʉ����Đ�����Ԋu�i�b�j

    void Start()
    {
        BgmSource = GetComponent<AudioSource>();
        if (BGM != null)
        {
            BgmSource.clip = BGM;
            BgmSource.Play();
        }

        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void isPlaySE(AudioClip Clip)
    {
        /*
        foreach (var audios in audioPlays)
        {
            if (audios != null)
            {
                if (audios.GetCurrentClip() == Clip && audios.IsPlaying())
                {
                    Debug.Log("sound");
                    return; // �����N���b�v���Đ����Ȃ�V�K�Đ��𒆒f
                }
            }
        }*/
        if (lastPlayTimes.ContainsKey(Clip))
        {
            float lastPlayTime = lastPlayTimes[Clip];
            if (Time.time - lastPlayTime < minInterval)
            {
                //Debug.Log("�Đ��Ԋu���F�X�L�b�v");
                return; // �Œ�Ԋu�𖞂����Ă��Ȃ��ꍇ�A�Đ����Ȃ�
            }
        }

        GameObject CL_AudioPlay = Instantiate(AudioPlayObj);
        AudioPlay audio = CL_AudioPlay.GetComponent<AudioPlay>();

        audio.isCL_PlaySE(Clip);
        CL_AudioPlay.SetActive(true);

        audioPlays.Add(audio);

        // �Đ����Ԃ��L�^
        lastPlayTimes[Clip] = Time.time;

    }

    public void StopAllAudio()
    {
        // ���ׂĂ̍Đ����̉����~����
        foreach (var audio in audioPlays)
        {
            audio.Stop(); // AudioPlay�N���X��Stop���\�b�h������Ɖ���
        }
        audioPlays.Clear(); // ���X�g���N���A���čė��p�\��
    }
}
