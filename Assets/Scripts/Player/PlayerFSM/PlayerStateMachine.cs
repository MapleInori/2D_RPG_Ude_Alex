using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家状态机，用于玩家状态的初始化和切换状态
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
    /// 用于状态切换，每次切换后是否应该立即返回，截断当前状态的后续update？
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(PlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
