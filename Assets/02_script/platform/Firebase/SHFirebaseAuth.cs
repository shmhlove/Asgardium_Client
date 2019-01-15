// using UnityEngine;
// using UnityEngine.SocialPlatforms;

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// using Firebase;
// using Firebase.Auth;

// public class SHFirebaseAuth
// {
//     public Action<SHReply> m_pEventChangeAuth;

//     private FirebaseAuth m_pAuth;
//     private FirebaseUser m_pUser;
    
//     public void OnInitialize()
//     {
//         Debug.LogFormat("[LSH] Call is OnInitialize");

//         if (null != m_pAuth)
//             return;

//         m_pAuth = FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance);
//         m_pAuth.StateChanged += OnEventByAuthStateChanged;

//     }

//     public void OnFinalize()
//     {
//         Debug.LogFormat("[LSH] Call is OnFinalize");

//         if (null == m_pAuth)
//             return;

//         m_pAuth.StateChanged -= OnEventByAuthStateChanged;
//     }

//     public void CreateAccount(string strUserEmail, string strUserPassword, Action<SHReply> pCallback)
//     {
//         if (string.IsNullOrEmpty(strUserEmail) || string.IsNullOrEmpty(strUserPassword))
//         {
//             pCallback(new SHReply(new SHError(eErrorCode.FB_CreateAccount_Fail, "Need E-mail and Password")));
//             return;
//         }

//         m_pAuth.CreateUserWithEmailAndPasswordAsync(strUserEmail, strUserPassword).ContinueWith((pTask) =>
//         {
//             if (pTask.IsCanceled)
//             {
//                 pCallback(new SHReply(new SHError(eErrorCode.FB_CreateAccount_Fail, "User Canceled")));
//                 return;
//             }

//             if (pTask.IsFaulted)
//             {
//                 pCallback(new SHReply(new SHError(eErrorCode.FB_CreateAccount_Fail, pTask.Exception.Message)));
//                 return;
//             }

//             m_pUser = pTask.Result;
//             pCallback(new Firebase.Auth.SHReplyCreateAccount(m_pUser));
//         });
//     }

//     public void Login(string strUserEmail, string strUserPassword, Action<SHReply> pCallback)
//     {
//         if (string.IsNullOrEmpty(strUserEmail) || string.IsNullOrEmpty(strUserPassword))
//         {
//             pCallback(new SHReply(new SHError(eErrorCode.FB_Login_Fail, "Need E-mail and Password")));
//             return;
//         }

//         m_pAuth.SignInWithEmailAndPasswordAsync(strUserEmail, strUserPassword).ContinueWith((pTask) =>
//         {
//             if (pTask.IsCanceled)
//             {
//                 pCallback(new SHReply(new SHError(eErrorCode.FB_Login_Fail, "User Canceled")));
//                 return;
//             }

//             if (pTask.IsFaulted)
//             {
//                 pCallback(new SHReply(new SHError(eErrorCode.FB_Login_Fail, pTask.Exception.Message)));
//                 return;
//             }

//             m_pUser = pTask.Result;
//             pCallback(new Firebase.Auth.SHReplyLogin(m_pUser));
//         });
//     }

//     public void GuestLogin(Action<SHReply> pCallback)
//     {
//         // 계전전환
//         // 1. 사용자가 가입하면 해당 사용자가 선택한 인증 제공업체의 로그인 흐름을 진행하되 메소드 호출 전까지만 진행합니다. 
//         //    예를 들어 사용자의 Google ID 토큰, Facebook 액세스 토큰 또는 이메일 주소와 비밀번호를 가져옵니다.
//         // 2. 새로운 인증 제공업체의 을 가져옵니다.
//         // 3. 개체를 로그인 사용자의 메소드에 전달합니다.
//         // * 이 방법은 임의의 계정 2개를 연결할 때도 사용할 수 있습니다.

//         m_pAuth.SignInAnonymouslyAsync().ContinueWith((pTask) =>
//         {
//             if (pTask.IsCanceled)
//             {
//                 pCallback(new SHReply(new SHError(eErrorCode.FB_Guest_Login_Fail, "User Canceled")));
//                 return;
//             }

//             if (pTask.IsFaulted)
//             {
//                 pCallback(new SHReply(new SHError(eErrorCode.FB_Guest_Login_Fail, pTask.Exception.Message)));
//                 return;
//             }

//             m_pUser = pTask.Result;
//             pCallback(new Firebase.Auth.SHReplyGuestLogin(m_pUser));
//         });
//     }

//     public void GoogleSignIn(string strGoogleIdToken, Action<SHReply> pCallback)
//     {
//         if (string.IsNullOrEmpty(strGoogleIdToken))
//         {
//             pCallback(new SHReply(new SHError(eErrorCode.FB_Google_Login_Fail, "Google IdToken is Empty")));
//             return;
//         }

// #if UNITY_ANDROID
//         m_pAuth.SignInWithCredentialAsync(GoogleAuthProvider.GetCredential(strGoogleIdToken, null)).ContinueWith((pTask) =>
//         {
//             if (pTask.IsCanceled)
//             {
//                 pCallback(new SHReply(new SHError(eErrorCode.FB_Google_Login_Fail, "User Canceled")));
//                 return;
//             }

//             if (pTask.IsFaulted)
//             {
//                 pCallback(new SHReply(new SHError(eErrorCode.FB_Google_Login_Fail, pTask.Exception.Message)));
//                 return;
//             }
            
//             m_pUser = pTask.Result;
//             pCallback(new Firebase.Auth.SHReplyGoogleLogin(m_pUser));
//         });
// #else
//         pCallback(new SHReply(new SHError(eErrorCode.FB_Google_Login_Fail, "Not Supported this platform")));
// #endif
//     }

//     public void Logout(Action<SHReply> pCallback)
//     {
//         m_pAuth.SignOut();
//         pCallback(new Firebase.Auth.SHReplyLogout());
//     }
    
//     void OnEventByAuthStateChanged(object sender, EventArgs eventArgs)
//     {
//         if (null == m_pEventChangeAuth)
//             return;

//         m_pEventChangeAuth(new Firebase.Auth.SHReplyAuthChanged(m_pAuth.CurrentUser));
//     }
// }