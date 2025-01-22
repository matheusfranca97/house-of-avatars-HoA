using Supabase;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Client = Supabase.Client;
using Unity.Services.Matchmaker;
using System.Collections;
using System.Data;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using Unity.Services.Authentication.PlayerAccounts;



#if !UNITY_WEBGL
using Unity.Services.Multiplay;
#endif
public class DedicatedServerManager : MonoBehaviour
{
    public static DedicatedServerManager instance;

#if !UNITY_WEBGL
    private static readonly string supabaseURL = "https://kgyimaymtoepvyfcyfsd.supabase.co";
    private static readonly string supabaseTestAPIKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtneWltYXltdG9lcHZ5ZmN5ZnNkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MjgyOTgxMzMsImV4cCI6MjA0Mzg3NDEzM30.aT5Rgl7WO_tUr7bfRsVLXEH1pLxo0cP5dD2cJjyg7lk";
    private static readonly string supabaseServiceKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtneWltYXltdG9lcHZ5ZmN5ZnNkIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTcyODI5ODEzMywiZXhwIjoyMDQzODc0MTMzfQ.cLRYG4wqAbR8KvJbRQGrzyIVb2_2KUnnpHEgf4UIza4";
    private IServerQueryHandler serverQueryHandler;

    private static readonly string serverCertificate =
@"-----BEGIN CERTIFICATE-----
MIIDWTCCAkECFAtfO6HpmlSfxX0Lo4ISDRHkqEEQMA0GCSqGSIb3DQEBCwUAMGkx
CzAJBgNVBAYTAlVLMRYwFAYDVQQIDA1XZXN0IE1pZGxhbmRzMRQwEgYDVQQHDAtT
dG91cmJyaWRnZTEXMBUGA1UECgwOQXp1bG9uIFN0dWRpb3MxEzARBgNVBAMMCkhv
QV9TZXJ2ZXIwHhcNMjQxMDEyMTAzNzM2WhcNMjUxMDEyMTAzNzM2WjBpMQswCQYD
VQQGEwJVSzEWMBQGA1UECAwNV2VzdCBNaWRsYW5kczEUMBIGA1UEBwwLU3RvdXJi
cmlkZ2UxFzAVBgNVBAoMDkF6dWxvbiBTdHVkaW9zMRMwEQYDVQQDDApIb0FfU2Vy
dmVyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwX6146UERCevjbaX
MZYHzvi13N+VYwwgMSpDgfda75LGjYNykwCEeu//hcJt+o/pdXKiOBY8gC8hgIpG
XRuoxRpH+PG3CZCA1J5toXmUC/M4bYfFlLp3okhpTMIxw3o7kXwHENDtVcsYkpWn
ItsPF6Q9B2/I+B/hK2TMMbFiXCEldSfVgA8h3diugxA8W8k0tYPm3mJnwzL49rpT
RdDKkE8R+6MdRjwMZH16SbgWK4PUuoXYietChnNbRSiWrXDavHdX3nTCJd2CErcp
DxY8RjAyX3q0B+TfjPOC78U7rQAOnYgsPIniGXd9dCSN+sFhFsq/yDSN+o+A5FXK
AOpN8QIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQBo9c8iyu2Tvbk42j5IV63l3wOU
C4RB1j9cesOeNuCPDzxf+TLrQAUQzlFCHDM5OobPsw1qrjRsYfwaHH6hDvol9H0b
pzgISzdxOnQlgKNZPsD+pdSxO6wLXprVrTt1Mw7u+IOIQe0c0j0WzQ5fdHOyUk0y
VP6mmwhHGKSVcKcvYZ9+DOoHWinHSQt1nlHM3IVsBriQ0LCwGKZs32mF4LjXACsD
giapQTaPR2d7S6khlDzexiC6Ny2JnGhH1+8C4PHfjaaVBP9YbEJJzwiH3mwPtvFL
h3mNU26GIH3GaJ5ph7rXy92bpzO3zLpST0VEw3EwbtnoIHNgjZZRQRuNCtP5
-----END CERTIFICATE-----";
    private static readonly string serverPrivateKey =
@"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEAwX6146UERCevjbaXMZYHzvi13N+VYwwgMSpDgfda75LGjYNy
kwCEeu//hcJt+o/pdXKiOBY8gC8hgIpGXRuoxRpH+PG3CZCA1J5toXmUC/M4bYfF
lLp3okhpTMIxw3o7kXwHENDtVcsYkpWnItsPF6Q9B2/I+B/hK2TMMbFiXCEldSfV
gA8h3diugxA8W8k0tYPm3mJnwzL49rpTRdDKkE8R+6MdRjwMZH16SbgWK4PUuoXY
ietChnNbRSiWrXDavHdX3nTCJd2CErcpDxY8RjAyX3q0B+TfjPOC78U7rQAOnYgs
PIniGXd9dCSN+sFhFsq/yDSN+o+A5FXKAOpN8QIDAQABAoIBAGFZKpGZWAgiH0Sg
9HhSDyOmJXk2U6Y9V4Tkyon8tJeLtLFFzMMAo6ZmUJwvMb254a7hOZQWO+IR1D0j
VDtLyyE/E66/jWMWfHp8KpPu4vkQKPeSM2mcVswiujeQDBFY0ddkGvnu4zkisP4u
pKP4qiMu0jWHnAiZoWN/luv5Xo8SjE2NjsTgAKUq/nyJSFy4S7cpajf6VH9LNgYS
PeR//DyKg+BURRtKjcUT/19JkLO3pvxl9rguSq6CTZq1l4DG2lkJy+B956oPesKU
pQ8nqVYeHDmpySNH5glNq4HBkE6K//zrTeIB8bu58stOCCCRm+/0W8QBde0AMr5r
+XuPenECgYEA/MgV4FBxhQq41WdtGO/NjTUAItJy22LMejWwMeYbzlWlwpWLV7F8
Z3CdnHjAAkPQKRGcQdbK91G8E7rPNkzQYqcKY15gfOiK3L2bM56NLiCFIqXlzdWI
PXnRsfsc7gw1h50yEdC8VWsxBfXtHP4asIhgAMfAEBYEjaRssIwrFhsCgYEAw/Vi
+2jU4bMDvpbXPpSxdOJFtDJpIac+LGHBdUSEj+41puT1Sascro1Pyokgsy9I4HSq
9EmuUx6AtkfU/u8wjDWRcIXXhb6WV8yOxyqu8MjoAegOzk5OBrhTIL6kP/ZbitJo
tydi7x4M9hntn8TkFbIZSWMstHCvWb9QnKyMXOMCgYAKVErAjcj1vMhsv/svR61I
ld/ZjGvxFwpv+/2lLFf6iHlriBzXioMg3vMxz6VY8lhxNS0Da7mDfa2HyNxqxZzG
SzkbcmHS+NWjy4OqClKOjfmivtCzJoSYrn+pHC/Ecm9FiWDgZX0sqGKqcbAsvR1u
FUSHA6KPhbbN6ugeFrwz/QKBgElIcpzs5ngFl4fmJ1b7CqZYnJK4K4LvZZv5bvzp
A95DyoLAq07ClDZfGJD42WbJbyqp1ukGyQ/Cn4YLtQcl8nTs75gyJZiZ3uW01Ux8
lPHtYH6eBzN2K03uDwB26zwUaMWwzIJ6U1BzX4uFxMz0OAw5D6XXVfehEKKynnYJ
PXDbAoGBAJacZkPMh4roA8Stii7gnh4LqlqUfbnIux7CqHGYSprl9uSmBbicDEvz
5EoF9/2jEaq2cqRsYXrLexf3YkBlVo8sy+EvSVz8JhTkRgFgDF7p5VqK5pXBRRnz
NgVIknKY/55pBVa8s0YS8O1l0jbVPFsYOCtyNPkdiWdLCGZqXJ6R
-----END RSA PRIVATE KEY-----";
#endif

