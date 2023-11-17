using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionPopup : UI_Popup
{
    enum Buttons 
    {
        ExitBtn,
    }
    enum Sliders
    {
        BGMSlider,
        SFXSlider,

    }
    enum Texts 
    { 

    }



    // Start is called before the first frame update
    void Start()
    {
        Init();
    }


    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        BindEventBtn();

        Get<Slider>((int)Sliders.BGMSlider).value = GameManager.Sound.BGMVolume;
        Get<Slider>((int)Sliders.SFXSlider).value = GameManager.Sound.SFXVolume;

        Get<Slider>((int)Sliders.BGMSlider).onValueChanged.AddListener(delegate { VolumeChange(Define.Sounds.BGM); });
        Get<Slider>((int)Sliders.SFXSlider).onValueChanged.AddListener(delegate { VolumeChange(Define.Sounds.SFX); });

    }

    #region Btn 
    void BindEventBtn()
    {
        BindEvent(GetButton((int)Buttons.ExitBtn).gameObject, Btn_Exit);

    }

    void Btn_Exit(PointerEventData evt)
    {
        GameManager.UI.ClosePopupUI();
    }
    #endregion Btn 

    #region Slider
    void VolumeChange(Define.Sounds Sound)
    {
        float volume;
        if (Sound == Define.Sounds.BGM)
        {
            volume = Get<Slider>((int)Sliders.BGMSlider).value;
            GameManager.Sound.BGMVolume = volume;

        }
        else
        {
            volume = Get<Slider>((int)Sliders.SFXSlider).value;
            GameManager.Sound.SFXVolume = volume;

        }

        GameManager.Sound.SetVolume(Sound, volume);

    }

    #endregion Slider

    
}
