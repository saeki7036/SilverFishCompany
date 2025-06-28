using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    public AudioSource Asource;

    public float PlayCount = 0;
    public string AudioName;
    public bool Dell = false;
    void Update()
    {
        if (PlayCount < 0.2)
        {
            PlayCount += Time.deltaTime;
        }

        if (Asource != null)
        {
            if (!Asource.isPlaying)
            {
                Destroy(gameObject);
            }
            if (Dell)
            {
                Destroy(gameObject);
            }
        }
        else { Debug.Log("�����˂�"); }
    }
    public void isCL_PlaySE(AudioClip Clip)
    {
        //Debug.Log(Clip);
        Asource.clip = Clip;
        Asource.Play();
    }

    public AudioClip GetCurrentClip()
    {
        return Asource.clip; // ���ݍĐ����̃N���b�v��Ԃ�
    }

    public bool IsPlaying()
    {
        return Asource.isPlaying; // �Đ������ǂ������m�F
    }
    public void Stop()
    {
        Asource.Stop();
    }
}
