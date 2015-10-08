using UnityEngine;
using System.Collections;

using Facebook.Unity;
using Facebook.MiniJSON;
using System.Collections.Generic;

public class FacebookManager : MonoBehaviour {


    void Awake () {   
        if (!FB.IsInitialized) {
            Debug.Log("INITTING");
            FB.Init(InitCallback, OnHideUnity);
        }
    }

    public void FBLogin() {
        if (FB.IsInitialized) {
            FB.LogInWithReadPermissions("public_profile, ", AuthCallback);
        } else {
            Debug.Log("FB NOT INITIALIZED");
        }
    }

    private void InitCallback ()
    {
        if (FB.IsInitialized) {

            if(FB.IsLoggedIn) {
                FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)",HttpMethod.GET, APICallback);
            }

        } else {
            Debug.Log("FB NOT INITIALIZED");
        }
    }

    private void OnHideUnity (bool isGameShown)
    {
        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        } else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    
    private void AuthCallback (ILoginResult result) {
        if (FB.IsLoggedIn) {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions) {
                Debug.Log(perm);
            }

            FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)",HttpMethod.GET, APICallback);

        } else {
            Debug.Log("User cancelled login");
        }
    }

    void APICallback(IGraphResult result)                                                                                              
    {                                                                                                                              
        Debug.Log("APICallBack");                                                                                               
        Debug.Log(result);                                                                       
    }                                                                                                                              
}
