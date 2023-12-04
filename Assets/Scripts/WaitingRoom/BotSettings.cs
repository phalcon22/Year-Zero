using Photon.Pun;

public class BotSettings : WaitingRoomSettings
{
    protected override void InitPanel()
    {
        base.InitPanel();
        kickButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void Kick()
    {
        PhotonNetwork.Destroy(photonView);
        FindFirstObjectByType<PlayersManager>().OnBotRemoved();
    }
}
