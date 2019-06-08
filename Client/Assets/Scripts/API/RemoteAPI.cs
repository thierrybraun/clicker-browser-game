using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace API
{
    public class RemoteAPI : API
    {
        private string endpoint = "https://localhost/api/";
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
            Debug.Log("GET " + url);
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
                Debug.Log(request.downloadHandler.text);
                T res = JsonUtility.FromJson<T>(request.downloadHandler.text);
                callback(res);
            }
        }

        public override void GetCity(long id, Action<GetCityResponse> callback)
        {
            StartCoroutine(GetRequest("city/" + id, callback));
        }

        public override void CreateBuilding(long cityId, int building, int x, int y, Action<CreateBuildingResponse> callback)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void GetStashForTile(long currentCityId, int x, int y, Action<GetStashForTileResponse> callback)
        {
            StartCoroutine(GetRequest<GetStashForTileResponse>("city/" + currentCityId + "/stash", callback));
        }

        public override void UpgradeBuilding(long currentCityId, int x, int y, Action<UpgradeResponse> callback)
        {
            throw new NotImplementedException();
        }

        public override void Authenticate(string user, string pass, Action<AuthenticateResponse> callback)
        {
            StartCoroutine(GetRequest<AuthenticateResponse>("login", callback));
        }

        public override void GetMe(Action<GetPlayerResponse> callback)
        {
            StartCoroutine(GetRequest<GetPlayerResponse>("player/me", callback));
        }
    }
}