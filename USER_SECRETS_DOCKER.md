# User Secrets con Docker

## Opzione 1: Mount dei User Secrets (Raccomandato per sviluppo)

```bash
# Build dell'immagine
docker build -t tesi-blazor .

# Run con mount dei secrets locali
docker run -p 8080:8080 \
  -v ~/.microsoft/usersecrets:/root/.microsoft/usersecrets:ro \
  tesi-blazor
```

Su Windows PowerShell:
```powershell
# Build dell'immagine
docker build -t tesi-blazor .

# Run con mount dei secrets locali
docker run -p 8080:8080 \
  -v "$env:APPDATA\Microsoft\UserSecrets:/root/.microsoft/usersecrets:ro" \
  tesi-blazor
```

## Opzione 2: Variabili d'ambiente (Raccomandato per produzione)

Invece di usare user secrets in produzione, usa variabili d'ambiente:

```bash
docker run -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=Server=..." \
  -e "ApiKeys__SomeService=your-api-key" \
  tesi-blazor
```

## Opzione 3: Secrets file nel progetto (Solo per sviluppo)

Se vuoi includere i secrets direttamente nell'immagine (NON raccomandato per produzione):

1. Esporta i tuoi user secrets in un file:
```bash
dotnet user-secrets list --project Tesi.Blazor/Tesi.Blazor.Server > Tesi.Blazor/Tesi.Blazor.Server/secrets.json
```

2. Modifica il Dockerfile per copiare il file secrets.json

**ATTENZIONE**: Non committare MAI il file secrets.json nel repository!

## Sicurezza

- Gli user secrets sono pensati SOLO per lo sviluppo locale
- In produzione, usa sempre variabili d'ambiente o un sistema di gestione dei segreti come Azure Key Vault, AWS Secrets Manager, etc.
- Non includere mai segreti direttamente nell'immagine Docker per la produzione
