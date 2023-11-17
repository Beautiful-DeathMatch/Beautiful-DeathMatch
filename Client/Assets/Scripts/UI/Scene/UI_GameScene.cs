
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using Client;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        joystickBG,
        joystickHandle,
    }
    enum Buttons
    {
        AttackBtn,
        SkillBtn,
        ItemBtn,
    }
    enum Texts
    {
        MoneyTxt,
        ScoreTxt
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        ButtonBind();



    }



    #region Buttons
    void ButtonBind()
    {
        //BindEvent(GetButton((int)Buttons.AttackBtn).gameObject, Btn_Attack);
    }


    #endregion Buttons


}
