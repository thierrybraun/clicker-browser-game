using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using API;
using System;
using UnityEngine.SceneManagement;

namespace Login
{
    public class LoginUI : MonoBehaviour
    {
        public InputField Username, Password;
        public Text Error;
        public GameObject ApiContainer;

        private void Awake()
        {
            Assert.IsNotNull(Username);
            Assert.IsNotNull(Password);
            Assert.IsNotNull(Error);
            Assert.IsNotNull(ApiContainer);

            Error.text = "";
            ApiContainer.AddComponent<RemoteAPI>();
            Username.text = "test0";
            Password.text = "test0";
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
                    Username.text = "";
                }
            }
            else
            {
                if (!(api is DummyAPI))
                {
                    DestroyImmediate(api);
                    ApiContainer.AddComponent<DummyAPI>();
                    Username.text = "Player1";
                }
            }
        }
    }
}