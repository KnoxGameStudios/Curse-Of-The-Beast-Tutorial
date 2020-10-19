using Photon.Pun;
using Photon.Realtime;
using PlayfabFriendInfo = PlayFab.ClientModels.FriendInfo;
using PhotonFriendInfo = Photon.Realtime.FriendInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace KnoxGameStudios
{
    public class PhotonFriendController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private float refreshCooldown;
        [SerializeField] private float refreshCountdown;
        [SerializeField] private List<PlayfabFriendInfo> friendList;
        public static Action<List<PhotonFriendInfo>> OnDisplayFriends = delegate { };

        private void Awake()
        {
            friendList = new List<PlayfabFriendInfo>();
            //PlayfabFriendController.OnFriendListUpdated += HandleFriendsUpdated;
        }

        private void OnDestroy()
        {
            //PlayfabFriendController.OnFriendListUpdated -= HandleFriendsUpdated;
        }

        private void Update()
        {
            if (refreshCountdown > 0)
            {
                refreshCountdown -= Time.deltaTime;
            }
            else
            {
                refreshCountdown = refreshCooldown;
                if (PhotonNetwork.InRoom) return;
                FindPhotonFriends(friendList);
            }
        }

        private void HandleFriendsUpdated(List<PlayfabFriendInfo> friends)
        {
            friendList = friends;
            FindPhotonFriends(friendList);
        }

        private static void FindPhotonFriends(List<PlayfabFriendInfo> friends)
        {
            Debug.Log($"Handle getting Photon friends {friends.Count}");
            if (friends.Count != 0)
            {
                string[] friendDisplayNames = friends.Select(f => f.TitleDisplayName).ToArray();
                PhotonNetwork.FindFriends(friendDisplayNames);
            }
            else
            {
                List<PhotonFriendInfo> friendList = new List<PhotonFriendInfo>();
                OnDisplayFriends?.Invoke(friendList);
            }
        }

        public override void OnFriendListUpdate(List<PhotonFriendInfo> friendList)
        {
            Debug.Log($"Invoke UI to display Photon friends found: {friendList.Count}");
            OnDisplayFriends?.Invoke(friendList);
        }
    }
}