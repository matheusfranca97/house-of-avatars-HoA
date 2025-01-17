# Notes on the server architecture
When the client connects to the main server this is what happens:
- Client sends encrypted user and password to Firebase auth server
- Auth server responds with information (success, fail)
- On success the client is given a token and is sent to avatar selection
- When the client selects an avatar it joins the game server with the token
- The game server sends the token to Firebase auth to check the token and ip is correct
- Firebase auth responds to the game server (success, fail)
- On success the game server accepts the client and the client will start loading
- The game server sends a player joined message to other clients
- Other clients spawn the network player for the new player
- The original client recieves the player list and creates the network players in it's own scene for syncing
- The client has connected