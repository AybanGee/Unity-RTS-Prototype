using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

    public int team = 1;
    [SyncVar]
    public string P_name = "Player";

    public int faction = 1;

    public int colorIndex = 1;

    public bool ready = false;

    [SyncVar]
    public int baseNo = -1;

    ComponentHandler CH;

    public LobbyManager LM;

    [SyncVar]
    public int mapNumber;
    public MapSelection mapDropdown;
    public Dropdown dropdown;
    public Coroutine broadcaster = null, mapbroadcaster = null;
    
    //Debugging
    public GameObject flag;
    
    void Start () {
        LM = LobbyManager.singleton.GetComponent<LobbyManager> ();
        CH = GetComponent<ComponentHandler> ();
        mapDropdown = LM.mapDropdown;
        dropdown = mapDropdown.transform.GetComponent<Dropdown> ();
        dropdown.onValueChanged.AddListener (delegate {
            SetMap (dropdown.value);
        });

        this.transform.SetParent (LobbyManager.singleton.GetComponent<LobbyManager> ().playerUIiPanel.transform, false);
        if (isLocalPlayer == false) {
            //this belongs to another player
            GetComponent<CanvasGroup> ().interactable = false;
            return;
        }

        CmdSelectTeam (1);
        CmdChangeName ("Player");

        if (isLocalPlayer) {
            if (broadcaster == null){
                broadcaster = StartCoroutine (broadcastSettings ());
                flag.SetActive(true);
            }

            if (isServer && mapbroadcaster == null)
               mapbroadcaster = StartCoroutine(broadcastMap());
        }

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
    public void InitData () {
        CmdChangeName ( CH.components[0].componentObject.GetComponent<TMP_InputField> ().text);
        CmdSelectFaction ( CH.components[2].componentObject.GetComponent<TMP_Dropdown> ().value);
        CmdSelectTeam ( CH.components[1].componentObject.GetComponent<TMP_Dropdown> ().value + 1);
        CmdSelectColor (CH.components[4].componentObject.GetComponent<Dropdown> ().value);
    }
    float sendRate = 1;
    IEnumerator broadcastSettings () {

        while (true) {
            InitData ();
            yield return new WaitForSeconds (1 / sendRate);
        }

    }
    IEnumerator broadcastMap () {

        while (true) {
            CmdChangeMap(LM.mapDropdown.gameObject.GetComponent<Dropdown>().value);
            yield return new WaitForSeconds (1 / sendRate);
        }

    }
    public void OnChangeTeam (int i) {
        if (!isLocalPlayer) return;

        i++;
        Debug.Log ("Changing team to : " + i);
        // Clicked red team (team nr 0)
        CmdSelectTeam (i);
        //team = i;
    }

    public void OnChangeFaction (int f) {
        if (!isLocalPlayer) return;

        Debug.Log ("Changing faction to : " + f);
        // Clicked red team (team nr 0)
        CmdSelectFaction (f);
        //faction = f;
    }

    public void OnChangeColor (int c) {
        if (!isLocalPlayer) return;

        Debug.Log ("Changing color to : " + c);
        // Clicked red team (team nr 0)
        CmdSelectColor (c);
       //colorIndex = c;
    }

    public void OnChangeName (string s) {
        // Clicked blueteam (team nr 1)
        if (!isLocalPlayer) return;

        Debug.Log ("Changing name to : " + s);
        CmdChangeName (s);
        //name = s;
    }

    public void OnChangeBase (int b) {
        // Clicked blueteam (team nr 1)
        //if (!isLocalPlayer) return;
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
        if (isLocalPlayer) return;
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

    public void SetMap (int mapIndex) {
        CmdChangeMap (mapIndex);
    }

    [Command]
    public void CmdChangeMap (int mapIndex) {
        //Set Map
        dropdown.value = mapIndex;
        RpcChangeMap (mapIndex);
    }

    [ClientRpc]
    public void RpcChangeMap (int mapIndex) {
        mapNumber = mapIndex;
        dropdown.value = mapIndex;
    }


}