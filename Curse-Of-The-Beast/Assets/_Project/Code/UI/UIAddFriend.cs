using System;
using UnityEngine;

namespace KnoxGameStudios
{
    public class UIAddFriend : MonoBehaviour
    {
        [SerializeField] private string displayName;

        public static Action<string> OnAddFriend = delegate { };

        public void SetAddFriendName(string name)
        {
            displayName = name;
        }
        public void AddFriend()
        {
            Debug.Log($"UI Add Friend Clicked: {displayName}");
            if (string.IsNullOrEmpty(displayName)) return;
            OnAddFriend?.Invoke(displayName);
        }
    }
}