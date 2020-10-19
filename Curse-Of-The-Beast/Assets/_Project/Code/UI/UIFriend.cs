using System;
using TMPro;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using System.Collections.Generic;

namespace KnoxGameStudios
{
    public class UIFriend : MonoBehaviour
    {
        [SerializeField] private TMP_Text friendNameText;
        [SerializeField] private string friendName;
        [SerializeField] private Image onlineImage;
        [SerializeField] private Color onlineColor;
        [SerializeField] private Color offlineColor;

        public static Action<string> OnRemoveFriend = delegate { };
        public static Action<string> OnInviteFriend = delegate { };
        public static Action<string> OnGetCurrentStatus = delegate { };

        private void Awake()
        {
            PhotonChatController.OnStatusUpdated += HandleStatusUpdated;
            PhotonChatFriendController.OnStatusUpdated += HandleStatusUpdated;
        }
        private void OnDestroy()
        {
            PhotonChatController.OnStatusUpdated -= HandleStatusUpdated;
            PhotonChatFriendController.OnStatusUpdated -= HandleStatusUpdated;
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(friendName)) return;
            OnGetCurrentStatus?.Invoke(friendName);
        }

        public void Initialize(FriendInfo friend)
        {
            Debug.Log($"{friend.UserId} is online: {friend.IsOnline} ; in room: {friend.IsInRoom} ; room name: {friend.Room}");

            SetupUI();
        }
        public void Initialize(string friendName)
        {
            Debug.Log($"{friendName} is added");
            this.friendName = friendName;

            SetupUI();
            OnGetCurrentStatus?.Invoke(friendName);
        }

        private void HandleStatusUpdated(PhotonStatus status)
        {
            if (string.Compare(friendName, status.PlayerName) == 0)
            {
                Debug.Log($"Updating status in UI for {status.PlayerName} to status {status.Status}");
                SetStatus(status.Status);
            }
            else
            {
                Debug.Log($"Good for nothing HandleStatusUpdated {status.PlayerName}");
            }
        }

        private void SetupUI()
        {
            friendNameText.SetText(friendName);
        }

        private void SetStatus(int status)
        {
            if (status == ChatUserStatus.Online)
            {
                onlineImage.color = onlineColor;
            }
            else
            {
                onlineImage.color = offlineColor;
            }
        }

        public void RemoveFriend()
        {
            Debug.Log($"Clicked to remove friend {friendName}");
            OnRemoveFriend?.Invoke(friendName);
        }

        public void InviteFriend()
        {
            Debug.Log($"Clicked to invite friend {friendName}");
            OnInviteFriend?.Invoke(friendName);
        }
    }
}