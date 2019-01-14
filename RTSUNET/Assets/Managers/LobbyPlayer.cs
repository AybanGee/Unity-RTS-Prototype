using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

    public int team;

    public string P_name;

    public int faction;

    public int colorIndex;

    public bool ready = false;

    public int baseNo = -1;

    ComponentHandler CH;
    void Start () {

        CH = GetComponent<ComponentHandler> ();

        this.transform.SetParent (LobbyManager.singleton.GetComponent<LobbyManager> ().playerUIiPanel.transform, false);

        if (isLocalPlayer == false) {
            //this belongs to another player
            GetComponent<CanvasGroup> ().interactable = false;
            return;
        }

        CmdSelectTeam (1);
        CmdChangeName ("Player");
    }

    #region "ready player one"
    public void OnClickReady () {
        Debug.Log ("base No.:" + baseNo);

        if (!isLocalPlayer) return;

        ready = !ready;
        OnClientReady (ready);
        if (ready) {
            //change UI
            SendReadyToBeginMessage ();
            CH.components[3].componentObject.transform.GetChild (0).GetComponent<TextMeshProUGUI> ().text = "Ready";
            // CH.components[3].componentObject.GetComponent<Button>().GetComponent<Image>().color =  new Color(69,180,88,1);

        } else {
            SendNotReadyToBeginMessage ();
            CH.components[3].componentObject.transform.GetChild (0).GetComponent<TextMeshProUGUI> ().text = "Not Ready";
            //  CH.components[3].componentObject.GetComponent<Button>().GetComponent<Image>().color = new Color(191,182,151,1);

        }
    }

    public override void OnClientReady (bool state) {
        Debug.Log ("READY");
        if (state) {
            //change UI
            CH.components[3].componentObject.transform.GetChild (0).GetComponent<TextMeshProUGUI> ().text = "Ready";
            // CH.components[3].componentObject.GetComponent<Button>().GetComponent<Image>().color =  new Color(69,180,88,1);

        } else {
            CH.components[3].componentObject.transform.GetChild (0).GetComponent<TextMeshProUGUI> ().text = "Not Ready";
            //  CH.components[3].componentObject.GetComponent<Button>().GetComponent<Image>().color = new Color(191,182,151,1);

        }
    }
    #endregion
    #region "GameChanges"
    public void OnChangeTeam (int i) {

        i++;
        Debug.Log ("Changing team to : " + i);
        // Clicked red team (team nr 0)
        CmdSelectTeam (i);
    }

    public void OnChangeFaction (int f) {

        Debug.Log ("Changing faction to : " + f);
        // Clicked red team (team nr 0)
        CmdSelectFaction (f);
    }

    public void OnChangeColor (int c) {

        Debug.Log ("Changing color to : " + c);
        // Clicked red team (team nr 0)
        CmdSelectColor (c);
    }

    public void OnChangeName (string s) {
        // Clicked blueteam (team nr 1)

        Debug.Log ("Changing name to : " + s);
        CmdChangeName (s);

    }

    public void OnChangeBase (int b) {
        // Clicked blueteam (team nr 1)

        Debug.Log ("Changing base to : " + b);
        CmdChangeBase (b);

    }

    [Command]
    public void CmdChangeName (string newName) {
        Debug.Log ("SERVER: change player name from " + P_name + " to " + newName);
        P_name = newName;
        RpcChangeName (newName);
    }

    [Command]
    public void CmdSelectTeam (int teamIndex) {
        // Set team of player on the server.
        team = teamIndex;
        RpcSelectTeam (teamIndex);
    }

    [Command]
    public void CmdSelectFaction (int facitonIndex) {
        // Set team of player on the server.
        faction = facitonIndex;
        RpcSelectFaction (facitonIndex);
    }

    [Command]
    public void CmdSelectColor (int cIndex) {
        // Set team of player on the server.
        colorIndex = cIndex;
        RpcSelectColor (cIndex);
    }

    [Command]
    public void CmdChangeBase (int bIndex) {
        // Set team of player on the server.
        baseNo = bIndex;
        RpcChangeBase (bIndex);
    }

    [ClientRpc]
    public void RpcChangeName (string newName) {
        Debug.Log ("Client: change player name from " + P_name + " to " + newName);
        P_name = newName;
        CH.components[0].componentObject.GetComponent<TMP_InputField> ().text = newName;
    }

    [ClientRpc]
    public void RpcSelectTeam (int teamIndex) {
        // Set team of player on the server.
        team = teamIndex;
        CH.components[1].componentObject.GetComponent<TMP_Dropdown> ().value = Mathf.Clamp (team - 1, 0, 3);
    }

    [ClientRpc]
    public void RpcSelectFaction (int facitonIndex) {
        // Set team of player on the server.
        faction = facitonIndex;
        CH.components[2].componentObject.GetComponent<TMP_Dropdown> ().value = faction;
    }

    [ClientRpc]
    public void RpcSelectColor (int cIndex) {
        // Set team of player on the server.
        colorIndex = cIndex;
        CH.components[4].componentObject.GetComponent<Dropdown> ().value = cIndex;
    }

    [ClientRpc]
    public void RpcChangeBase (int bIndex) {
        // Set team of player on the server.
        baseNo = bIndex;
        //CH.components[4].componentObject.GetComponent<Dropdown> ().value = cIndex;
    }
    #endregion

}