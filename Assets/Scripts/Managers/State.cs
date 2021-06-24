using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplePlatformer.Enemy;

public class State : MonoBehaviour
{
    protected FiniteStateMachine stateMachine;
    protected Enemy enemy;

    protected float startTime;

    protected string animName;

    public State(Enemy _enemy, FiniteStateMachine _stateMachine, string _animName)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
        animName = _animName;
    }

    public virtual void Enter()
    {
        startTime = Time.time;
        enemy.anim.Play(animName);
    }

    public virtual void Exit()
    {
        //NOTHING
    }

    public virtual void LogicUpdate()
    {
        //NOTHING
    }

    public virtual void PhyisicsUpdate()
    {
        //NOTHING
    }
}
