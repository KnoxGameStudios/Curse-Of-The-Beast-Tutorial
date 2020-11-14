using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KnoxGameStudios
{
    public class PhotonGameController : MonoBehaviourPunCallbacks
    {
        public void GetOutOfThisRoomNow()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(1);
        }
    }
}