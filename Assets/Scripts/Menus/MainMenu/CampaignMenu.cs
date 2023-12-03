using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class CampaignMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] public GameObject WarningTutoNotCleared;
    [SerializeField] public GameObject WarningMissionNotCleared;

    public void StartTuto()
     {
        PhotonNetwork.LoadLevel(4);
        FindFirstObjectByType<AudioManager>().PlaySound("UniverseMusic");
     }
 
     public void StartMission()
     {
         if (PlayerPrefs.GetInt("tutoCleared",0)==1)
         {
             PhotonNetwork.LoadLevel(5);
             FindFirstObjectByType<AudioManager>().PlaySound("BattleMusic");
         }
         else
         {
             WarningTutoNotCleared.GetComponent<TemporaryMenuMessage>().Activate();
         }
     }
     
     public void StartMission2()
     {
         if (PlayerPrefs.GetInt("missionCleared",0)==1)
         {
             PhotonNetwork.LoadLevel("Mission2");
             FindFirstObjectByType<AudioManager>().PlaySound("BattleMusic");
         }
         else
         {
             WarningMissionNotCleared.GetComponent<TemporaryMenuMessage>().Activate();
         }
     }
     
     public void StartMission3()
     {
         if (PlayerPrefs.GetInt("mission2Cleared",0)==1)
         {
             PhotonNetwork.LoadLevel("Mission3");
             FindFirstObjectByType<AudioManager>().PlaySound("BattleMusic");
         }
         else
         {
             WarningMissionNotCleared.GetComponent<TemporaryMenuMessage>().Activate();
         }
     }
}