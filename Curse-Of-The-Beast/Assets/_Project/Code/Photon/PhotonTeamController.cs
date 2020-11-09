using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KnoxGameStudios
{
    public class PhotonTeamController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<PhotonTeam> _roomTeams;
        [SerializeField] private int _teamSize;
        [SerializeField] private PhotonTeam _priorTeam;

        public static Action<List<PhotonTeam>, GameMode> OnCreateTeams = delegate { };
        public static Action<Player, PhotonTeam> OnSwitchTeam = delegate { };
        public static Action<Player> OnRemovePlayer = delegate { };
        public static Action OnClearTeams = delegate { };

        #region Unity Methods
        private void Awake()
        {
            UITeam.OnSwitchToTeam += HandleSwitchTeam;
            PhotonRoomController.OnJoinRoom += HandleCreateTeams;
            PhotonRoomController.OnRoomLeft += HandleLeaveRoom;
            PhotonRoomController.OnOtherPlayerLeftRoom += HandleOtherPlayerLeftRoom;

            _roomTeams = new List<PhotonTeam>();
        }

        private void OnDestroy()
        {
            UITeam.OnSwitchToTeam -= HandleSwitchTeam;
            PhotonRoomController.OnJoinRoom -= HandleCreateTeams;
            PhotonRoomController.OnRoomLeft -= HandleLeaveRoom;
            PhotonRoomController.OnOtherPlayerLeftRoom -= HandleOtherPlayerLeftRoom;
        }
        #endregion

        #region Handle Methods
        private void HandleSwitchTeam(PhotonTeam newTeam)
        {            
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == null)
            {
                _priorTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
                PhotonNetwork.LocalPlayer.JoinTeam(newTeam);                
            }
            else if (CanSwitchToTeam(newTeam))
            {
                _priorTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
                PhotonNetwork.LocalPlayer.SwitchTeam(newTeam);                
            }
        }

        private void HandleCreateTeams(GameMode gameMode)
        {
            CreateTeams(gameMode);

            OnCreateTeams?.Invoke(_roomTeams, gameMode);

            AutoAssignPlayerToTeam(PhotonNetwork.LocalPlayer, gameMode);
        }

        private void HandleLeaveRoom()
        {
            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
            _roomTeams.Clear();
            _teamSize = 0;
            OnClearTeams?.Invoke();
        }

        private void HandleOtherPlayerLeftRoom(Player otherPlayer)
        {
            OnRemovePlayer?.Invoke(otherPlayer);
        }
        #endregion

        #region Private Methods
        private void CreateTeams(GameMode gameMode)
        {
            _teamSize = gameMode.TeamSize;
            int numberOfTeams = gameMode.MaxPlayers;
            if (gameMode.HasTeams)
            {
                numberOfTeams = gameMode.MaxPlayers / gameMode.TeamSize;
            }

            for (int i = 1; i <= numberOfTeams; i++)
            {
                _roomTeams.Add(new PhotonTeam
                {
                    Name = $"Team {i}",
                    Code = (byte)i
                });
            }
        }

        private bool CanSwitchToTeam(PhotonTeam newTeam)
        {
            bool canSwitch = false;

            if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Code != newTeam.Code)
            {
                Player[] players = null;
                if (PhotonTeamsManager.Instance.TryGetTeamMembers(newTeam.Code, out players))
                {
                    if (players.Length < _teamSize)
                    {
                        canSwitch = true;
                    }
                    else
                    {
                        Debug.Log($"{newTeam.Name} is full");
                    }
                }
            }
            else
            {
                Debug.Log($"You are already on the team {newTeam.Name}");
            }

            return canSwitch;
        }

        private void AutoAssignPlayerToTeam(Player player, GameMode gameMode)
        {
            foreach (PhotonTeam team in _roomTeams)
            {
                int teamPlayerCount = PhotonTeamsManager.Instance.GetTeamMembersCount(team.Code);

                if (teamPlayerCount < gameMode.TeamSize)
                {
                    Debug.Log($"Auto assigned {player.NickName} to {team.Name}");
                    if (player.GetPhotonTeam() == null)
                    {
                        player.JoinTeam(team.Code);
                    }
                    else if (player.GetPhotonTeam().Code != team.Code)
                    {
                        player.SwitchTeam(team.Code);
                    }
                    break;
                }
            }
        }
        #endregion

        #region Photon Callback Methods
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            object teamCodeObject;
            if (changedProps.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamCodeObject))
            {
                if (teamCodeObject == null) return;

                byte teamCode = (byte)teamCodeObject;
                
                PhotonTeam newTeam;
                if(PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out newTeam))
                {
                    Debug.Log($"Switching {targetPlayer.NickName} to new team {newTeam.Name}");
                    OnSwitchTeam?.Invoke(targetPlayer, newTeam);
                }
            }
        }
        #endregion
    }
}