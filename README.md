# GitHub Actions Preview - BlackJack

Dieses Projekt dient als kleines Beispiel, um GitHub Actions an einer echten .NET/WPF-Anwendung zu erklären. Die Anwendung heißt `BlackJack`, dazu gibt es ein Testprojekt `BlackJack.Tests`.

Die wichtigsten Dateien für GitHub Actions liegen im Ordner `.github/workflows/`. Jede YAML-Datei beschreibt einen Workflow, also eine automatische Aufgabe, die GitHub auf einem sogenannten Runner ausführt.

## Projektstruktur

```text
.
|-- .github/workflows/
|   |-- dotnet-ci.yml
|   |-- dotnet-desktop.yml
|   `-- github-actions-demo.yml
|-- BlackJack/
|   `-- BlackJack.csproj
|-- BlackJack.Tests/
|   |-- BlackJack.Tests.csproj
|   `-- UserTests.cs
`-- BlackJack.slnx
```

## Was ist ein Workflow?

Ein Workflow ist eine Automatisierung in GitHub Actions. Er wird durch ein Ereignis gestartet, zum Beispiel durch einen `push` oder einen `pull_request`.

In diesem Projekt bedeutet das:

1. Jemand pusht Code nach GitHub.
2. GitHub startet automatisch einen Workflow.
3. Ein frischer Runner wird vorbereitet.
4. Der Quellcode wird heruntergeladen.
5. .NET wird installiert.
6. Die NuGet-Pakete werden wiederhergestellt.
7. Die Anwendung wird gebaut.
8. Die Tests werden ausgeführt.
9. Ergebnisse oder Build-Dateien werden als Artefakte gespeichert.

## Workflow 1: dotnet-ci.yml

`dotnet-ci.yml` ist die wichtigste CI-Pipeline. CI steht für Continuous Integration. Die Idee ist: Jede Codeänderung soll automatisch geprüft werden.

Der Workflow startet bei:

```yaml
on:
  push:
    branches:
      - main
      - master
  pull_request:
    branches:
      - main
      - master
```

Das bedeutet: Wenn Code auf `main` oder `master` gepusht wird, startet die Pipeline. Dasselbe passiert, wenn ein Pull Request gegen diese Branches erstellt wird.

### Job: Build

Der Build-Job prüft, ob die Lösung überhaupt kompiliert.

```yaml
run: dotnet build BlackJack.slnx --configuration Release --no-restore
```

Projektspezifisch heißt das: GitHub Actions baut die WPF-Anwendung aus `BlackJack/BlackJack.csproj` und das Testprojekt aus `BlackJack.Tests/BlackJack.Tests.csproj`. Wenn zum Beispiel eine Klasse umbenannt wird, aber ein Test noch den alten Namen verwendet, würde dieser Job fehlschlagen.

### Job: Automated Tests

Der Test-Job startet erst nach einem erfolgreichen Build:

```yaml
needs: build
```

Dadurch wird keine Zeit mit Tests verschwendet, wenn der Code nicht einmal kompiliert.

Die Tests werden mit diesem Befehl ausgeführt:

```yaml
run: dotnet test BlackJack.slnx --configuration Release --no-restore --logger "trx;LogFileName=test-results.trx"
```

In diesem Projekt prüft `BlackJack.Tests/UserTests.cs` zum Beispiel:

- Ein neuer `User` startet mit `Balance = 1000`.
- `Username`, `PasswordHash` und `Balance` können korrekt gesetzt werden.
- Gewinn oder Verlust verändert den Kontostand richtig.

Beispiel: Wenn jemand in `BlackJack/Modelle/User.cs` den Startwert von `Balance` von `1000` auf `500` ändert, dann schlägt der Test `NewUser_ShouldStartWithDefaultValues` fehl. GitHub Actions zeigt dann im Pull Request, dass die Änderung eine erwartete Regel gebrochen hat.

### Job: Code Quality

Dieser Job baut die Lösung erneut, behandelt Warnungen aber als Fehler:

```yaml
run: >
  dotnet build BlackJack.slnx
  --configuration Release
  --no-restore
  -warnaserror
  -p:EnableNETAnalyzers=true
```

Das ist hilfreich, weil Warnungen oft auf spätere Fehler hinweisen. Fuer ein Lernprojekt sieht man hier gut: GitHub Actions kann nicht nur testen, sondern auch Qualitätsregeln erzwingen.

### Job: Formatting Check

Der Formatierungs-Job prüft, ob der Code nach den .NET-Regeln formatiert ist:

```yaml
run: dotnet format BlackJack.slnx --verify-no-changes --verbosity diagnostic
```

Wichtig: `--verify-no-changes` verändert keine Dateien. Der Workflow prüft nur, ob etwas formatiert werden müsste.

### Job: Pull Request Validation

Dieser Job läuft nur bei Pull Requests:

```yaml
if: github.event_name == 'pull_request'
```

Er wartet auf alle vorherigen Jobs:

```yaml
needs:
  - build
  - test
  - code-quality
  - formatting
```

Damit ist er ein guter Abschluss für die Erklaerung: Ein Pull Request gilt erst als validiert, wenn Build, Tests, Codequalität und Formatierung erfolgreich waren.

