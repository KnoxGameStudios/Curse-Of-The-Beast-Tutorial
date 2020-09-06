using System;
using TMPro;
using Photon.Realtime;
using UnityEngine;

public class UIFriend : MonoBehaviour
{
    [SerializeField] private TMP_Text friendNameText;
    [SerializeField] private FriendInfo friend;

    public static Action<string> OnRemoveFriend = delegate { };

    public void Initialize(FriendInfo friend)
    {
        Debug.Log($"{friend.UserId} is online: {friend.IsOnline} ; in room: {friend.IsInRoom} ; room name: {friend.Room}");
        this.friend = friend;
        friendNameText.SetText(this.friend.UserId);
    }
    public void RemoveFriend()
    {
        Debug.Log($"Clicked to remove friend {friend.UserId}");
        OnRemoveFriend?.Invoke(friend.UserId);
    }
}
