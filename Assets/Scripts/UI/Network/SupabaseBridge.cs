using Supabase;
using Supabase.Gotrue;
using Supabase.Postgrest.Responses;
using Supabase.Postgrest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.UI;
using Client = Supabase.Client;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Matchmaker;
using System.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.TLS;

public class SupabaseBridge : TaskProcessor
{
    private static readonly string supabaseURL = "https://kgyimaymtoepvyfcyfsd.supabase.co";
    private static readonly string supabaseTestKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtneWltYXltdG9lcHZ5ZmN5ZnNkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MjgyOTgxMzMsImV4cCI6MjA0Mzg3NDEzM30.aT5Rgl7WO_tUr7bfRsVLXEH1pLxo0cP5dD2cJjyg7lk";

    private static readonly string clientCertificate =
"-----BEGIN CERTIFICATE-----\nMIIDszCCApugAwIBAgIUCjTu4m1VFgkqVsHMUxiczT0nWtIwDQYJKoZIhvcNAQEL\nBQAwaTELMAkGA1UEBhMCVUsxFjAUBgNVBAgMDVdlc3QgTWlkbGFuZHMxFDASBgNV\nBAcMC1N0b3VyYnJpZGdlMRcwFQYDVQQKDA5BenVsb24gU3R1ZGlvczETMBEGA1UE\nAwwKSG9BX1NlcnZlcjAeFw0yNDEwMTIxMDM1NDZaFw0yNzEwMTIxMDM1NDZaMGkx\nCzAJBgNVBAYTAlVLMRYwFAYDVQQIDA1XZXN0IE1pZGxhbmRzMRQwEgYDVQQHDAtT\ndG91cmJyaWRnZTEXMBUGA1UECgwOQXp1bG9uIFN0dWRpb3MxEzARBgNVBAMMCkhv\nQV9TZXJ2ZXIwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDULBgBk7b3\nvnQEPaPN9YsZYVaRHi4dH3mb2HA6k0H730N2a+OkXVAHqg+4QlGtJKuYjOSOuNjx\nIMlI9z3cMcWUpi1gtX4QjGWeDVgadN5VEepqWq1tivba9DKqM2j1Pe7f6fRmUPbJ\nRH4pkk9txCmvQILvmZkCbdHx6PGyLsQAkRJsxHldfEoUUIgrmZKBgp4MLng3p1im\n+e/zD987OxXjn2iS6nx1N9RzXG7SOLHiZj8jr8a5pZPIbXltZ0vQeoheQc+zojxC\n1MxbX01NQJ/5VCXYnOyFiWMfloj1zuBW5MRQdrKdw8bGzB85RVkAMXvorW89Lb+D\nIiC1zpsqvPa9AgMBAAGjUzBRMB0GA1UdDgQWBBSg9GIAeWEmFN1qu5jYarE5I0mr\n9DAfBgNVHSMEGDAWgBSg9GIAeWEmFN1qu5jYarE5I0mr9DAPBgNVHRMBAf8EBTAD\nAQH/MA0GCSqGSIb3DQEBCwUAA4IBAQBhFZ1HAwZpMmyhnO2QGAQ6ja/XxOlXe/Ir\niundsqpYJXOTn19ERIqRbiTLhQcYEixLJuShY90s5IKa+B+Oz9aM8WAJnU8FHghy\nSsCNCnE5v/GjuEH+Lsugz1H6BCxGeah082Lg/dWo+ekWZnt+ePprme725+YvMLy/\nHUFBAgIG/QjQFCsothX0yPW7dJgwrQUGaZdf/a+qq8qGqzy6z7Cb4BBpsjY3rEaH\nWD1FUZpGIURs+eko0lX5vV8V9uB/hkQejXHGPvigke6cOB+KwjGe4ICxxLer8DdC\nBZn75FGJOVYrUwtlnD44Xq44p8qUorVKRxFguPZjbcuDgaAtJOXP\n-----END CERTIFICATE-----";

    private NetworkManager networkManager;

    [SerializeField] private NetworkManager localNetworkManager;
    [SerializeField] private NetworkManager relayNetworkManager;

    public static SupabaseBridge instance;

    private Client supabaseClient;
    public Client SupabaseClient
    {
        get
        {
            if (supabaseClient == null)
            {
                //Create and initialize the client
                supabaseClient = new Client(supabaseURL, supabaseTestKey);
                supabaseClient.InitializeAsync();
            }

            return supabaseClient;
        }
    }

    private bool tryingLocal = false;
    private bool tryingRelay = false;
    private bool relaySuccess = false;

    private void Awake()
    {
        instance = this;
    }

