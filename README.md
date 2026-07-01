# Darshfish

.NET / C# chess engine compiled to WebAssembly and consumed from the frontend via
two separate Blazor WASM builds, each running in its own Web Worker.

## Bots

| Bot | Method | Behavior |
|---|---|---|
| darshfish v1 (Random) | `ChessEngine.GetRandomMove(fen)` | Returns a random legal move |
| darshfish v2 (Basic Search) | `ChessEngine.GetBestMove(fen)` | Returns the best move found via search |

Both methods live on `chessEngine.ChessEngine` and take a FEN string as input,
returning a move string (e.g. `"e2e4"`).

## Project structure

```
chessEngine/
├── chessEngine.csproj
├── ChessEngine.cs        # GetRandomMove, GetBestMove
└── ...
```

## Build / Publish

The frontend expects **two separate published outputs**, since v1 and v2 run as
independent WASM runtimes in separate Web Workers:

- v1 → served at `/_framework/`
- v2 → served at `/_framework-v2/`

Publish (not build) in Release mode so AOT/trimming settings actually apply:

```bash
dotnet publish -c Release -r browser-wasm --self-contained
```

Copy/rename the resulting `wwwroot/_framework` output to the appropriate
`_framework` or `_framework-v2` folder in the frontend's `public/` directory
depending on which bot build this is.

Requires the `wasm-tools` workload:

```bash
dotnet workload install wasm-tools
```

## Frontend integration

Each build is loaded in a dedicated Web Worker (`/dotnet-worker-v1.js`,
`/dotnet-worker-v2.js`) rather than on the main thread, so loading the .NET
runtime doesn't freeze the UI. See `src/api/BotMoveSelector.tsx` for the
worker setup and message-passing (`postMessage`/`onmessage`) used to call
into these methods from React.

Both workers are preloaded at app startup (`main.tsx`) with a warmup call on
the starting position, so the runtime is already loaded by the time a user
picks a bot and makes a move.

## Adding a new bot / engine version

1. Add the new method to `ChessEngine.cs` (or a new class).
2. Publish to a new `_framework-vN` folder.
3. Add `/dotnet-worker-vN.js` mirroring the existing worker files, pointing
   at `_framework-vN`.
4. Add a `getDotnetWorkerVN` / `getXxxMoveVN` pair in `BotMoveSelector.tsx`.
5. Add a branch for the new bot name in `SelectMove`.
6. Add a warmup call in `main.tsx`.
