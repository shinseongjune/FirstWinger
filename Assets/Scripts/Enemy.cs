using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public enum State
    {
        None = -1, // 사용전
        Ready = 0, // 준비 완료
        Appear,    // 등장
        Battle,    // 전투중
        Dead,      // 사망
        Disappear, // 퇴장
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

    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1;

    float LastBattleUpdateTime = 0.0f;

    [SerializeField]
    int FireRemainCount = 1;

    protected override void UpdateActor()
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

        // 속도 = 거리 / 시간이므로 시간 = 거리 / 속도
        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref CurrentVelocity, distance / CurrentSpeed, MaxSpeed);
    }

    void Arrived()
    {
        if(CurrentState == State.Appear)
        {
            CurrentState = State.Battle;
            LastBattleUpdateTime = Time.time;
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
        if(Time.time - LastBattleUpdateTime > 1.0f)
        {
            if(FireRemainCount > 0)
            {
                Fire();
                FireRemainCount--;
            }
            else
            {
                Disappear(new Vector3(-15.0f, transform.position.y, transform.position.z));
            }

            LastBattleUpdateTime = Time.time;
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

    public void Fire()
    {
        GameObject go = Instantiate(Bullet);

        Bullet bullet = go.GetComponent<Bullet>();
        bullet.Fire(OwnerSide.Enemy, FireTransform.position, -FireTransform.right, BulletSpeed, Damage);
    }
}
