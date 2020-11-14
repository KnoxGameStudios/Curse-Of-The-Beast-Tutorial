using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;

namespace KnoxGameStudios
{
    public class UIDisplayRoom : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomTitleText;
        [SerializeField] private GameObject _startButton;
        [SerializeField] private GameObject _exitButton;        
        [SerializeField] private GameObject _roomContainer;
        [SerializeField] private GameObject[] _hideObjects;
        [SerializeField] private GameObject[] _showObjects;

        public static Action OnStartGame = delegate { };
        public static Action OnLeaveRoom = delegate { };

        private void Awake()
        {           
            PhotonRoomController.OnJoinRoom += HandleJoinRoom;
            PhotonRoomController.OnRoomLeft += HandleRoomLeft;
            PhotonRoomController.OnMasterOfRoom += HandleMasterOfRoom;
            PhotonRoomController.OnCountingDown += HandleCountingDown;
        }

        private void OnDestroy()
        {
            PhotonRoomController.OnJoinRoom -= HandleJoinRoom;
            PhotonRoomController.OnRoomLeft -= HandleRoomLeft;
            PhotonRoomController.OnMasterOfRoom -= HandleMasterOfRoom;
            PhotonRoomController.OnCountingDown -= HandleCountingDown;
        }

        private void HandleJoinRoom(GameMode gameMode)
        {
            _roomTitleText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"].ToString());
            
            _exitButton.SetActive(true);
            _roomContainer.SetActive(true);

            foreach (GameObject obj in _hideObjects)
            {
                obj.SetActive(false);
            }
        }

        private void HandleRoomLeft()
        {
            _roomTitleText.SetText("JOINING ROOM");

            _startButton.SetActive(false);
            _exitButton.SetActive(false);
            _roomContainer.SetActive(false);
            foreach (GameObject obj in _showObjects)
            {
                obj.SetActive(true);
            }
        }

        private void HandleMasterOfRoom(Player masterPlayer)
        {
            _roomTitleText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"].ToString());

            if (PhotonNetwork.LocalPlayer.Equals(masterPlayer))
            {
                _startButton.SetActive(true);
            }
            else
            {
                _startButton.SetActive(false);
            }
        }

        private void HandleCountingDown(float count)
        {
            _startButton.SetActive(false);
            _exitButton.SetActive(false);
            _roomTitleText.SetText(count.ToString("F0"));
        }

        public void LeaveRoom()
        {
            OnLeaveRoom?.Invoke();
        }

        public void StartGame()
        {
            Debug.Log($"Starting game...");
            OnStartGame?.Invoke();
        }
    }
}