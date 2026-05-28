import { dotnet } from './_framework/dotnet.js';

const { getAssemblyExports, getConfig } = await dotnet
    .withApplicationArguments()
    .create();

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

export function getMove(fen) {
    return exports.chessEngine.ChessEngine.GetRandomMove(fen);
}