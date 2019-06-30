using System.Collections;
using System.Collections.Generic;
using API;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteAccount : MonoBehaviour
{

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        API.API.Instance.DeleteAccount(OnDelete);
    }

    private void OnDelete(DeleteAccountResponse res)
    {
        if (res.Success)
        {
            SceneManager.LoadScene("Login");
        }
        else
        {
            Debug.LogError(res.Error);
        }
    }

}
