To compile to wasm
```dotnet workload install wasm-tools```
```dotnet publish -c Release -r browser-wasm --self-contained```