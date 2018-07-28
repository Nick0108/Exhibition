using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlaying : MonoBehaviour {

    public IGamePlayingState State { get; set; }

    public GamePlaying(IGamePlayingState state)
    {
        State = state;
    }

    public void ChangeState()
    {
        State.ChangeState(this);
    }
}

public interface IGamePlayingState
{
    void ChangeState(GamePlaying gamePlaying);
}
