using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KnoxGameStudios
{
    public class UIDisplayTeam : MonoBehaviour
    {
        [SerializeField] private UITeam _uiTeamPrefab;
        [SerializeField] private List<UITeam> _uiTeams;
        [SerializeField] private Transform _teamContainer;

        public static Action<Player, PhotonTeam> OnAddPlayerToTeam = delegate { };
        public static Action<Player> OnRemovePlayerFromTeam = delegate { };

        private void Awake()
        {
            PhotonTeamController.OnCreateTeams += HandleCreateTeams;
            PhotonTeamController.OnSwitchTeam += HandleSwitchTeam;
            PhotonTeamController.OnRemovePlayer += HandleRemovePlayer;
            PhotonTeamController.OnClearTeams += HandleClearTeams;
            _uiTeams = new List<UITeam>();
        }

        private void OnDestroy()
        {
            PhotonTeamController.OnCreateTeams -= HandleCreateTeams;
            PhotonTeamController.OnSwitchTeam += HandleSwitchTeam;
            PhotonTeamController.OnRemovePlayer += HandleRemovePlayer;
            PhotonTeamController.OnClearTeams -= HandleClearTeams;
        }

        private void HandleCreateTeams(List<PhotonTeam> teams, GameMode gameMode)
        {
            foreach (PhotonTeam team in teams)
            {
                UITeam uiTeam = Instantiate(_uiTeamPrefab, _teamContainer);                
                uiTeam.Initialize(team, gameMode.TeamSize);
                _uiTeams.Add(uiTeam);
            }
        }

        private void HandleSwitchTeam(Player player, PhotonTeam newTeam)
        {
            Debug.Log($"Updating UI to move {player.NickName} to {newTeam.Name}");
            
            OnRemovePlayerFromTeam?.Invoke(player);
            
            OnAddPlayerToTeam?.Invoke(player, newTeam);            
        }

        private void HandleRemovePlayer(Player otherPlayer)
        {
            OnRemovePlayerFromTeam?.Invoke(otherPlayer);
        }

        private void HandleClearTeams()
        {
            foreach (UITeam uiTeam in _uiTeams)
            {
                Destroy(uiTeam.gameObject);
            }
            _uiTeams.Clear();
        }
    }
}