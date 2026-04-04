# Unity Client-Server Mock — Task Project

**Author:** Norven Ephraim C. Caracas

A take-home technical task simulating how a client and server communicate in a game without any real networking. Everything runs inside one Unity scene using a fake packet system with simulated latency.

---

## Project Structure

```
Assets/Test Tasks/
├── Editable/               — Scripts I wrote and modified
│   ├── ClientMobsManager.cs
│   ├── ClientColors.cs
│   ├── ClientPacketsHandler.cs
│   ├── PacketHandlerLookup.cs
│   ├── ServerMobsManager.cs
│   ├── ServerPacketsHandler.cs
│   ├── UIMobsDisplay.cs
│   └── UIColorDisplay.cs
└── Non-Editable/           — Read-only infrastructure provided with the project
    ├── ClientManager.cs
    ├── ServerMock.cs
    ├── MonsterData.cs
    ├── MonsterNameExtensions.cs
    ├── Packet.cs
    ├── PacketLatencyMock.cs
    ├── PacketReceiverClient.cs
    ├── PacketReceiverServer.cs
    ├── PacketSenderClient.cs
    ├── PacketSenderServer.cs
    └── ServerColors.cs
```

---

## Packet ID Reference

**Client sends to Server:**

| ID  | What it does           |
| --- | ---------------------- |
| 1   | Login request          |
| 2   | Request monster data   |
| 3   | Deal damage to monster |
| 4   | Request color list     |

**Server sends to Client:**

| ID  | What it does          | Data inside                                                  |
| --- | --------------------- | ------------------------------------------------------------ |
| 1   | Login response        | int status, int clientId                                     |
| 2   | Monster data          | int monsterId, int type, float maxHp, float currentHp        |
| 3   | Monster health update | int monsterId, float healthRatio                             |
| 4   | Color list            | int count, then float r, float g, float b, float a per color |

---

## Network Flow

### Client → Server

```
ClientPacketsHandler writes data into a new Packet (e.g. new Packet(1))
    → PacketSenderClient.SendToServer(packet)
        → PacketLatencyMock.EnqueueMessage(bytes)
            → PacketLatencyMock.FixedUpdate() waits until delay passes
                → PacketSenderClient.Send(bytes)
                    → OnSendToServer static event fires
                        → PacketReceiverServer.HandleServerMessage(bytes)
                            → PacketHandlerLookup.OnServerPacketHandlers[id]
                                → Matching handler in ServerPacketsHandler is called
```

### Server → Client

```
ServerPacketsHandler writes data into a new Packet (e.g. new Packet(1))
    → PacketSenderServer.SendToClient(packet)
        → PacketLatencyMock.EnqueueMessage(bytes)
            → PacketLatencyMock.FixedUpdate() waits until delay passes
                → PacketSenderServer.Send(bytes)
                    → OnSendToClient static event fires
                        → PacketReceiverClient.HandleClientMessage(bytes)
                            → PacketHandlerLookup.OnClientPacketHandlers[id]
                                → Matching handler in ClientPacketsHandler is called
                                    → ClientPacketsHandler calls the right method
                                        on ClientMobsManager or ClientColors
```

---

## Features

### 1. Login

**Step by step:**

1. Player clicks Login button → `ClientManager.LogIn()` is called
2. `ClientPacketsHandler.SendLoginRequest()` creates packet id=1 and sends it
3. `ServerPacketsHandler.LoginRequest(packet)` receives it and calls `ServerMock.Instance.TryConnectClient(out clientId)`
4. `ServerPacketsHandler.SendLoginResponse(response, clientId)` writes status code and client ID into packet id=1 and sends it back
5. `ClientPacketsHandler.LoginDataReceived(packet)` reads the response and calls `ClientManager.Instance.SetClientLogInStatus(responseCode, clientId)`
6. `ClientManager` stores the client ID and fires `ClientLogInStatusChanged` event
7. If login succeeded, `SendMonsterRequest()` is called immediately

---

### 2. Monster Spawn and Health Display

**Step by step:**

1. `ClientPacketsHandler.SendMonsterRequest()` creates packet id=2 and sends it
2. `ServerPacketsHandler.MonsterRequest(packet)` receives it and calls `ServerMock.Instance.ServerMobsManager.SendMonsterToClient()`
3. `ServerMobsManager.SendMonsterToClient()` calls `ServerPacketsHandler.SendMonsterSpawn(MonsterData)`
4. `SendMonsterSpawn` writes `MonsterId`, `MonsterType`, `MonsterMaxHealth`, `MonsterCurrentHealth` into packet id=2 and sends it
5. `ClientPacketsHandler.MonsterDataReceived(packet)` reads all four values and creates a local `MonsterData` object
6. `ClientMobsManager.OnMonsterSpawn(monsterData)` stores it as `currentMonster` and fires `OnMonsterSpawned` event
7. `UIMobsDisplay` is subscribed via `OnEnable` — `InitializeMonsterUI(monster)` sets the sprite, name, and HP bar