    private NetworkManager networkManager;
    [SerializeField] private GameObject playerListPrefab;
    [SerializeField] private GameObject ottDisplayPrefab;

    [SerializeField] private NetworkManager localNetworkManager;
    [SerializeField] private NetworkManager relayNetworkManager;

    private Client supabaseClient;
    private IGotrueAdminClient<User> adminClient;

    private List<NetworkPlayer> networkPlayers = new List<NetworkPlayer>();
    private PlayerSpawnLocation[] spawnLocations;

    private bool useMultiplay = false;

    private const float shutdownTimer = 60;
    private Coroutine shutdownCoroutine;
    private float shutdownElapsed = 0;

    //private BackfillTicket backfillTicket;
    //private Coroutine backfillUpdateCoroutine;

    private int playerLimit;

    private List<ulong> oratorModePlayers = new();
    private int currentID = 2;

    private const float localTalkRange = 20;

    private NetworkOTTDisplay ottDisplay;

    private GameID gameID;

    #region Unity Messages
    private void Awake()
    {
        instance = this;
    }

    private async void Start()
    {
#if UNITY_SERVER
        try
        {
            //Set target frame rate low so we don't use extra CPU
            Application.targetFrameRate = 30;

            //Initialize Supabase clients
            supabaseClient = new Client(supabaseURL, supabaseServiceKey);
            await supabaseClient.InitializeAsync();
            adminClient = supabaseClient.AdminAuth(supabaseServiceKey);
            Debug.Log("Connected to Supabase");

            //Check for player count command-line argument
            string playerCountArg = GetCommandLineArg("-playerCount");
            playerLimit = 50;
            if (playerCountArg != null)
            {
                playerLimit = int.Parse(playerCountArg);
            }
            Debug.Log($"Got player count as {playerLimit}");

            Debug.Log("Server initialized");

            try
            {
                //Initialize Multiplay
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync((ushort)playerLimit, $"HoA Server Version {Application.version}", "House of Avatars", Application.version, "Church");
                MultiplayEventCallbacks callbacks = new();
                callbacks.Allocate += OnAllocated;
                callbacks.Deallocate += OnDeallocated;
                await MultiplayService.Instance.SubscribeToServerEventsAsync(callbacks);
                networkManager = relayNetworkManager;
                networkManager.SetSingleton();
                useMultiplay = true;
            }
            catch (Exception ex)
            {
                //If not running on Multiplay, set as false
                Debug.Log(ex);
                Debug.Log("Above error indicated Multiplay unavailable, this is fine if running locally");
                networkManager = localNetworkManager;
                networkManager.SetSingleton();
                OnAllocated(new MultiplayAllocation("local-open", 0, "local"));
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            ErrorShutdown("HoA server encountered an exception during startup");
            return;
        }
#else
        Destroy(gameObject);
#endif
    }
#if !UNITY_WEBGL
    private void OnDisable()
    {
#if UNITY_SERVER
        OnDeallocated(new MultiplayDeallocation("local-close", 0, "local"));
#endif
    }
#endif
    #endregion
    #region Network Callbacks
    private void OnNetworkTick()
    {
#if !UNITY_WEBGL
        if (useMultiplay)
        {
            serverQueryHandler.UpdateServerCheck();
        }
#endif
    }
#if !UNITY_WEBGL
    private async void OnAllocated(MultiplayAllocation allocation)
    {
        Debug.Log("Server allocated");
        //Start the server network handler, start update loop, allow players

        UnityTransport transport = networkManager.GetComponent<UnityTransport>();
        if (useMultiplay)
        {
            Debug.Log("Getting allocation info");
            // payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();

            //Start network handler
            Allocation relayAllocation = await RelayService.Instance.CreateAllocationAsync(playerLimit, "us-west2");
            transport.SetRelayServerData(new RelayServerData(relayAllocation, "wss"));

            //Add join_code to game_ids database
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(relayAllocation.AllocationId);
            gameID = new("server", joinCode);
            if (await supabaseClient.From<GameID>().Where(x => x.Id == "server").Single() != null)
            {
                await supabaseClient.From<GameID>().Update(gameID);
            }
            else
            {
                await supabaseClient.From<GameID>().Insert(gameID);
            }
        }

        networkManager.OnServerStopped += (x) => OnDeallocated(new("server-stopped", MultiplayService.Instance.ServerConfig.ServerId, MultiplayService.Instance.ServerConfig.AllocationId));
        networkManager.ConnectionApprovalCallback += OnConnectionApprovalRequest;
        networkManager.OnClientConnectedCallback += OnPlayerJoined;
        networkManager.OnClientDisconnectCallback += OnPlayerLeft;

        //UnityTransport.s_DriverConstructor = new ServerNetworkDriverBuilder();
        transport.SetServerSecrets(serverCertificate, serverPrivateKey);

        Debug.Log("Starting server");
        networkManager.StartServer();

        if (useMultiplay)
        {
            networkManager.NetworkTickSystem.Tick += OnNetworkTick;
        }

        //Initialize scene manager, load outside level, find spawn points
        Debug.Log("Loading scene");
        networkManager.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
        networkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        networkManager.SceneManager.LoadScene(SceneType.Outside.GetIdentifier(), LoadSceneMode.Additive);
        
        //await SceneManager.LoadSceneAsync(SceneType.Outside.GetIdentifier(), LoadSceneMode.Additive); 
        //await OnSceneLoaded();
    }
#endif
    private async void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("Scene loaded");
        networkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneType.Outside.GetIdentifier()));

        spawnLocations = FindObjectsByType<PlayerSpawnLocation>(FindObjectsSortMode.None);
        Debug.Log($"{spawnLocations.Length} Spawn locations found");

