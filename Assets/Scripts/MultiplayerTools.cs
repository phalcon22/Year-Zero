using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MultiplayerTools : MonoBehaviour
{
    public static bool IsMine(SelectableObj unit)
    {
        return unit.photonView.IsMine && unit.botIndex == -1;
    }

    public static int GetTeamOf(SelectableObj unit)
    {
        int res;
        if (PhotonNetwork.OfflineMode)
        {
            if (unit.botIndex == -1)
            {
                res = InstanceManager.instanceManager.GetTeam();
            }
            else if (unit.botIndex == -2)
            {
                res = -2;
            }
            else
            {
                res = InstanceManager.instanceManager.GetBot(unit.botIndex).GetTeam();
            }
        }
        else
        {
            if (unit.botIndex == -1)
            {
                res = (int)PhotonNetwork.CurrentRoom.CustomProperties["Player" + unit.photonView.Owner.ActorNumber + "Team"];
            }
            else if (unit.botIndex == -2)
            {
                res = -2;
            }
            else
            {
                res = (int)PhotonNetwork.CurrentRoom.CustomProperties["Bot" + unit.botIndex + "Team"];
            }
        }
        return res;
    }

    public static Color32 GetColorOf(SelectableObj unit)
    {
        int res;
        if (PhotonNetwork.OfflineMode)
        {
            if (unit.botIndex == -1)
            {
                return InstanceManager.instanceManager.GetColor();
            }
            else if (unit.botIndex == -2)
            {
                res = 1;
            }
            else
            {
                return InstanceManager.instanceManager.GetBot(unit.botIndex).GetColor();
            }
        }
        else
        {
            if (unit.botIndex == -1)
            {
                res = (int)PhotonNetwork.CurrentRoom.CustomProperties["Player" + unit.photonView.Owner.ActorNumber.ToString() + "Color"];
            }
            else if (unit.botIndex == -2)
            {
                res = -2;
            }
            else
            {
                res = (int)PhotonNetwork.CurrentRoom.CustomProperties["Bot" + unit.botIndex + "Color"];
            }
        }
        return Int2Color(res);
    }

    public static Color32 Int2Color(int val)
    {
        Color32 res;
        switch (val)
        {
        case 0: // Red
            res = new Color32(255, 0, 0, 255);
            break;
        case 1: // Green
            res = new Color32(0, 255, 0, 255);
            break;
        case 2: // Blue
            res = new Color32(0, 0, 255, 255);
            break;
        case 3: // Orange
            res = new Color32(255, 127, 39, 255);
            break;
        case 4: // Pink
            res = new Color32(255, 174, 201, 255);
            break;
        case 5: // White
            res = new Color32(255, 255, 255, 255);
            break;
        case 6: // Yellow
            res = new Color32(255, 242, 0, 255);
            break;
        default:
            res = new Color32(0, 0, 0, 255);
            Debug.LogError("Wrong color selected");
            break;
        }
        return res;
    }


    public static string GetHolderOf(SelectableObj unit)
    {
        string res = "Holder";
        if (PhotonNetwork.OfflineMode)
        {
            if (unit.botIndex == -1)
            {
                res += "Player";
            }
            else
            {
                res += unit.botIndex.ToString();
            }
        }
        else
        {
            if (unit.botIndex == -1)
            {
                res += unit.photonView.Owner.NickName;
            }
            else
            {
                res += unit.botIndex.ToString();
            }
        }
        return res;
    }
}
