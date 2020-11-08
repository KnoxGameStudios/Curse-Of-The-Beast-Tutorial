using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KnoxGameStudios
{
    public class UIGameMode : MonoBehaviour
    {
        [SerializeField] private GameMode _gameMode;
        public static Action<GameMode> OnGameModeSelected = delegate { };

        public void SelectGameMode()
        {
            if (_gameMode == null) return;

            OnGameModeSelected?.Invoke(_gameMode);
        }
    }
}