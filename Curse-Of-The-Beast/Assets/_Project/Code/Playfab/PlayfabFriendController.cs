using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;
using UnityEngine;

namespace KnoxGameStudios
{
    public class PlayfabFriendController : MonoBehaviour
    {
        public static Action<List<FriendInfo>> OnFriendListUpdated = delegate { };
        private List<FriendInfo> friends;

        #region Unity Methods
        private void Awake()
        {
            friends = new List<FriendInfo>();
            PhotonConnector.GetPhotonFriends += HandleGetFriends;
            UIAddFriend.OnAddFriend += HandleAddPlayfabFriend;
            UIFriend.OnRemoveFriend += HandleRemoveFriend;
        }

        private void OnDestroy()
        {
            PhotonConnector.GetPhotonFriends -= HandleGetFriends;
            UIAddFriend.OnAddFriend -= HandleAddPlayfabFriend;
            UIFriend.OnRemoveFriend -= HandleRemoveFriend;
        }
        #endregion

        #region Private Methods
        private void HandleAddPlayfabFriend(string name)
        {
            Debug.Log($"Playfab add friend request for {name}");
            var request = new AddFriendRequest { FriendTitleDisplayName = name };
            PlayFabClientAPI.AddFriend(request, OnFriendAddedSuccess, OnFailure);
        }
        private void HandleRemoveFriend(string name)
        {
            string id = friends.FirstOrDefault(f => f.TitleDisplayName == name).FriendPlayFabId;
            Debug.Log($"Playfab remove friend {name} with id {id}");
            var request = new RemoveFriendRequest { FriendPlayFabId = id };
            PlayFabClientAPI.RemoveFriend(request, OnFriendRemoveSuccess, OnFailure);
        }

        private void HandleGetFriends()
        {
            GetPlayfabFriends();
        }

        private void GetPlayfabFriends()
        {
            Debug.Log("Playfab get friend list request");
            var request = new GetFriendsListRequest { IncludeSteamFriends = false, IncludeFacebookFriends = false, XboxToken = null };
            PlayFabClientAPI.GetFriendsList(request, OnFriendsListSuccess, OnFailure);
        }
        #endregion

        #region Playfab Call backs
        private void OnFriendAddedSuccess(AddFriendResult result)
        {
            Debug.Log("Playfab add friend success getting updated friend list");
            GetPlayfabFriends();
        }

        private void OnFriendsListSuccess(GetFriendsListResult result)
        {
            Debug.Log($"Playfab get friend list success: {result.Friends.Count}");
            friends = result.Friends;
            OnFriendListUpdated?.Invoke(result.Friends);
        }

        private void OnFriendRemoveSuccess(RemoveFriendResult result)
        {
            Debug.Log($"Playfab remove friend success");
            GetPlayfabFriends();
        }

        private void OnFailure(PlayFabError error)
        {
            Debug.Log($"Playfab Friend Error occured: {error.GenerateErrorReport()}");
        }
        #endregion
    }
}