    public async Task<GameUser> GetGameUser(Session session)
    {
        GameUser user = await SupabaseClient
            .From<GameUser>()
            .Select("*")
            .Where(x => x.Id == session.User.Id)
            .Single();
        if (user == null)
        {
            //Try to create user data using uid and cached username
            GameUsername username = await SupabaseClient
                .From<GameUsername>()
                .Select("username")
                .Where(x => x.Id == session.User.Id)
                .Single();
            if (username != null)
            {
                Debug.Log("No user data found, creating new user data");
                user = await CreateGameUser(session, username.Username, (int)PlayerAuthLevel.User);
            }
        }
        return user;
    }

    public async Task<GameUser> CreateGameUser(Session session, string displayName, int authLevel)
    {
        GameUser user = new(session.User.Id, displayName, authLevel, false, false, null, null, session.User.CreatedAt);
        return (await SupabaseClient
            .From<GameUser>()
            .Insert(user, new QueryOptions { Returning = QueryOptions.ReturnType.Representation }))
            .Model;
    }

    public async Task<bool> IsUsernameTaken(string username)
    {
        return await SupabaseClient
            .From<GameUsername>()
            .Where(x => x.Username == username)
            .Count(Supabase.Postgrest.Constants.CountType.Exact) > 0;
    }

