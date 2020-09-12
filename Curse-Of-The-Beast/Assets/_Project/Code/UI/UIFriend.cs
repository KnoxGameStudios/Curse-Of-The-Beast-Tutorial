using System;
using TMPro;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace KnoxGameStudios
{
    public class UIFriend : MonoBehaviour
    {
        [SerializeField] private TMP_Text friendNameText;
        [SerializeField] private FriendInfo friend;
        [SerializeField] private Image onlineImage;
        [SerializeField] private Color onlineColor;
        [SerializeField] private Color offlineColor;

        public static Action<string> OnRemoveFriend = delegate { };

        public void Initialize(FriendInfo friend)
        {
            Debug.Log($"{friend.UserId} is online: {friend.IsOnline} ; in room: {friend.IsInRoom} ; room name: {friend.Room}");
            this.friend = friend;

            SetupUI();
        }

        private void SetupUI()
        {
            friendNameText.SetText(friend.UserId);

            if (friend.IsOnline)
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
            Debug.Log($"Clicked to remove friend {friend.UserId}");
            OnRemoveFriend?.Invoke(friend.UserId);
        }
    }
}