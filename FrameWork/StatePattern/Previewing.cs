using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Previewing : MonoBehaviour,IGamePlayingState {

    public Previewing()
    {
        
    }

    public void ChangeState(GamePlaying gamePlaying)
    {
        //具体行为
        gamePlaying.State = new AsyncLoadExhibition();
    }

}