#if !UNITY_WEBGL
        if (useMultiplay)
        {
            await MultiplayService.Instance.ReadyServerForPlayersAsync();
            Debug.Log("Ready for players");

            ////Now create a backfill ticket so users can join the existing game
            //if (payloadAllocation != null)
            //{
            //    BackfillTicketProperties backfillTicketProperties = new(payloadAllocation.MatchProperties);
            //    CreateBackfillTicketOptions options = new("Default", $"{MultiplayService.Instance.ServerConfig.IpAddress}:{MultiplayService.Instance.ServerConfig.Port}", new(), backfillTicketProperties);

            //    Debug.Log("Creating backfill ticket");
            //    string ticketId = await MatchmakerService.Instance.CreateBackfillTicketAsync(options);
            //    Debug.Log("Backfill ticket created");
            //    backfillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(ticketId);
            //    backfillUpdateCoroutine = StartCoroutine(ApproveBackfillTicketUpdate());

            //    //Add join_code to game_ids database
            //}
            //else
            //{
            //    Debug.Log("No matchmaker allocation found");
            //}
        }
        Debug.Log("Creating network objects");

        NetworkObject playerListObject = Instantiate(playerListPrefab).GetComponent<NetworkObject>();
        playerListObject.Spawn();

        NetworkObject ottDisplayObject = Instantiate(ottDisplayPrefab).GetComponent<NetworkObject>();
        ottDisplayObject.Spawn();
        ottDisplay = ottDisplayObject.GetComponent<NetworkOTTDisplay>();

        Debug.Log($"Server started with limit {(useMultiplay ? serverQueryHandler.MaxPlayers : playerLimit)}");

        shutdownCoroutine = StartCoroutine(DoShutdownTimer());
