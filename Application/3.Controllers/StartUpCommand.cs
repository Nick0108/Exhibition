using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
class StartUpCommand : Controller
{
    
    public override void Execute(object data)
    {
        GameModel gameModel = new GameModel();
        //注册模型 Model
        RegisterModel(gameModel);
        //注册命令 Controller

        RegisterController(Consts.E_EnterScene, typeof(EnterSceneCommand));
        RegisterController(Consts.E_ExitScene, typeof(ExitSceneCommand));
        RegisterController(Consts.E_RegisterView, typeof(RegisterViewCommand));
        //RegisterView(Game.Instance.GetComponent<UIManager>());
        //RegisterView(Game.Instance.GetComponent<Spawner>());
        GamePlay.Instance.gameModel = gameModel;
        gameModel.Init();
       
    }
    
}
