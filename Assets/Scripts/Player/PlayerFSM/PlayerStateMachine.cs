using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���״̬�����������״̬�ĳ�ʼ�����л�״̬
/// </summary>
public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    public void Initialize(PlayerState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    /// <summary>
    /// ����״̬�л���ÿ���л����Ƿ�Ӧ���������أ��ضϵ�ǰ״̬�ĺ���update��
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(PlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
