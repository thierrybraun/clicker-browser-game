using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;

namespace API
{
    public class RemoteAPI : API
    {
        private string endpoint = "https://localhost/dev/api/";
        private string credentials;

        public override string Credentials { set => credentials = value; }

        ///<summary>CertificateHandler that always validates true for debugging purposes</summary>
        private class SnakeOilCertificateHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        };

        protected override void Awake()
        {
            base.Awake();
            var url = Application.absoluteURL;
            if (url.Trim().Length > 0)
            {
                if (!url.EndsWith("/")) url = url + "/";
                endpoint = url + "api/";
            }
            Debug.Log("API endpoint: " + endpoint);
        }

        private IEnumerator GetRequest<T>(string uri, Action<T> callback)
        {
            string url = endpoint + uri;
            // Debug.Log("GET " + url);
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.certificateHandler = new SnakeOilCertificateHandler();
            request.SetRequestHeader("Authorization", "Basic " + credentials);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                try
                {
                    T res = JsonUtility.FromJson<T>(request.downloadHandler.text);
                    callback(res);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    Debug.LogError(request.downloadHandler.text);
                }
            }
        }
        private IEnumerator PostRequest<T>(string uri, WWWForm data, Action<T> callback)
        {
            string url = endpoint + uri;
            // Debug.Log("Post " + url);

            UnityWebRequest request = UnityWebRequest.Post(url, data);
            request.certificateHandler = new SnakeOilCertificateHandler();
            request.SetRequestHeader("Authorization", "Basic " + credentials);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                try
                {
                    T res = JsonUtility.FromJson<T>(request.downloadHandler.text);
                    callback(res);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    Debug.LogError(request.downloadHandler.text);
                }
            }
        }

        public override void GetCity(long id, Action<GetCityResponse> callback)
        {
            StartCoroutine(GetRequest("city/" + id, callback));
        }

        public override void CreateBuilding(long cityId, int building, int x, int y, Action<CreateBuildingResponse> callback)
        {
            WWWForm form = new WWWForm();
            form.AddField("building", building);
            form.AddField("x", x);
            form.AddField("y", y);
            StartCoroutine(PostRequest<CreateBuildingResponse>("city/" + cityId + "/building", form, callback));
        }

        public override void GetPlayer(long playerId, Action<GetPlayerResponse> callback)
        {
            StartCoroutine(GetRequest<GetPlayerResponse>("player/" + playerId, callback));
        }

        public override void GetCityForPlayer(long playerId, Action<GetCityResponse> callback)
        {
            StartCoroutine(GetRequest<GetCityResponse>("player/" + playerId + "/city", callback));
        }

        public override void CollectResources(long currentCityId, int x, int y, Action<CollectResourcesResponse> callback)
        {
            StartCoroutine(PostRequest<CollectResourcesResponse>($"city/{currentCityId}/collect/{x}/{y}", new WWWForm(), callback));
        }

        public override void GetStashForTile(long currentCityId, int x, int y, Action<GetStashForTileResponse> callback)
        {
            StartCoroutine(GetRequest<GetStashForTileResponse>($"city/{currentCityId}/stash/{x}/{y}", callback));
        }

        public override void UpgradeBuilding(long currentCityId, int x, int y, int targetLevel, Action<UpgradeResponse> callback)
        {
            WWWForm form = new WWWForm();
            form.AddField("x", x);
            form.AddField("y", y);
            form.AddField("targetLevel", targetLevel);
            StartCoroutine(PostRequest<UpgradeResponse>("city/" + currentCityId + "/upgrade", form, callback));
        }

        public override void Authenticate(string user, string pass, Action<AuthenticateResponse> callback)
        {
            StartCoroutine(GetRequest<AuthenticateResponse>("login", callback));
        }

        public override void GetMe(Action<GetPlayerResponse> callback)
        {
            StartCoroutine(GetRequest<GetPlayerResponse>("player/me", callback));
        }

        public override void GetVersion(Action<GetVersionResponse> callback)
        {
            StartCoroutine(GetRequest<GetVersionResponse>("version", callback));
        }

        public override void Register(string user, string pass, Action<RegistrationResponse> callback)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", user);
            form.AddField("password", pass);
            StartCoroutine(PostRequest<RegistrationResponse>("register", form, callback));
        }

        public override void DeleteAccount(Action<DeleteAccountResponse> callback)
        {
            StartCoroutine(PostRequest<DeleteAccountResponse>("deleteaccount", new WWWForm(), callback));
        }
    }
}