    private async void Shutdown()
    {
        Debug.Log("Shutting down");
        //If in a server, leave
        if (networkManager.IsConnectedClient)
        {
            networkManager.OnConnectionEvent -= OnConnectionEvent;
            networkManager.OnTransportFailure -= OnTransportFailure;
            networkManager.Shutdown();
        }

        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut();
            }
        }
        catch (Exception) { }

        //If anonymously logged in, destroy that gamedata
        if (supabaseClient != null && SupabaseClient.Auth.CurrentUser != null)
        {
            if (SupabaseClient.Auth.CurrentUser.IsAnonymous)
            {
                await SupabaseClient.Rpc("Delete user data", new Dictionary<string, object>() { { "user_uid", SupabaseClient.Auth.CurrentUser.Id } });
            }
            await SupabaseClient.Auth.SignOut();
            Debug.Log("User signed out");

            PlayerSettingsManager.instance.gameUser.value = null;
            PlayerSettingsManager.instance.accountGuid.value = "";
        }

        if (!SceneController.inGame)
        {
            SceneController.instance.CancelLoad();
        }
        SceneController.instance.LoadMainMenu_FromGameScene(StartupScreenType.MainMenu);
    }

    public void LeaveServer()
    {
        Shutdown();
    }

    public void OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEvent)
    {
        if (connectionEvent.ClientId == networkManager.LocalClientId)
        {
            if (connectionEvent.EventType is ConnectionEvent.ClientDisconnected)
            {
                if (!tryingLocal && !tryingRelay)
                {
                    Shutdown();
                }
                tryingLocal = false;
                tryingRelay = false;
                relaySuccess = false;
            }
            else if (connectionEvent.EventType is ConnectionEvent.ClientConnected)
            {
                if (tryingLocal)
                {
                    tryingLocal = false;
                }
                if (tryingRelay)
                {
                    tryingRelay = false;
                    relaySuccess = true;
                }
            }
        }
    }

    public void OnTransportFailure()
    {
        if (tryingRelay)
        {
            tryingRelay = false;
            relaySuccess = false;
        }
        else
        {
            Shutdown();
        }
    }

    public async Task JoinServer()
    {
        Debug.Log("entering...");
        //Connect to the server
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Debug.Log("Checking and clearing invalid game_id entries...");
        try
        {
            var gameIdResponse = await SupabaseClient.From<GameID>()
                .Select("*")
                .Where(x => x.Id == "server")
                .Single();

            if (gameIdResponse == null || string.IsNullOrEmpty(gameIdResponse.JoinCode))
            {
                Debug.Log("No valid game_id found. Proceeding to matchmaking.");
            }
            else
            {
                Debug.Log("Clearing invalid game_id...");
                await SupabaseClient.From<GameID>().Filter("Id", Supabase.Postgrest.Constants.Operator.Equals, "server").Delete();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error clearing game_id: {e.Message}. Proceeding to matchmaking anyway.");
        }

        ConnectionRequest request = new ConnectionRequest(
            PlayerSettingsManager.instance.gameUser.value.Username,
            PlayerSettingsManager.instance.authLevel.value is PlayerAuthLevel.Guest ? PlayerSettingsManager.instance.accountGuid.value : PlayerSettingsManager.instance.accountGuid.value,
            supabaseClient.Auth.CurrentSession.AccessToken,
            PlayerSettingsManager.instance.playerAvatarDataIndex.value,
            AuthenticationService.Instance.PlayerId,
            PlayerSettingsManager.instance.authLevel.value is PlayerAuthLevel.Guest);

        //Join locally :)
        //UnityTransport.s_DriverConstructor = new ClientNetworkDriverBuilder();
        networkManager = localNetworkManager;
        networkManager.SetSingleton();
        networkManager.NetworkConfig.ConnectionData = request.Serialize();

        UnityTransport transport = networkManager.GetComponent<UnityTransport>();
        transport.SetClientSecrets("HoA_Server", clientCertificate);
        transport.UseWebSockets = true;

        networkManager.OnConnectionEvent += OnConnectionEvent;
        tryingLocal = true;
        bool result = networkManager.StartClient();
        while (result && networkManager.IsClient && tryingLocal)
        {
            await Awaitable.FixedUpdateAsync();
        }

        if (!networkManager.IsClient)
        {
            networkManager.OnConnectionEvent -= OnConnectionEvent;
            networkManager = relayNetworkManager;
            networkManager.SetSingleton();

            networkManager.NetworkConfig.ConnectionData = request.Serialize();

            transport = networkManager.GetComponent<UnityTransport>();
            transport.SetClientSecrets("HoA_Server", clientCertificate);
            transport.UseWebSockets = true;

            networkManager.OnConnectionEvent += OnConnectionEvent;

            GameID gameId = await supabaseClient.From<GameID>().Where(x => x.Id == "server").Single();

            if (gameId == null)  //Only matchmake if there isn't a game id
            {
                Debug.Log("Creating matchmaking ticket");
                //Matchmake and find server
                List<Player> players = new()
                {
                    new Player(AuthenticationService.Instance.PlayerId, new Dictionary<string, object>())
                };

                CreateTicketOptions options = new("Default", new Dictionary<string, object>());

                CreateTicketResponse ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);

                MultiplayAssignment multiplayAssignment = null;
                bool gotAssignment = false;

                do
                {
                    //Rate limit delay
                    Debug.Log("Checking matchmaking ticket");
                    await Awaitable.WaitForSecondsAsync(1);

                    // Poll ticket
                    var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(ticketResponse.Id);
                    if (ticketStatus == null)
                    {
                        continue;
                    }

                    //Convert to platform assignment data (IOneOf conversion)
                    if (ticketStatus.Type == typeof(MultiplayAssignment))
                    {
                        multiplayAssignment = ticketStatus.Value as MultiplayAssignment;
                    }

                    switch (multiplayAssignment?.Status)
                    {
                        case MultiplayAssignment.StatusOptions.Found:
                            gotAssignment = true;
                            break;
                        case MultiplayAssignment.StatusOptions.InProgress:
                            //...
                            break;
                        case MultiplayAssignment.StatusOptions.Failed:
                            gotAssignment = true;
                            Debug.LogError("Failed to get ticket status. Error: " + multiplayAssignment.Message);
                            throw new Exception($"Matchmaking error: {multiplayAssignment.Message}");
                        case MultiplayAssignment.StatusOptions.Timeout:
                            gotAssignment = true;
                            Debug.LogError("Failed to get ticket status. Ticket timed out.");
                            throw new Exception("Ticket timed out");
                        default:
                            throw new InvalidOperationException();
                    }

                } while (!gotAssignment); //Check at the end otherwise this will run again

                if (multiplayAssignment == null)
                {
                    throw new Exception("No match found, server must be offline");
                }
            }

            //Join multiplay server
            Debug.Log("Match found");

            //Check if a join code exists, if not something went wrong
            gameId = await supabaseClient.From<GameID>().Where(x => x.Id == "server").Single();

            networkManager.OnTransportFailure += OnTransportFailure;

            int attemptsLeft = 5;
            while (attemptsLeft > 0)
            {
                Debug.Log($"Attempting to connect to relay, {attemptsLeft} tries left");
                //transport.SetConnectionData(multiplayAssignment.Ip, (ushort)multiplayAssignment.Port);
                tryingRelay = true;
                JoinAllocation relayAllocation = await RelayService.Instance.JoinAllocationAsync(gameId.JoinCode);
                transport.SetRelayServerData(new RelayServerData(relayAllocation, "wss"));
                transport.UseWebSockets = true;
                networkManager.StartClient();
                while (tryingRelay)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                }

                if (relaySuccess)
                {
                    attemptsLeft = 0;
                }
                else
                {
                    attemptsLeft--;
                }
            }

            if (!networkManager.IsClient)
            {
                throw new Exception(networkManager.DisconnectReason);
            }
        }

        //Loading screen and essential scenes
        SceneController.instance.LoadGameScene_FromMainMenu(SceneType.Outside);
    }
}
