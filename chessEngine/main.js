import { dotnet } from './_framework/dotnet.js';

const { getAssemblyExports, getConfig } = await dotnet
    .withApplicationArguments()
    .create();

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);