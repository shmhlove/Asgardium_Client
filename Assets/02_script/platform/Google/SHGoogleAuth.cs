// using UnityEngine;
// using UnityEngine.SocialPlatforms;

// using System;
// using System.Collections;
// using System.Collections.Generic;

// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using GooglePlayGames.BasicApi.SavedGame;

// public class SHGoogleAuth
// {
//     public void OnInitialize()
//     {
//         Debug.LogFormat("[LSH] Call is OnInitialize");
        
// #if UNITY_ANDROID
//         var pGPGConfig = new PlayGamesClientConfiguration.Builder()
//             .RequestIdToken()
//             .RequestEmail()
//             .EnableSavedGames()
//             .Build();
//         PlayGamesPlatform.InitializeInstance(pGPGConfig);
//         PlayGamesPlatform.DebugLogEnabled = true;
//         PlayGamesPlatform.Activate();
// #endif
//     }

//     public void OnFinalize()
//     {
//         Debug.LogFormat("[LSH] Call is OnFinalize");
//     }
    
//     public void Login(Action<SHReply> pCallback)
//     {
//         if (true == IsLogin())
//         {
//             pCallback(new SHReply(new SHError(eErrorCode.Google_Login_Fail, "Already Logined")));
//             return;
//         }

// #if UNITY_ANDROID && !UNITY_EDITOR
//         PlayGamesPlatform.Instance.Authenticate((isSucceed, strErrorMessage) =>
//         {
//             if (isSucceed)
//             {
//                 pCallback(new Google.Auth.SHReplyLogin(
//                     GetUserID(), GetUserName(), GetUserState(), GetUserEmail(), GetIdToken()));
//             }
//             else
//             {
//                 pCallback(new SHReply(new SHError(eErrorCode.Google_Login_Fail, strErrorMessage)));
//             }
//         });
// #else
//         pCallback(new SHReply(new SHError(eErrorCode.Google_Login_Fail, "Not Supported this platform")));
// #endif
//     }

//     public void Logout(Action<SHReply> pCallback)
//     {
// #if UNITY_ANDROID && !UNITY_EDITOR
//         ((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
// #endif
//         pCallback(new Google.Auth.SHReplyLogout());
//     }

//     public bool IsLogin()
//     {
//         return Social.localUser.authenticated;
//     }

//     public string GetUserID()
//     {
//         return Social.localUser.id;
//     }
    
//     public string GetUserName()
//     {
//         return Social.localUser.userName;
//     }

//     public string GetUserEmail()
//     {
// #if UNITY_ANDROID
//         return PlayGamesPlatform.Instance.GetUserEmail();
// #else
//         return string.Empty;
// #endif
//     }

//     public UserState GetUserState()
//     {
//         return Social.localUser.state;
//     }

//     public Texture2D GetProfileImage()
//     {
//         return Social.localUser.image;
//     }

//     public string GetIdToken()
//     {
// #if UNITY_ANDROID
//         return PlayGamesPlatform.Instance.GetIdToken();
// #else
//         return string.Empty;
// #endif
//     }
// }