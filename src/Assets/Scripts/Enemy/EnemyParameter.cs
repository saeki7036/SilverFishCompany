using UnityEngine;

[System.Serializable]
public class EnemyParameter
{
    [SerializeField]
    int hp;

    [SerializeField]
    int atk;

    [SerializeField]
    float Speed;

    [SerializeField]
    int Intarval;

    int MAXHP;

    public void ParameterSetUP()
    {
        MAXHP = hp;
    }

    public void Damege(int damege) => hp -= damege;

    public bool Death() => hp <= 0;

    public float HPrate() => Mathf.Clamp01((float)hp / MAXHP);

    public float SPEED => Speed;

    public int ATK => atk;

    public int INTERVAL => Intarval;
}
