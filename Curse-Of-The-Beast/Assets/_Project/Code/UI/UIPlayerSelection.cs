using UnityEngine;
using TMPro;
using Photon.Realtime;

namespace KnoxGameStudios
{
    public class UIPlayerSelection : MonoBehaviour
    {
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private Player _owner;

        public Player Owner
        {
            get { return _owner; }
            private set { _owner = value; }
        }

        public void Initialize(Player player)
        {
            Debug.Log($"Player Selection Init {player.NickName}");
            Owner = player;
            SetupPlayerSelection();
        }

        private void SetupPlayerSelection()
        {
            usernameText.SetText(_owner.NickName);
        }
    }
}