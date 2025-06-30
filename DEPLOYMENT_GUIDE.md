# Deployment con file pubblicati

## Passo 1: Pubblica il progetto localmente

Esegui questo comando nella directory principale del progetto:

```bash
dotnet publish Tesi.Blazor/Tesi.Blazor.Server/Tesi.Blazor.Server.csproj -c Release -o ./publish
```

Poi crea un file ZIP della cartella publish (IMPORTANTE: comprimi il contenuto, non la cartella stessa):

```bash
# Su Windows (PowerShell) - Comprimi il CONTENUTO della cartella
Compress-Archive -Path ./publish/* -DestinationPath ./publish.zip

# Su Linux/Mac - Comprimi il CONTENUTO della cartella
cd publish && zip -r ../publish.zip . && cd ..

# Verifica che il ZIP contenga i file direttamente nella root:
# Windows PowerShell:
Expand-Archive -Path ./publish.zip -DestinationPath ./test-extract -Force
Get-ChildItem ./test-extract
# Dovresti vedere Tesi.Blazor.Server.dll direttamente, non in una sottocartella

# Linux/Mac:
unzip -l publish.zip | head -20
# Dovresti vedere i file senza prefisso di cartella
```

## Passo 2: Prepara i file per il server

Crea una cartella sul server con questa struttura:
```
server-deployment/
├── Dockerfile (rinomina Dockerfile.published in Dockerfile)
├── gurobi.prod.lic (se necessario)
└── publish.zip (file ZIP con tutti i file pubblicati)
```

## Passo 3: Carica i file sul server

1. Copia il file `publish.zip` (molto più facile da trasferire!)
2. Copia il file `Dockerfile.published` e rinominalo in `Dockerfile`
3. Copia il file `gurobi.prod.lic` se necessario

## Passo 4: Build e run sul server

```bash
# Build dell'immagine Docker
docker build -t tesi-blazor .

# Run del container
docker run -p 8080:8080 tesi-blazor

# Oppure in background
docker run -d -p 8080:8080 --name tesi-app tesi-blazor
```

## Passo 5: Con variabili d'ambiente (opzionale)

```bash
docker run -d -p 8080:8080 \
  -e "ASPNETCORE_ENVIRONMENT=Production" \
  -e "ConnectionStrings__DefaultConnection=..." \
  --name tesi-app \
  tesi-blazor
```

## Vantaggi di questo approccio:

- ✅ Non serve il codice sorgente sul server
- ✅ Non serve l'SDK .NET sul server
- ✅ Build molto più veloce (solo copia file)
- ✅ Immagine Docker più piccola
- ✅ Maggiore sicurezza (no codice sorgente sul server)
- ✅ **Trasferimento semplificato con file ZIP**

## Note:

- Assicurati che il file `publish.zip` contenga tutti i file necessari
- Il file `gurobi.prod.lic` deve essere presente se usi Gurobi
- Tutte le dipendenze native sono incluse nei file pubblicati
- Il processo di unzip avviene automaticamente durante il build Docker

## Troubleshooting

### Errore: "The application 'Tesi.Blazor.Server.dll' does not exist"

**Causa**: Il file ZIP non contiene i file nella posizione corretta.

**Soluzione**:
1. Verifica che il publish sia avvenuto correttamente:
   ```bash
   ls -la ./publish/Tesi.Blazor.Server.dll
   ```

2. Verifica il contenuto del ZIP:
   ```bash
   # Windows PowerShell:
   Expand-Archive -Path ./publish.zip -DestinationPath ./test -Force
   Get-ChildItem ./test

   # Linux/Mac:
   unzip -l publish.zip | head -10
   ```

3. Il file `Tesi.Blazor.Server.dll` deve essere direttamente nella root del ZIP, non in una sottocartella.

4. Se necessario, ricrea il ZIP correttamente:
   ```bash
   # Windows PowerShell:
   Remove-Item ./publish.zip -Force
   Compress-Archive -Path ./publish/* -DestinationPath ./publish.zip

   # Linux/Mac:
   rm -f publish.zip
   cd publish && zip -r ../publish.zip . && cd ..
   ```

### Errore durante il build Docker

Se il build fallisce, puoi debuggare con:
```bash
# Build con output verboso
docker build -t tesi-blazor . --progress=plain

# Esegui un container temporaneo per ispezionare i file
docker run -it --rm mcr.microsoft.com/dotnet/aspnet:8.0 /bin/bash
```
