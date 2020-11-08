using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;

namespace KnoxGameStudios
{
    public class PhotonRoomController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameMode _selectedGameMode;
        [SerializeField] private GameMode[] _availableGameModes;
        private const string GAME_MODE = "GAMEMODE";        

        public static Action<GameMode> OnJoinRoom = delegate { };        
        public static Action<bool> OnRoomStatusChange = delegate { };
        public static Action OnRoomLeft = delegate { };
        public static Action<Player> OnOtherPlayerLeftRoom = delegate { };

        private void Awake()
        {
            UIGameMode.OnGameModeSelected += HandleGameModeSelected;
            UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
            PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
            UIDisplayRoom.OnLeaveRoom += HandleLeaveRoom;
            UIFriend.OnGetRoomStatus += HandleGetRoomStatus;                        
        }

        private void OnDestroy()
        {
            UIGameMode.OnGameModeSelected -= HandleGameModeSelected;
            UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
            PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
            UIDisplayRoom.OnLeaveRoom -= HandleLeaveRoom;
            UIFriend.OnGetRoomStatus -= HandleGetRoomStatus;            
        }

        #region Handle Methods
        private void HandleGameModeSelected(GameMode gameMode)
        {
            if (!PhotonNetwork.IsConnectedAndReady) return;
            if (PhotonNetwork.InRoom) return;

            _selectedGameMode = gameMode;
            Debug.Log($"Joining new {_selectedGameMode.Name} game");            
            JoinPhotonRoom();
        }

        private void HandleRoomInviteAccept(string roomName)
        {
            PlayerPrefs.SetString("PHOTONROOM", roomName);
            if (PhotonNetwork.InRoom)
            {
                OnRoomLeft?.Invoke();
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.JoinRoom(roomName);
                    PlayerPrefs.SetString("PHOTONROOM", "");
                }
            }
        }

        private void HandleLobbyJoined()
        {
            string roomName = PlayerPrefs.GetString("PHOTONROOM");
            if (!string.IsNullOrEmpty(roomName))
            {
                PhotonNetwork.JoinRoom(roomName);
                PlayerPrefs.SetString("PHOTONROOM", "");
            }
        }

        private void HandleLeaveRoom()
        {
            if(PhotonNetwork.InRoom)
            {
                OnRoomLeft?.Invoke();
                PhotonNetwork.LeaveRoom();
            }            
        }

        private void HandleGetRoomStatus()
        {
            OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
        }
        #endregion

        #region Private Methods
        private void JoinPhotonRoom()
        {
            Hashtable expectedCustomRoomProperties = new Hashtable()
            { {GAME_MODE, _selectedGameMode.Name} };

            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        }
        private void CreatePhotonRoom()
        {
            string roomName = Guid.NewGuid().ToString();
            RoomOptions ro = GetRoomOptions();

            PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
        }
        private RoomOptions GetRoomOptions()
        {
            RoomOptions ro = new RoomOptions();
            ro.IsOpen = true;
            ro.IsVisible = true;
            ro.MaxPlayers = _selectedGameMode.MaxPlayers;

            string[] roomProperties = { GAME_MODE };

            Hashtable customRoomProperties = new Hashtable()
            { {GAME_MODE, _selectedGameMode.Name} };

            ro.CustomRoomPropertiesForLobby = roomProperties;
            ro.CustomRoomProperties = customRoomProperties;

            return ro;
        }
        private void DebugPlayerList()
        {
            string players = "";
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                players += $"{player.Value.NickName}, ";
            }
            Debug.Log($"Current Room Players: {players}");
        }
        private GameMode GetRoomGameMode()
        {
            string gameModeName = (string)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE];
            GameMode gameMode = null;
            for (int i = 0; i < _availableGameModes.Length; i++)
            {
                if (string.Compare(_availableGameModes[i].Name, gameModeName) == 0)
                {
                    gameMode = _availableGameModes[i];
                    break;
                }
            }
            return gameMode;
        }
        #endregion

        #region Photon Callbacks
        public override void OnCreatedRoom()
        {
            Debug.Log($"You have created a Photon Room named {PhotonNetwork.CurrentRoom.Name}");            
        }
        public override void OnJoinedRoom()
        {
            Debug.Log($"You have joined the Photon room {PhotonNetwork.CurrentRoom.Name}");
            DebugPlayerList();

            _selectedGameMode = GetRoomGameMode();
            OnJoinRoom?.Invoke(_selectedGameMode);
            OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
        }
        public override void OnLeftRoom()
        {
            Debug.Log("You have left a Photon Room");
            _selectedGameMode = null;
            OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);            
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRandomFailed {message}");
            CreatePhotonRoom();
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"You failed to join a Photon room: {message}");
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Another player has joined the room {newPlayer.NickName}");
            DebugPlayerList();
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player has left the room {otherPlayer.NickName}");
            OnOtherPlayerLeftRoom?.Invoke(otherPlayer);
            DebugPlayerList();
        }
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log($"New Master Client is {newMasterClient.NickName}");
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            Debug.Log($"{targetPlayer.NickName} has changed something");            
        }
        #endregion
    }
}