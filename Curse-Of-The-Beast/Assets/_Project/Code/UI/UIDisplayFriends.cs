using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class UIDisplayFriends : MonoBehaviour
{
    [SerializeField] private Transform friendContainer;
    [SerializeField] private UIFriend uiFriendPrefab;
    private void Awake()
    {
        PhotonFriendController.OnDisplayFriends += HandleDisplayFriends;
    }
    private void OnDestroy()
    {
        PhotonFriendController.OnDisplayFriends -= HandleDisplayFriends;
    }

    private void HandleDisplayFriends(List<FriendInfo> friends)
    {
        Debug.Log("UI remove prior friends displayed");
        foreach (Transform child in friendContainer)
        {
            Destroy(child.gameObject);
        }
        Debug.Log($"UI instantiate friends display {friends.Count}");
        foreach (FriendInfo friend in friends)
        {
            UIFriend uifriend = Instantiate(uiFriendPrefab, friendContainer);
            uifriend.Initialize(friend);
        }
    }
}
