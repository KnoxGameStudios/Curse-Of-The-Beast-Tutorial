using System;
using System.Collections.Generic;
using UnityEngine;
using PlayfabFriendInfo = PlayFab.ClientModels.FriendInfo;
using System.Linq;
using Photon.Chat;

namespace KnoxGameStudios
{
    public class PhotonChatFriendController : MonoBehaviour
    {        
        [SerializeField] private bool initialized;
        [SerializeField] private List<string> friendList;
        [SerializeField] private List<string> friendStatusesTest;
        private ChatClient chatClient;
        public static Dictionary<string, PhotonStatus> friendStatuses;
        
        public static Action<List<string>> OnDisplayFriends = delegate { };
        public static Action<PhotonStatus> OnStatusUpdated = delegate { };

        private void Awake()
        {
            friendList = new List<string>();
            friendStatusesTest = new List<string>();
            friendStatuses = new Dictionary<string, PhotonStatus>();
            PlayfabFriendController.OnFriendListUpdated += HandleFriendsUpdated;
            PhotonChatController.OnChatConnected += HandleChatConnected;
            PhotonChatController.OnStatusUpdated += HandleStatusUpdated;
            UIFriend.OnGetCurrentStatus += HandleGetCurrentStatus;
        }

        private void OnDestroy()
        {
            PlayfabFriendController.OnFriendListUpdated -= HandleFriendsUpdated;
            PhotonChatController.OnChatConnected -= HandleChatConnected;
            PhotonChatController.OnStatusUpdated -= HandleStatusUpdated;
            UIFriend.OnGetCurrentStatus -= HandleGetCurrentStatus;
        }

        private void HandleFriendsUpdated(List<PlayfabFriendInfo> friends)
        {
            friendList = friends.Select(f => f.TitleDisplayName).ToList();
            RemovePhotonFriends();
            FindPhotonFriends();
        }

        private void HandleChatConnected(ChatClient client)
        {
            chatClient = client;
            RemovePhotonFriends();
            FindPhotonFriends();
        }

        private void HandleStatusUpdated(PhotonStatus status)
        {
            if(friendStatuses.ContainsKey(status.PlayerName))
            {
                friendStatuses[status.PlayerName] = status;
            }
            else
            {
                friendStatuses.Add(status.PlayerName, status);
            }
            friendStatusesTest.Add($"{status.PlayerName}:{status.Status}");
            foreach (KeyValuePair<string, PhotonStatus> currentStatus in friendStatuses)
            {
                Debug.Log($"asdfasfdasfdasd...asdf.as.fd{currentStatus.Value.PlayerName} changed to {currentStatus.Value.Status} with message {currentStatus.Value.Message}");
            }
        }

        private void HandleGetCurrentStatus(string name)
        {
            PhotonStatus status;
            if (friendStatuses.ContainsKey(name))
            {
                status = friendStatuses[name];
            }
            else
            {
                status = new PhotonStatus(name, 0, "");
            }
            OnStatusUpdated?.Invoke(status);
        }

        private void RemovePhotonFriends()
        {            
            if(friendList.Count > 0 && initialized)
            {
                string[] friendDisplayNames = friendList.ToArray();
                chatClient.RemoveFriends(friendDisplayNames);
            }
        }
        private void FindPhotonFriends()
        {
            if (chatClient == null) return;
            if (friendList.Count != 0)
            {
                initialized = true;
                string[] friendDisplayNames = friendList.ToArray();                
                chatClient.AddFriends(friendDisplayNames);                
            }
            OnDisplayFriends?.Invoke(friendList);
        }
    }
}