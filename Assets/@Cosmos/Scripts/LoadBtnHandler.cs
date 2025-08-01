using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LoadBtnHandler : MonoBehaviour
{
    private Button continueButton;

    private void Awake()
    {
        continueButton = GetComponent<Button>();
        ActivateButton();
    }




    private void ActivateButton()
    {
        if (SteamRemoteStorage.FileExists("save_session.dat") == true)
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }
}
