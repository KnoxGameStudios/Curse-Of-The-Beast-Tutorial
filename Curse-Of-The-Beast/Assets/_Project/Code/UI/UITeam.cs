using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KnoxGameStudios
{
    public class UITeam : MonoBehaviour
    {
        [SerializeField] private int _teamSize;
        [SerializeField] private int _maxTeamSize;
        [SerializeField] private PhotonTeam _team;
        [SerializeField] private TMP_Text _teamNameText;
        [SerializeField] private Transform _playerSelectionContainer;
        [SerializeField] private UIPlayerSelection _playerSelectionPrefab;
        [SerializeField] private Dictionary<Player, UIPlayerSelection> _playerSelections;

        public static Action<PhotonTeam> OnSwitchToTeam = delegate { };

        private void Awake()
        {
            UIDisplayTeam.OnAddPlayerToTeam += HandleAddPlayerToTeam;
            UIDisplayTeam.OnRemovePlayerFromTeam += HandleRemovePlayerFromTeam;
            PhotonRoomController.OnRoomLeft += HandleLeaveRoom;            
        }

        private void OnDestroy()
        {
            UIDisplayTeam.OnAddPlayerToTeam -= HandleAddPlayerToTeam;
            UIDisplayTeam.OnRemovePlayerFromTeam -= HandleRemovePlayerFromTeam;
            PhotonRoomController.OnRoomLeft -= HandleLeaveRoom;
        }

        public void Initialize(PhotonTeam team, int teamSize)
        {
            _team = team;
            _maxTeamSize = teamSize;
            Debug.Log($"{_team.Name} is added with the size {_maxTeamSize}");            
            _playerSelections = new Dictionary<Player, UIPlayerSelection>();
            UpdateTeamUI();

            Player[] teamMembers;
            if (PhotonTeamsManager.Instance.TryGetTeamMembers(_team.Code, out teamMembers))
            {
                foreach (Player player in teamMembers)
                {
                   AddPlayerToTeam(player);
                }
            }       
        }

        public void HandleAddPlayerToTeam(Player player, PhotonTeam team)
        {
            if (_team.Code == team.Code)
            {
                Debug.Log($"Updating {_team.Name} UI to add {player.NickName}");
                AddPlayerToTeam(player);
            }
        }

        public void HandleRemovePlayerFromTeam(Player player)
        {
            RemovePlayerFromTeam(player);
        }

        private void HandleLeaveRoom()
        {
            Destroy(gameObject);
        }

        private void UpdateTeamUI()
        {
            _teamNameText.SetText($"{_team.Name} \n {_playerSelections.Count} / {_maxTeamSize}");
        }

        private void AddPlayerToTeam(Player player)
        {
            UIPlayerSelection uiPlayerSelection = Instantiate(_playerSelectionPrefab, _playerSelectionContainer);
            uiPlayerSelection.Initialize(player);
            _playerSelections.Add(player, uiPlayerSelection);
            UpdateTeamUI();
        }

        private void RemovePlayerFromTeam(Player player)
        {
            if(_playerSelections.ContainsKey(player))
            {
                Debug.Log($"Updating {_team.Name} UI to remove {player.NickName}");
                Destroy(_playerSelections[player].gameObject);
                _playerSelections.Remove(player);
                UpdateTeamUI();
            }
        }

        public void SwitchToTeam()
        {
            Debug.Log($"Trying to switch to team {_team.Name}");
            if (_teamSize == _maxTeamSize) return;

            Debug.Log($"Switching to team {_team.Name}");
            OnSwitchToTeam?.Invoke(_team);
        }
    }
}