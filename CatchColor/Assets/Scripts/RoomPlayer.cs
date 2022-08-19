using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public MyColor playerColor;


    //public CharacterMover lobbyPlayerCharacter;

    public void Start()
    {
        base.Start();
        if (isServer)
        {
            SpawnLobbyPlayerCharacter();
        }

    }

    private void SpawnLobbyPlayerCharacter()
    {

        int idx = Random.Range(0, 3);
        MyColor color;
        if (idx == 0) color = MyColor.Red;
        else if (idx == 1) color = MyColor.Green;
        else color = MyColor.Blue;
        playerColor = color;

        var playerCharacter = Instantiate(RoomManager.singleton.spawnPrefabs[0]).GetComponent<CharacterMover>();
        NetworkServer.Spawn(playerCharacter.gameObject, connectionToClient);
        playerCharacter.playerColor = color;
    }


}
