﻿using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class LaunchURL : MonoBehaviour
    {
        public string URL;

        public void urlLinkOrWeb()
        {
            Application.OpenURL(URL);
        }

        public void ButtonClicked()
        {
            Debug.Log("button clicked");
        }
    }
}