---

### 3. Monster Damage

**Step by step:**

1. Player clicks Damage Monster button → `ClientMobsManager.DamageMonster()` is called
2. `Mathf.Round(Random.Range(20f, 50f))` picks a random damage value rounded to a whole number
3. `ClientPacketsHandler.SendDamageRequest(monsterId, damage)` writes the monster ID and damage into packet id=3 and sends it
4. `ServerPacketsHandler.DamageRequest(packet)` reads `monsterId` and `damage` — if the monster ID does not match the current server monster the packet is ignored
5. `monsterData.TakeDamage(damage)` subtracts HP and fires two events internally:
   - `MonsterDamaged` fires with the current HP as a ratio
   - `MonsterDeath` fires if HP hits 0 or below
6. `ServerMobsManager.OnMonsterDamaged(healthRatio)` calls `ServerPacketsHandler.SendMonsterHealthUpdate(MonsterData.MonsterId, healthRatio)`
7. `SendMonsterHealthUpdate` writes the monster ID and health ratio into packet id=3 and sends it
8. `ClientPacketsHandler.MonsterHealthUpdateReceived(packet)` reads both values and calls `ClientMobsManager.UpdateMonsterHealth(monsterId, healthRatio)`
9. `UpdateMonsterHealth` validates the monster ID then fires `OnMonsterHealthChanged`
10. `UIMobsDisplay.UpdateHealthBar(healthRatio)` sets `monsterHpBar.value`

**When the monster dies:**

1. `MonsterDeath` fires on the server's `MonsterData`
2. `ServerMobsManager.OnMonsterDied()` unsubscribes from events, spawns a new `MonsterData`, and calls `SendMonsterToClient()`
3. Client receives the new monster via `MonsterDataReceived` and the whole cycle repeats

---

### 4. Color Request

**Step by step:**

1. Player clicks Request Colors button → `ClientColors.RequestColors()` is called
2. `isLoggedIn` is checked — if false the request is blocked with a warning
3. `ClientPacketsHandler.SendColorRequest()` creates packet id=4 and sends it
4. `ServerPacketsHandler.ColorRequest(packet)` calls `ServerMock.Instance.ServerColors.GetServerColors().ToList()`
5. `SendColors(colors)` writes the count then each color as four floats (r, g, b, a) into packet id=4 and sends it
6. `ClientPacketsHandler.ColorsDataReceived(packet)` reads the count then loops and reads each color
7. `ClientColors.ReceiveColors(colors)` stores the list and fires `OnColorsReceived` event
8. `UIColorDisplay.InitializeColorUI(colors)` instantiates a color prefab for each color and sets its image color

---

## Architecture

### Client and Server are fully separated

The client side and server side never call each other directly. All communication happens exclusively through packets sent over the mock network.

### Packet-based communication

Every feature follows the same pattern — client sends a request packet, server processes it and sends a response packet back. `PacketHandlerLookup` maps packet IDs to handler functions. `ClientPacketsHandler` and `ServerPacketsHandler` are the only scripts that read from and write to packets.

### Data and UI are separated

Data scripts (`ClientMobsManager`, `ClientColors`) own game state and fire static C# events when something changes. UI scripts (`UIMobsDisplay`, `UIColorDisplay`) subscribe to those events and update visuals.

```
ClientMobsManager fires OnMonsterSpawned ────────► UIMobsDisplay.InitializeMonsterUI()
ClientMobsManager fires OnMonsterHealthChanged ──► UIMobsDisplay.UpdateHealthBar()
ClientColors fires OnColorsReceived ─────────────► UIColorDisplay.InitializeColorUI()
```

---

## Scene Setup

| Button         | Calls                               |
| -------------- | ----------------------------------- |
| Login          | `ClientManager.LogIn()`             |
| Damage Monster | `ClientMobsManager.DamageMonster()` |
| Request Colors | `ClientColors.RequestColors()`      |

**Monster display — wired to `UIMobsDisplay`:**

- `MonsterPanel` — holds the monster sprite Image
- `ContainerTitle` — holds the monster name TextMeshPro
- `HealthBarSlider` — holds the HP bar Slider
- Monster sprites are assigned as a serialized array in the Inspector, ordered to match the `MonsterNames` enum (Goblin, Troll, Dragon, Skeleton, Orc)

**Color display — wired to `UIColorDisplay`:**

- `ColorScrollView` — parent ScrollRect that handles horizontal scrolling
- `ColorPanel` — sits inside the Viewport, acts as the content container with a Horizontal Layout Group
- `ColorPrefab` — individual color square instantiated at runtime, one per color received
