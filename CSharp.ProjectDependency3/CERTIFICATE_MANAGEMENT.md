# Gestion des Certificats SSL avec mkcert

## Vue d'ensemble

Le système gère automatiquement les certificats SSL pour tous les services via mkcert. Les certificats sont :
- Générés automatiquement lors du premier démarrage
- Stockés dans `~/.aspire-certs/`
- Régénérés automatiquement si la configuration DNS change
- Peuvent être régénérés manuellement si nécessaire

## Configuration DNS

La configuration DNS est définie par un **record immutable** (`DnsConfiguration`) qui inclut :
- Domaine principal (`pd3i1.com`)
- Hosts pour chaque service (api, viz, orchestrator, worker-agents)
- Ports pour chaque service
- Paramètres de certificats

### Propriétés immutables (init-only)
- `Domain`, `OrchestratorHost`, `WorkerHostTemplate`, `DashboardHost`, `ApiHost`
- `Ports` (configuration des ports)
- `WorkerCount`, `EnableHttps`, `CertificatesDirectory`

### Propriétés mutables
- `CertPath`, `KeyPath` - Chemins des certificats (définis après génération)
- `VizHost` - Peut être configuré dynamiquement

## Détection automatique des changements

Le système utilise le **hash structurel du record** pour détecter les changements de configuration.

### Méthode de hashing
```csharp
public string CalculateConfigurationHash()
{
    // Crée une copie avec chemins de certificats à null pour hash cohérent
    var configForHashing = this with { CertPath = null, KeyPath = null };
    
    // Utilise GetHashCode du record (égalité structurelle)
    var hashCode = configForHashing.GetHashCode();
    
    return hashCode.ToString("X8");
}
```

**Avantages du record :**
- Hash automatique basé sur l'égalité structurelle
- Immutabilité garantie avec `init`
- Syntaxe `with` pour créer des copies modifiées
- Comparaison d'égalité automatique

### Changements détectés
Si un changement est détecté dans :
- Le nom de domaine
- Les noms d'hôtes
- Le nombre de workers
- Les ports
- Les paramètres de certificats

Les certificats seront automatiquement régénérés.

**Note :** Les propriétés `CertPath` et `KeyPath` sont exclues du hash car elles sont mutables et ne font pas partie de l'identité de la configuration.

## Fichiers de configuration

### Configuration sauvegardée
- **Emplacement** : `~/.aspire-certs/dns-config.json`
- **Contenu** : Configuration DNS complète en JSON
- **Utilisation** : Détection de changements de configuration

### Certificats SSL
- **Certificat** : `~/.aspire-certs/pd3i1.com.pem`
- **Clé privée** : `~/.aspire-certs/pd3i1.com-key.pem`

## Régénération manuelle

### Option 1 : Forcer via code

```csharp
var dnsConfig = builder.Configuration.GetSection("DnsConfiguration").Get<DnsConfiguration>() ?? new DnsConfiguration();
builder.EnsureSetup(dnsConfig, logger, forceCertificateRegeneration: true);
```

### Option 2 : Supprimer les fichiers

Supprimez les fichiers de certificats pour forcer leur régénération :

**Windows (PowerShell)**
```powershell
Remove-Item $env:USERPROFILE\.aspire-certs\pd3i1.com.pem
Remove-Item $env:USERPROFILE\.aspire-certs\pd3i1.com-key.pem
```

**Unix/Mac (Bash)**
```bash
rm ~/.aspire-certs/pd3i1.com.pem
rm ~/.aspire-certs/pd3i1.com-key.pem
```

### Option 3 : Supprimer la configuration

Supprimez le fichier de configuration pour forcer une complète régénération :

**Windows (PowerShell)**
```powershell
Remove-Item $env:USERPROFILE\.aspire-certs\dns-config.json
```

**Unix/Mac (Bash)**
```bash
rm ~/.aspire-certs/dns-config.json
```

## Immutabilité et Records

### Création d'une configuration modifiée

Utilisez la syntaxe `with` pour créer une copie avec des propriétés modifiées :

```csharp
var newConfig = originalConfig with 
{ 
    WorkerCount = 3,
    Ports = originalConfig.Ports with { WorkerBase = 6000 }
};
```

### Comparaison de configurations

```csharp
if (config1 == config2)  // Égalité structurelle automatique
{
    Console.WriteLine("Configurations identiques");
}

// Comparaison de hash
var hash1 = config1.CalculateConfigurationHash();
var hash2 = config2.CalculateConfigurationHash();
```

## Workflow de génération

1. **Premier démarrage**
   - Pas de configuration sauvegardée ? génération des certificats
   - Sauvegarde de la configuration actuelle (JSON)

2. **Démarrages suivants**
   - Chargement de la configuration sauvegardée
   - Comparaison du hash avec la configuration actuelle
   - Si différent ? régénération automatique
   - Si identique ? réutilisation des certificats existants

3. **Régénération forcée**
 - Suppression des anciens certificats
   - Génération de nouveaux certificats
   - Sauvegarde de la nouvelle configuration

## Logs

Le système log chaque étape :
- `"No saved configuration - certificates will be generated"` : Première génération
- `"Configuration unchanged - certificates are still valid"` : Réutilisation
- `"Configuration has changed - certificates need regeneration"` : Régénération automatique
- `"Force regeneration of SSL certificates..."` : Régénération forcée
- `"? Certificates generated: {path}"` : Succès
- `"Current hash: {hash}"` / `"Saved hash: {hash}"` : Détails du hash

## Troubleshooting

### Les certificats ne sont pas régénérés

1. Vérifiez que mkcert est installé :
   ```bash
   mkcert -version
   ```

2. Vérifiez que la configuration a changé :
   ```bash
   # Unix/Mac
   cat ~/.aspire-certs/dns-config.json
   
   # Windows
   type %USERPROFILE%\.aspire-certs\dns-config.json
 ``

3. Comparez les hash dans les logs

4. Forcez la régénération en supprimant `dns-config.json`

### Erreurs SSL dans le navigateur

1. Réinstallez le CA racine de mkcert :
   ```bash
   mkcert -install
   ```

2. Régénérez les certificats (voir options ci-dessus)

3. Redémarrez le navigateur

### Permissions insuffisantes

Sur Windows, vous devez exécuter en tant qu'administrateur pour :
- Installer le CA racine de mkcert
- Modifier le fichier hosts

Le système vous demandera automatiquement l'élévation si nécessaire.

## Architecture Technique

### Avantages du Record

- **Immutabilité** : Garantie par `init` - les propriétés ne peuvent être modifiées après construction
- **Hash structurel** : `GetHashCode()` généré automatiquement basé sur toutes les propriétés
- **Égalité par valeur** : Deux records avec les mêmes valeurs sont égaux (`==`)
- **Syntaxe `with`** : Création facile de copies modifiées
- **Thread-safety** : Immutabilité naturelle évite les race conditions

### Propriétés exclues du hash

`CertPath` et `KeyPath` sont mutables (`set`) et exclus du hash via :
```csharp
var configForHashing = this with { CertPath = null, KeyPath = null };
```

Cela garantit que le hash ne change pas quand les chemins de certificats sont définis.

## Sécurité

- Les certificats mkcert sont uniquement valides sur la machine locale
- Le CA racine est installé dans le magasin de certificats de l'utilisateur
- Les certificats ne sont **PAS** valides pour Internet
- Utilisez ces certificats **uniquement pour le développement local**
- L'immutabilité du record prévient les modifications accidentelles de configuration