## Workflow 2: dotnet-desktop.yml

`dotnet-desktop.yml` zeigt eine Desktop-Build-Pipeline für die WPF-Anwendung.

Sie läuft bei Pushes und Pull Requests auf:

- `main`
- `develop`

Dieser Workflow hat nur einen Job, aber mehrere Schritte.

### Checkout

```yaml
uses: actions/checkout@v4
```

Ohne diesen Schritt wäre der Code nicht auf dem Runner vorhanden. Der Runner startet leer und muss das Repository zuerst auschecken.

### Setup .NET

```yaml
uses: actions/setup-dotnet@v4
with:
  dotnet-version: 10.0.x
```

Dieser Schritt installiert das passende .NET SDK. Das Projekt verwendet `net10.0-windows`, deshalb muss auch der Workflow ein passendes SDK verwenden.

### Cache NuGet

```yaml
uses: actions/cache@v4
with:
  path: ~/.nuget/packages
  key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
```

NuGet-Pakete wie `Microsoft.EntityFrameworkCore` müssen nicht bei jedem Lauf komplett neu heruntergeladen werden. Der Cache macht spätere Builds schneller.

### Restore, Build, Test

```yaml
run: dotnet restore
run: dotnet build --configuration Release --no-restore
run: dotnet test --configuration Release --no-build
```

Diese drei Schritte bilden den Kern fast jeder .NET-Pipeline:

- `restore` lädt Abhängigkeiten.
- `build` kompiliert den Code.
- `test` führt automatisierte Tests aus.

### Publish und Artifact

```yaml
run: dotnet publish --configuration Release --output publish
```

Danach wird der Ordner `publish/` als Artefakt hochgeladen:

```yaml
uses: actions/upload-artifact@v4
with:
  name: WPF-Build
  path: publish/
```

Projektspezifisch bedeutet das: Nach einem erfolgreichen Lauf kann man in GitHub Actions das Build-Ergebnis der BlackJack-WPF-Anwendung herunterladen.

## Workflow 3: github-actions-demo.yml

Dieser Workflow ist ein sehr einfacher Demo-Workflow. Er eignet sich gut für den Einstieg, weil er nur Informationen im Log ausgibt.

Beispiele:

```yaml
echo "The job was automatically triggered by a ${{ github.event_name }} event."
echo "This job is now running on a ${{ runner.os }} server hosted by GitHub!"
echo "The name of your branch is ${{ github.ref }}"
```

Damit kann man erklären, dass GitHub Actions sogenannte Kontexte bereitstellt:

- `github.event_name`: Welches Ereignis hat den Workflow gestartet?
- `runner.os`: Auf welchem Betriebssystem läuft der Job?
- `github.ref`: Welcher Branch oder Tag wurde verwendet?
- `github.repository`: In welchem Repository läuft der Workflow?
- `job.status`: Wie ist der aktuelle Job-Status?

## Konkretes Beispiel aus diesem Projekt

Angenommen, jemand ändert in `BlackJack/Modelle/User.cs` die Standard-Balance:

```csharp
public decimal Balance { get; set; } = 500;
```

Danach wird die Änderung gepusht.

Was passiert in GitHub Actions?

1. Der Workflow `dotnet-ci.yml` startet automatisch.
2. Der Runner `windows-latest` wird vorbereitet.
3. `actions/checkout` holt den Code.
4. `actions/setup-dotnet` installiert .NET 10.
5. `dotnet restore` lädt die NuGet-Pakete.
6. `dotnet build` baut die Lösung.
7. `dotnet test` führt `UserTests.cs` aus.
8. Der Test `NewUser_ShouldStartWithDefaultValues` erwartet weiterhin `1000`.
9. Der Test schlägt fehl, weil der Code jetzt `500` liefert.
10. GitHub Actions markiert den Workflow als fehlgeschlagen.

So sieht man sehr einfach, wozu CI gut ist: Fehler werden nicht erst beim manuellen Testen entdeckt, sondern automatisch direkt nach dem Push.

## Wichtige GitHub-Actions-Begriffe

| Begriff | Bedeutung in diesem Projekt |
| --- | --- |
| Workflow | Eine YAML-Datei im Ordner `.github/workflows/` |
| Event | Auslöser wie `push` oder `pull_request` |
| Job | Eine größere Aufgabe, z. B. `build` oder `test` |
| Step | Ein einzelner Schritt innerhalb eines Jobs |
| Runner | Virtuelle Maschine, auf der der Workflow läuft |
| Action | Wiederverwendbarer Baustein, z. B. `actions/checkout` |
| Artifact | Datei oder Ordner, der nach dem Workflow heruntergeladen werden kann |
| needs | Legt fest, dass ein Job auf andere Jobs warten muss |
| if | Bedingung, ob ein Job oder Step ausgeführt wird |

## Lokale Befehle zum Vergleichen

Die GitHub-Actions-Schritte kann man lokal nachstellen:

```powershell
dotnet restore BlackJack.slnx
dotnet build BlackJack.slnx --configuration Release
dotnet test BlackJack.slnx --configuration Release
```

Der Unterschied ist: Lokal führt man diese Befehle selbst aus. In GitHub Actions laufen sie automatisch bei jedem passenden Ereignis.
