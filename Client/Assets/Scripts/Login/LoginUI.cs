using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using API;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Login
{
    public class LoginUI : MonoBehaviour
    {
        public string Version = "indev";
        public Text ClientVersion, ApiVersion;
        public InputField Username, Password;
        public InputField RegisterUsername, RegisterPassword, RegisterPassword2;
        public GameObject LoginPanel, RegistrationPanel;
        public ToggleGroup PanelSelectionGroup;
        public Text Error, RegisterError;
        public GameObject ApiContainer;
        public Button RegisterButton;
        public Text RegistrationStatus;

        private void Awake()
        {
            Assert.IsNotNull(Username);
            Assert.IsNotNull(Password);
            Assert.IsNotNull(Error);
            Assert.IsNotNull(ApiContainer);

            Error.text = "";
            RegisterError.text = "";
            RegistrationStatus.text = "";
            SetAPIType(0);
        }

        public void Login()
        {
            var user = Username.text;
            var pass = Password.text;
            var credentials = System.Text.Encoding.UTF8.GetBytes(Username.text + ":" + Password.text);

            var api = ApiContainer.GetComponent<API.API>();
            api.Credentials = System.Convert.ToBase64String(credentials);
            api.Authenticate(user, pass, LoginRespose);
        }

        private void LoginRespose(AuthenticateResponse res)
        {
            if (res.Success)
            {
                SceneManager.LoadScene("City");
            }
            else
            {
                Error.text = res.Error;
            }
        }

        public void SetAPIType(int type)
        {
            var api = ApiContainer.GetComponent<API.API>();
            if (type == 0)
            {
                if (!(api is RemoteAPI))
                {
                    DestroyImmediate(api);
                    ApiContainer.AddComponent<RemoteAPI>();
                    Username.text = "test0";
                    Password.text = "test0";
                }
            }
            else
            {
                if (!(api is DummyAPI))
                {
                    DestroyImmediate(api);
                    ApiContainer.AddComponent<DummyAPI>();
                    Username.text = "Player1";
                    Password.text = "Player1";
                }
            }
            ClientVersion.text = $"Client version: {Version}";
            ApiContainer.GetComponent<API.API>().GetVersion(res => ApiVersion.text = $"API version: {res.Version}");
        }

        public void Register()
        {
            RegisterButton.interactable = false;
            RegistrationStatus.text = "Creating world...";
            RegisterError.text = "";
            var user = RegisterUsername.text;
            var pass = RegisterPassword.text;
            if (pass != RegisterPassword2.text)
            {
                RegisterButton.interactable = true;
                RegistrationStatus.text = "";
                RegisterError.text = "Passwords don't match";
                return;
            }
            var credentials = System.Text.Encoding.UTF8.GetBytes(user + ":" + pass);

            var api = ApiContainer.GetComponent<API.API>();
            api.Credentials = System.Convert.ToBase64String(credentials);
            api.Register(user, pass, RegisterResponse);
        }

        private void RegisterResponse(RegistrationResponse res)
        {
            RegistrationStatus.text = "";
            if (res.Success)
            {
                Username.text = RegisterUsername.text;
                Password.text = RegisterPassword.text;
                RegistrationStatus.text = "Loading world";
                Login();
            }
            else
            {
                RegisterButton.interactable = true;
                RegisterError.text = res.Error;
            }
        }

        public void ChangePanel()
        {
            var active = PanelSelectionGroup.ActiveToggles().First();
            if (active.name == "Register")
            {
                LoginPanel.SetActive(false);
                RegistrationPanel.SetActive(true);
            }
            else
            {
                LoginPanel.SetActive(true);
                RegistrationPanel.SetActive(false);
            }
        }
    }
}