using System;
using System.Collections.Generic;
using TestTask.NonEditable;
using UnityEngine;

namespace TestTask.Editable
{
    public class ClientColors : MonoBehaviour
    {
        public static event Action<List<Color>> OnColorsReceived;

        private List<Color> receivedColors = new List<Color>();
        private bool isLoggedIn;

        private void Start()
        {
            ClientManager.Instance.ClientLogInStatusChanged += OnLoginStatusChanged;
        }

        private void OnDestroy()
        {
            ClientManager.Instance.ClientLogInStatusChanged -= OnLoginStatusChanged;
        }

        private void OnLoginStatusChanged(int status, int clientId)
        {
            if (status == (int)LoginResponse.Success)
                isLoggedIn = true;
        }

        public void RequestColors()
        {
            if (!isLoggedIn)
            {
                Debug.LogWarning("Cannot request colors before logging in.");
                return;
            }

            ClientPacketsHandler.SendColorRequest();
        }

        public void ReceiveColors(List<Color> colors)
        {
            receivedColors = colors;
            OnColorsReceived?.Invoke(receivedColors);
        }
    }
}