#endif
    }
#if !UNITY_WEBGL
    private async void OnDeallocated(MultiplayDeallocation deallocation)
    {
        //Stop the server network handler, cease update loop, exit cleanly

        if (shutdownCoroutine != null)
        {
            StopCoroutine(shutdownCoroutine);
        }

        if (useMultiplay)
        {
            await MultiplayService.Instance.UnreadyServerAsync();
            serverQueryHandler.Dispose();

            //if (backfillTicket != null)
            //{
            //    if (backfillUpdateCoroutine != null)
            //    {
            //        StopCoroutine(backfillUpdateCoroutine);
            //    }
            //    await MatchmakerService.Instance.DeleteBackfillTicketAsync(backfillTicket.Id);
            //}

            //Remove join_code from game_ids
            try
            {
                await supabaseClient.From<GameID>().Delete(gameID);
            }
            catch (Exception ex)
            {
                Debug.Log("There was an issue deleting game ID");
                Debug.LogException(ex);
            }
        }

        if (deallocation.EventId != "server-stopped")
        {
            networkManager.Shutdown();
        }

        Debug.Log("Server shutdown achieved");
        Application.Quit();
    }
#endif
    private async void OnConnectionApprovalRequest(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        //Payload is a ConnectionRequest
        response.Pending = true;
        ConnectionRequest payload = ConnectionRequest.Deserialize(request.Payload);

        //Confirm UID and Access token with Supabase
        User user = await adminClient.GetUser(payload.accessToken);

        if (user != null && user.Id == payload.supabaseUID && !payload.isGuest)
        {
            //Check if user already joined
            if (networkPlayers.Any(x => x.supabaseUID == payload.supabaseUID))
            {
                DenyConnectionApprovalRequest(response, "User already joined");
                return;
            }

            //If correct accept request and add NetworkPlayer
            //Grab player data
            GameUser gameUser = await supabaseClient.From<GameUser>().Where(x => x.Id == user.Id).Single();
            if (gameUser == null)
            {
                DenyConnectionApprovalRequest(response, "User data was not found");
                return;
            }

            //Check kick and ban history, update if needed
            if (gameUser.IsBanned)
            {
                DenyConnectionApprovalRequest(response, "User is banned");
                return;
            }

            if (gameUser.IsKicked)
            {
                if (gameUser.KickedTimestamp == null || gameUser.KickedMinutes == null || gameUser.KickedTimestamp + TimeSpan.FromMinutes((int)gameUser.KickedMinutes) < DateTime.Now)
                {
                    gameUser.KickedMinutes = null;
                    gameUser.KickedTimestamp = null;
                    gameUser.IsKicked = false;
                    //Start update in background so we don't have to wait
                    _ = Task.Run(async () => await supabaseClient.From<GameUser>().Update(gameUser));
                }
                else
                {
                    DenyConnectionApprovalRequest(response, "User is kicked");
                    return;
                }
            }

            //If all good, create NetworkPlayer and assign spawn point
            NetworkPlayer newPlayer = NetworkPlayer.PopulateInitial(gameUser, payload.supabaseUID, request.ClientNetworkId, gameUser.Username, (PlayerAuthLevel)gameUser.AuthLevel, payload.avatarIndex, payload.authUID);
            networkPlayers.Add(newPlayer);

            ApproveConnectionApprovalRequest(response, newPlayer);
        }
        else if (payload.isGuest)
        {
            NetworkPlayer newPlayer = NetworkPlayer.PopulateInitial(null, "", request.ClientNetworkId, payload.username, PlayerAuthLevel.Guest, payload.avatarIndex, payload.authUID);
            networkPlayers.Add(newPlayer);

            ApproveConnectionApprovalRequest(response, newPlayer);
        }
        else
        {
            //Else deny the request
            DenyConnectionApprovalRequest(response, "Account UID or access token was incorrect");
        }
    }

    private IEnumerator DoShutdownTimer()
    {
        yield return null;
        bool shutdown = false;
#if !UNITY_WEBGL
        while (!shutdown)
        {
            yield return new WaitForSecondsRealtime(1);
            if (networkPlayers.Count <= 0 && useMultiplay)
            {
                shutdownElapsed += 1;
                if (shutdownElapsed >= shutdownTimer)
                {
                    shutdown = true;
                    if (networkPlayers.Count <= 0)
                    {
                        Debug.Log($"No players left after {shutdownTimer} seconds of waiting, shutting down");
                        OnDeallocated(new MultiplayDeallocation("no-players", MultiplayService.Instance.ServerConfig.ServerId, MultiplayService.Instance.ServerConfig.AllocationId));
                    }
                }
            }
        }
#endif
    }

    //private IEnumerator ApproveBackfillTicketUpdate()
    //{
    //    while (backfillTicket != null)
    //    {
    //        yield return new WaitForSecondsRealtime(1);

    //        Task<BackfillTicket> approveTask = MatchmakerService.Instance.ApproveBackfillTicketAsync(backfillTicket.Id);
    //        yield return new WaitUntil(() => approveTask.IsCompleted);
    //        backfillTicket = approveTask.Result;
    //    }
    //}

    private void DenyConnectionApprovalRequest(NetworkManager.ConnectionApprovalResponse response, string reason)
    {
        response.Approved = false;
        response.Reason = reason;
        response.Pending = false;
        Debug.Log("Connecting request denied for reason: " + reason);
    }

    private void ApproveConnectionApprovalRequest(NetworkManager.ConnectionApprovalResponse response, NetworkPlayer newPlayer)
    {
        PlayerSpawnLocation spawnPoint = null;
        if (newPlayer.authLevel is PlayerAuthLevel.Guest)
        {
            spawnPoint = spawnLocations.Where(x => x.spawnLocationType is SpawnLocationType.LoginGuest).FirstOrDefault();
        }
        else
        {
            spawnPoint = spawnLocations.Where(x => x.spawnLocationType is SpawnLocationType.LoginAccount).FirstOrDefault();
        }

        response.Position = spawnPoint.transform.position;
        response.Rotation = spawnPoint.transform.rotation;

        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;

        //Approve the connection
        response.Approved = true;
        response.Pending = false;
        Debug.Log($"{newPlayer.authLevel.ToString()} '{newPlayer.displayName}' ID '{newPlayer.playerRef.ToString()}' is joining");
    }
    #endregion
    #region Player Join/Leave
    private void OnPlayerJoined(ulong playerRef)
    {
        //Grab network player
        NetworkPlayer netPlayer = GetPlayerFromID(playerRef);

        //Grab their network player controller
        NetworkPlayerController controller = networkManager.SpawnManager.GetPlayerNetworkObject(playerRef).GetComponent<NetworkPlayerController>();
        netPlayer.playerObject = controller;

        //Spawn their avatar
        PlayerAvatar avatar = Instantiate(AvatarManager.instance.avatarList.avatarDataList[netPlayer.avatarIndex].playerAvatar);
        avatar.player = netPlayer;
        avatar.NetworkObject.SpawnAsPlayerObject(playerRef, true);
        avatar.transform.SetParent(controller.transform, false);
        netPlayer.avatar = avatar;

        netPlayer.playerObject.ClientSpawnPlayerControllerRPC(avatar.NetworkObject, ottDisplay.NetworkObject);

        if (netPlayer.authLevel is not PlayerAuthLevel.Guest)
        {
            PlayerList.instance.playerIDList.Add(playerRef);
            PlayerList.instance.playerNameList.Add(netPlayer.displayName);
        }

        Debug.Log($"{netPlayer.authLevel.ToString()} '{netPlayer.displayName}' ID '{playerRef.ToString()}' has joined");

#if UNITY_SERVER
        if (useMultiplay)
        {
            serverQueryHandler.CurrentPlayers = (ushort)networkManager.ConnectedClients.Count;
        }
#endif
    }

    private void OnPlayerLeft(ulong playerRef)
    {
        //Remove netplayer
        NetworkPlayer netPlayer = GetPlayerFromID(playerRef);

        if (netPlayer == null)
        {
            return;
        }

        if (netPlayer.sittingInteractable != null)
        {
            UnsetPlayerSittingInteractable(playerRef);
        }

        //Remove avatar
        if (netPlayer.avatar != null && netPlayer.avatar.IsSpawned)
        {
            netPlayer.avatar.NetworkObject.Despawn();
        }

        Debug.Log($"{netPlayer.authLevel.ToString()} '{netPlayer.displayName}' ID '{playerRef.ToString()}' has left");
        networkPlayers.Remove(netPlayer);

        if (netPlayer.authLevel is not PlayerAuthLevel.Guest)
        {
            PlayerList.instance.playerIDList.Remove(playerRef);
            PlayerList.instance.playerNameList.Remove(netPlayer.displayName);
        }

        if (oratorModePlayers.Contains(playerRef))
        {
            oratorModePlayers.Remove(playerRef);
            if (oratorModePlayers.Count <= 0 && ottDisplay.isActive.Value)
            {
                ottDisplay.isActive.Value = false;
            }
        }

#if UNITY_SERVER
        if (useMultiplay)
        {
            serverQueryHandler.CurrentPlayers = (ushort)networkManager.ConnectedClients.Count;
            ////Update backfill ticket
            //Player player = backfillTicket.Properties.MatchProperties.Players.Where(x => x.Id == netPlayer.authUID).FirstOrDefault();
            //backfillTicket.Properties.MatchProperties.Players.Remove(player);
            //backfillTicket.Properties.MatchProperties.Teams[0].PlayerIds.Remove(player.Id);
            //await MatchmakerService.Instance.UpdateBackfillTicketAsync(backfillTicket.Id, backfillTicket);

            if (networkPlayers.Count <= 0)
            {
                shutdownElapsed = 0;
            }
        }
#endif
    }
    #endregion
    #region Player Animation RPCs
    //public void SetAvatarAnimationTrigger(ulong playerRef, string newTrigger)
    //{
    //    NetworkPlayer player = GetPlayerFromID(playerRef);
    //    player.avatar.networkAnimator.SetTrigger(newTrigger);
    //}

    //public void ResetAvatarAnimationTrigger(ulong playerRef, string oldTrigger)
    //{
    //    NetworkPlayer player = GetPlayerFromID(playerRef);
    //    player.avatar.networkAnimator.ResetTrigger(oldTrigger);
    //}

    public void SetResetAvatarAnimationTriggers(ulong playerRef, string oldTrigger, string newTrigger)
    {
        NetworkPlayer player = GetPlayerFromID(playerRef);
        if (player == null)
        {
            return;
        }
        if (oldTrigger != string.Empty)
        {
            player.avatar.networkAnimator.ResetTrigger(oldTrigger);
        }
        player.avatar.networkAnimator.SetTrigger(newTrigger);
    }

    public void SetAnimationSpeed(ulong playerRef, StringContainer[] keys, float[] values)
    {
        NetworkPlayer netPlayer = GetPlayerFromID(playerRef);
        if (netPlayer == null)
        {
            return;
        }
        for (int i = 0; i < keys.Length; i++)
        {
            netPlayer.avatar.animator.SetFloat(keys[i].text, values[i]);
        }
    }

    public void SetPlayerSittingInteractable(ulong playerRef, NetworkInteractable netObject)
    {
        NetworkPlayer player = GetPlayerFromID(playerRef);
        if (player == null || player.authLevel is PlayerAuthLevel.Guest)
        {
            return;
        }

        if (player.sittingInteractable != netObject)
        {
            UnsetPlayerSittingInteractable(playerRef);
        }

        player.sittingInteractable = netObject;
        player.sittingInteractable.SetCanInteract(false);
    }

    public void UnsetPlayerSittingInteractable(ulong playerRef)
    {
        NetworkPlayer player = GetPlayerFromID(playerRef);
        if (player == null || player.sittingInteractable == null)
        {
            return;
        }

        player.sittingInteractable.SetCanInteract(true);
        player.sittingInteractable = null;
    }
    #endregion
    #region Chat Message RPCs
    public void PlayerOratorModeToggled(ulong playerRef, bool toggled)
    {
        if (!IsAdmin(playerRef))
        {
            return;
        }

        if (toggled)
        {
            oratorModePlayers.Add(playerRef);
            if (!ottDisplay.isActive.Value)
            {
                ottDisplay.isActive.Value = true;
            }
        }
        else if (oratorModePlayers.Contains(playerRef))
        {
            oratorModePlayers.Remove(playerRef);
            if (oratorModePlayers.Count <= 0 && ottDisplay.isActive.Value)
            {
                ottDisplay.isActive.Value = false;
            }
        }
    }

    public void SendChatMessage(ulong sender, ChatLogMessage message)
    {
        NetworkPlayer senderPlayer = GetPlayerFromID(sender);

        if (message.messageType is ChatMessageType.Shout || message.messageType is ChatMessageType.Orator)
        {
            if (senderPlayer.authLevel < PlayerAuthLevel.Admin && !senderPlayer.isMuted)
            {
                SendWhisperMessage(0, sender, ChatLogMessage.FromServer("Only orators and admins can shout"));
                return;
            }

            if (message.messageType is ChatMessageType.Orator)
            {
                ottDisplay.currentMessage.Value = message.message;
            }

            foreach (NetworkPlayer recipient in instance.networkPlayers)
            {
                recipient.playerObject.ClientRecieveMessageRPC(message);
            }
            return;
        }

        if (senderPlayer.isMuted)
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("You are muted and cannot message"));
            return;
        }

        if (senderPlayer.authLevel is PlayerAuthLevel.Guest)
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("Sorry, guests cannot message"));
            return;
        }

        message.SetID(currentID);
        currentID++;

        if (senderPlayer.authLevel > PlayerAuthLevel.User)
        {
            string[] splitMessage = message.message.ToString().Split(":");
            message.message = new FixedString512Bytes("<color=red>" + splitMessage[0] + "</color>: " + string.Join(":", splitMessage, 1, splitMessage.Length - 1));
        }

        foreach (NetworkPlayer recipient in instance.networkPlayers
            .Where(x => Vector3.Distance(x.avatar.transform.position, senderPlayer.avatar.transform.position) < localTalkRange))
        {
            recipient.playerObject.ClientRecieveMessageRPC(message);
        }
    }

    public void SendWhisperMessage(ulong sender, ulong recipient, ChatLogMessage message, string recipientName = "")
    {
        Debug.Log("sending message");
        NetworkPlayer senderPlayer = GetPlayerFromID(sender);

        //Check recipient exists
        NetworkPlayer recipientPlayer = GetPlayerFromID(recipient);
        if (recipientPlayer == null)
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer($"Recipient {recipientName} isn't valid"));
            return;
        }

        //Check if sender is muted
        if (sender != 0 && senderPlayer.isMuted)
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("You are muted and cannot message"));
            return;
        }

        //Check if sender is guest
        if (sender != 0 && senderPlayer.authLevel < PlayerAuthLevel.Guest)
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("Sorry, guests cannot message"));
            return;
        }

        message.SetID(currentID);
        currentID++;

        string senderName = (sender != 0 && senderPlayer.authLevel > PlayerAuthLevel.User ? "<color=red>" : "")
            + (sender != 0 ? senderPlayer.displayName : "Server")
            + (sender != 0 && senderPlayer.authLevel > PlayerAuthLevel.User ? "</color>" : "");


        recipientName = (recipient != 0 && recipientPlayer.authLevel > PlayerAuthLevel.User ? "<color=red>" : "")
       + (recipient != 0 ? recipientPlayer.displayName : "Server")
       + (recipient != 0 && recipientPlayer.authLevel > PlayerAuthLevel.User ? "</color>" : "");

        Debug.Log(recipientName);

        string recipientMessage = $"Whisper from {senderName}: {message.message}";
        string senderMessage = $"Whisper to {recipientName}: {message.message}";

        // string recipientMessage = $"Whisper fradasdadom {senderName}: {message.message}";
        // string senderMessage = $"Whispeasdadasdasdadr to {senderName}: {message.message}";



        message.message = recipientMessage;
        recipientPlayer.playerObject.ClientRecieveMessageRPC(message);

        if (sender != 0)
        {
            ChatLogMessage senderChatMessage = new ChatLogMessage(-1, ChatMessageType.Whisper, senderPlayer.authLevel, recipientPlayer.avatarIndex, senderMessage);
            senderPlayer.playerObject.ClientRecieveMessageRPC(senderChatMessage);
        }
    }
    #endregion
    #region Helpers
    public NetworkPlayer GetPlayerFromID(ulong id) => networkPlayers.FirstOrDefault(x => x.playerRef == id);
    public NetworkPlayer GetPlayerFromName(string name) => networkPlayers.FirstOrDefault(x => x.displayName == name);

    private void ErrorShutdown(string error)
    {
        Debug.LogError(error);
        Application.Quit();
    }

    private string GetCommandLineArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
    #endregion
    #region User Management
    public async void DeleteUser(ulong playerRef)
    {
        NetworkPlayer netPlayer = GetPlayerFromID(playerRef);
        networkManager.DisconnectClient(playerRef, "Account deleted successfully");
        await adminClient.DeleteUser(netPlayer.supabaseUID);
        await supabaseClient.From<GameUser>().Where(x => x.Id == netPlayer.supabaseUID).Delete();
        await supabaseClient.From<GameUsername>().Where(x => x.Id == netPlayer.supabaseUID).Delete();
    }
    #endregion
    #region Admin Toolbox
    private bool IsAdmin(ulong playerRef)
    {
        NetworkPlayer player = GetPlayerFromID(playerRef);
        if (player == null)
        {
            return false;
        }

        return player.authLevel > PlayerAuthLevel.User;
    }

    public void MutePlayer(ulong sender, ulong target)
    {
        if (!IsAdmin(sender))
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("Only admins can mute another player"));
            return;
        }

        if (IsAdmin(target))
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("You can't mute another admin"));
            return;
        }

        NetworkPlayer targetPlayer = GetPlayerFromID(target);
        targetPlayer.isMuted = true;
        targetPlayer.playerObject.ClientGetMutedRPC();

        SendWhisperMessage(0, sender, ChatLogMessage.FromServer($"User {targetPlayer.displayName} has been muted by you"));
        SendWhisperMessage(0, target, ChatLogMessage.FromServer("You have been muted by an admin"));
    }

    public void KickPlayer(ulong sender, ulong target)
    {
        if (!IsAdmin(sender))
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("Only admins can kick another player"));
            return;
        }

        if (IsAdmin(target))
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("You can't kick another admin"));
            return;
        }

        Debug.Log(target);

        //Update Supabase user
        NetworkPlayer targetPlayer = GetPlayerFromID(target);
        targetPlayer.gameUser.IsKicked = true;
        targetPlayer.gameUser.KickedMinutes = 10;
        targetPlayer.gameUser.KickedTimestamp = DateTime.Now;

        supabaseClient.From<GameUser>().Update(targetPlayer.gameUser);

        networkManager.DisconnectClient(target, "You were kicked by an admin");

        SendWhisperMessage(0, sender, ChatLogMessage.FromServer($"User {targetPlayer.displayName} has been kicked by you for 10 minutes"));
    }

    public void BanPlayer(ulong sender, ulong target)
    {
        if (!IsAdmin(sender))
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("Only admins can ban another player"));
            return;
        }

        if (IsAdmin(target))
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("You can't ban another admin"));
            return;
        }

        NetworkPlayer targetPlayer = GetPlayerFromID(target);
        targetPlayer.gameUser.IsBanned = true;

        supabaseClient.From<GameUser>().Update(targetPlayer.gameUser);

        networkManager.DisconnectClient(target, "You were banned by an admin");

        SendWhisperMessage(0, sender, ChatLogMessage.FromServer($"User {targetPlayer.displayName} has been banned by you"));
    }

    public void ToggleVisibility(ulong sender, bool visible)
    {
        if (!IsAdmin(sender))
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("Only admins can become invisible"));
            return;
        }

        NetworkPlayer netPlayer = GetPlayerFromID(sender);
        netPlayer.isHidden = !visible;
        SendWhisperMessage(0, sender, ChatLogMessage.FromServer($"Toggled visibility to {visible}"));
    }

    public void Teleport(ulong sender, Vector3 position, Quaternion rotation, string locationName)
    {
        if (!IsAdmin(sender))
        {
            SendWhisperMessage(0, sender, ChatLogMessage.FromServer("Only admins can teleport"));
            return;
        }

        NetworkPlayer netPlayer = GetPlayerFromID(sender);
        netPlayer.playerObject.ClientRecieveTeleportRPC(position, rotation);
        SendWhisperMessage(0, sender, ChatLogMessage.FromServer($"Teleported you to {locationName}"));
    }
    #endregion
}
