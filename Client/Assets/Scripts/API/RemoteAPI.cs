using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace API
{
    public class RemoteAPI : IAPI
    {
        private string endpoint = "http://localhost/api/v1";

        private IEnumerator GetRequest<T>(string uri, Action<T> callback)
        {
            string url = endpoint + uri;
            Debug.Log("GET " + url);
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                // Show results as text
                Debug.Log(request.downloadHandler.text);
                T res = JsonUtility.FromJson<T>(request.downloadHandler.text);
                callback(res);
            }
        }

        public void GetCity(long id, Action<GetCityResponse> callback)
        {
            GetRequest("/city/" + id, callback);
        }

        public void CreateBuilding(long cityId, int building, int x, int y, Action<CreateBuildingResponse> callback)
        {
            throw new NotImplementedException();
        }

        public void GetPlayer(long playerId, Action<GetPlayerResponse> callback)
        {
            throw new NotImplementedException();
        }

        public void GetCityForPlayer(long playerId, Action<GetCityResponse> callback)
        {
            throw new NotImplementedException();
        }

        public void CollectResources(long currentCityId, int x, int y, Action<CollectResourcesResponse> callback)
        {
            throw new NotImplementedException();
        }

        public void GetStashForTile(long currentCityId, int x, int y, Action<GetStashForTileResponse> callback)
        {
            throw new NotImplementedException();
        }

        public void UpgradeBuilding(long currentCityId, int x, int y, Action<UpgradeResponse> callback)
        {
            throw new NotImplementedException();
        }
    }
}