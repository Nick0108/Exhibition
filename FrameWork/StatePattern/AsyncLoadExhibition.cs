using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncLoadExhibition : MonoBehaviour, IGamePlayingState {

	public void ChangeState(GamePlaying gamePlaying)
    {
        gamePlaying.State = new Exhibiting();
    }
}
