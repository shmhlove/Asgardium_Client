// using UnityEngine;
// using UnityEngine.SocialPlatforms;

// using System;
// using System.Collections;
// using System.Collections.Generic;

// public class SHAppleAuth
// {
//     public void OnInitialize()
//     {
//         Debug.LogFormat("[LSH] Call is OnInitialize");
//     }

//     public void OnFinalize()
//     {
//         Debug.LogFormat("[LSH] Call is OnFinalize");
//     }
    
//     public void Login(Action<SHReply> pCallback)
//     {
//         if (true == IsLogin())
//         {
//             pCallback(new SHReply(new SHError(eErrorCode.Apple_Login_Fail, "Already Logined")));
//             return;
//         }

// #if UNITY_IOS && !UNITY_EDITOR
//         Social.localUser.Authenticate((isSucceed, strErrorMessage) =>
//         {
//             if (isSucceed)
//             {
//                 pCallback(new Apple.Auth.SHReplyLogin(
//                     GetUserID(), GetUserName(), GetUserState()));
//             }
//             else
//             {
//                 pCallback(new SHReply(new SHError(eErrorCode.Apple_Login_Fail, strErrorMessage)));
//             }
//         }); 
// #else
//         pCallback(new SHReply(new SHError(eErrorCode.Apple_Login_Fail, "Not Supported this platform")));
// #endif
//     }

//     public void Logout(Action<SHReply> pCallback)
//     {
// #if UNITY_IOS && !UNITY_EDITOR
        
// #endif
//         pCallback(new Apple.Auth.SHReplyLogout());
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

//     public UserState GetUserState()
//     {
//         return Social.localUser.state;
//     }

//     public Texture2D GetProfileImage()
//     {
//         return Social.localUser.image;
//     }
// }