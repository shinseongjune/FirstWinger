using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        None = -1, // �����
        Ready = 0, // �غ� �Ϸ�
        Appear,    // ����
        Battle,    // ������
        Dead,      // ���
        Disappear, // ����
    }

    [SerializeField]
    State CurrentState = State.None;

    const float MaxSpeed = 10.0f;

    const float MaxSpeedTime = 0.5f;

    [SerializeField]
    Vector3 TargetPosition;

    [SerializeField]
    float CurrentSpeed;

    Vector3 CurrentVelocity;

    float MoveStartTime = 0.0f;

    float BattleStartTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case State.None:
            case State.Ready:
                break;
            case State.Dead:
                break;
            case State.Appear:
            case State.Disappear:
                UpdateSpeed();
                UpdateMove();
                break;
            case State.Battle:
                UpdateBattle();
                break;
        }
    }

    void UpdateSpeed()
    {
        CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, (Time.time - MoveStartTime)/MaxSpeedTime);
    }

    void UpdateMove()
    {
        float distance = Vector3.Distance(TargetPosition, transform.position);
        if (distance == 0)
        {
            Arrived();
            return;
        }

        CurrentVelocity = (TargetPosition - transform.position).normalized * CurrentSpeed;

        // �ӵ� = �Ÿ� / �ð��̹Ƿ� �ð� = �Ÿ� / �ӵ�
        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref CurrentVelocity, distance / CurrentSpeed, MaxSpeed);
    }

    void Arrived()
    {
        if(CurrentState == State.Appear)
        {
            CurrentState = State.Battle;
            BattleStartTime = Time.time;
        }
        else // if (CurrentState == State.Disappear)
        {
            CurrentState = State.None;
        }
    }

    public void Appear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = MaxSpeed;

        CurrentState = State.Appear;
        MoveStartTime = Time.time;
    }

    void Disappear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = 0;

        CurrentState = State.Disappear;
        MoveStartTime = Time.time;
    }

    void UpdateBattle()
    {
        if(Time.time - BattleStartTime > 3.0f)
        {
            Disappear(new Vector3(-15.0f, transform.position.y, transform.position.z));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player)
            player.OnCrash(this);
    }

    public void OnCrash(Player player)
    {

    }
}
