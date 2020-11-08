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

            OnCreateTeams?.Invoke(_roomTeams, gameMode);
            
            foreach(PhotonTeam team in _roomTeams)
            {
                int teamPlayerCount = PhotonTeamsManager.Instance.GetTeamMembersCount(team.Code);

                if(teamPlayerCount < gameMode.TeamSize)
                {
                    Debug.Log($"Auto assigned to {team.Name}");
                    if(PhotonNetwork.LocalPlayer.GetPhotonTeam() == null)
                    {
                        PhotonNetwork.LocalPlayer.JoinTeam(team.Code);
                    }
                    else if(PhotonNetwork.LocalPlayer.GetPhotonTeam().Code != team.Code)
                    {
                        PhotonNetwork.LocalPlayer.SwitchTeam(team.Code);
                    }
                    break;
                }
            }
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

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            object teamCodeObject;
            if (changedProps.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamCodeObject))
            {
                if (teamCodeObject == null) return;

                byte teamCode = (byte)teamCodeObject;
                Debug.Log($"{targetPlayer.NickName} changed to team code {teamCode}");
                
                PhotonTeam newTeam;
                if(PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out newTeam))
                {
                    Debug.Log($"Switching {targetPlayer.NickName} to new team {newTeam.Name}");
                    OnSwitchTeam?.Invoke(targetPlayer, newTeam);
                }
            }
        }
    }
}