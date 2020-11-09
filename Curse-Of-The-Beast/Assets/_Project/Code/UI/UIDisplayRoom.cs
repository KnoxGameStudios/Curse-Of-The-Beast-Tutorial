using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using TMPro;
using UnityEngine;

namespace KnoxGameStudios
{
    public class UIDisplayRoom : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomGameModeText;
        [SerializeField] private GameObject _exitButton;        
        [SerializeField] private GameObject _roomContainer;
        [SerializeField] private GameObject[] _hideObjects;
        [SerializeField] private GameObject[] _showObjects;        

        public static Action OnLeaveRoom = delegate { };

        private void Awake()
        {           
            PhotonRoomController.OnJoinRoom += HandleJoinRoom;
            PhotonRoomController.OnRoomLeft += HandleRoomLeft;
        }

        private void OnDestroy()
        {
            PhotonRoomController.OnJoinRoom -= HandleJoinRoom;
            PhotonRoomController.OnRoomLeft -= HandleRoomLeft;
        }

        private void HandleJoinRoom(GameMode gameMode)
        {
            _roomGameModeText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"].ToString());
            
            _exitButton.SetActive(true);
            _roomContainer.SetActive(true);

            foreach (GameObject obj in _hideObjects)
            {
                obj.SetActive(false);
            }
        }

        private void HandleRoomLeft()
        {
            _roomGameModeText.SetText("JOINING ROOM");

            _exitButton.SetActive(false);
            _roomContainer.SetActive(false);
            foreach (GameObject obj in _showObjects)
            {
                obj.SetActive(true);
            }
        }

        public void LeaveRoom()
        {
            OnLeaveRoom?.Invoke();
        }
    }
}