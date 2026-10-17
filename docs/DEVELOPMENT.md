# Entwicklung

## Voraussetzungen
- Windows 10/11, [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (die App ist WPF und läuft nur unter Windows)
- Optional für den Installer: [Inno Setup 6](https://jrsoftware.org/isinfo.php)

## Bauen und testen
```bash
dotnet build BarbellTracker.sln
dotnet test BarbellTracker.sln
dotnet test BarbellTracker.sln --collect:"XPlat Code Coverage" --results-directory TestResults
```
Die Tracking-Tests erzeugen kleine Testvideos mit OpenCV (ca. 15 s). Die Oberfläche selbst ist nicht automatisiert getestet.

## Architektur
Schichten (Abhängigkeiten zeigen nach oben, die Kernschichten kennen keine UI):

| Projekt | Aufgabe |
|---|---|
| `AbstractionCode` | `Vector2D`, `SignalSmoother` (Savitzky-Golay) |
| `DomainCode` | `TrackedInformation` (Positionen, FPS, Pixel/cm), `Session` |
| `ApplicationCode` | Event-System, `FileManager`, `JsonSessionRepository`, `JsonSettingsStore`, `AppPaths` |
| `Adapter` | Plugin- und UI-Adapter-Verwaltung, Modelle (`PhysicalMotion`, `Repetition`, `SetAnalysis`, CSV-Modelle) |
| `Services` | `PhysicalMotionCalculator` (Pixel → m, m/s, m/s²), `SetAnalyzer` (Wiederholungen + Metriken), CSV-Übersetzer, Cache |
| `Tracking` | `VideoTracker` (OpenCV, CSRT-Tracker): Video + Startrechteck → `TrackedInformation` |
| `Plugins` | Tracker-Plugin `JsonLoader`, Processing-Plugins (CSV-Dateien, Tabellen) |
| `WPF_DesktopClient` | Oberfläche (MVVM): Video & Analyse, Verlauf, Start (Plugins), Settings |

### Ablauf
```
Video/JSON  ──►  TrackedInformation  ──►  ExtracedVideoInfo (Event)
                                              │
                    ┌─────────────────────────┼───────────────────────┐
                    ▼                         ▼                       ▼
        Processing-Plugins            Seite „Video & Analyse“      (weitere Plugins)
        (CSV-Dateien, Tabellen)       PhysicalMotion → SetAnalysis
                                      → Tabelle + Charts
```
- Die Services cachen ihre Ergebnisse nach `TrackedInformation.Id`. Neue Daten brauchen deshalb eine neue Id (der `JsonLoader` vergibt sie pro Extraktion, `TrackedInformationExtensions.WithNewId` in der UI).
- Plugins melden sich im Konstruktor beim `PluginManager` an und reagieren auf `Activate/DeactivatePlugin`.

### Neue Tracking-Quelle
`ITrackerPlugin` implementieren, auf `StartExtractVideoInfo` mit passendem `PluginName` reagieren und ein `ExtracedVideoInfo` mit einem `TrackedInformation` (mit eindeutiger Id) feuern. Registrierung in `DependencyInjectionHelper` und `App.OnStartup`.

## Daten und Logs
`%AppData%\BarbellTracker`: `settings.json`, `sessions\*.json`, `exports\` (CSV der Plugins), `logs\barbelltracker-<Datum>.log` (14 Tage). Unerwartete Fehler werden von globalen Handlern (`App.xaml.cs`) geloggt und dem Nutzer angezeigt, die App läuft weiter.

## Release
1. Tag setzen und pushen: `git tag v1.0.0 && git push origin v1.0.0`
2. Die Pipeline (`.github/workflows/dotnet.yml`) baut, testet, veröffentlicht self-contained für `win-x64`, erzeugt `BarbellTracker-<version>-win-x64.zip` und den Installer `BarbellTracker-Setup-<version>.exe` (`installer/BarbellTracker.iss`) und hängt beides an das GitHub-Release.

Lokal:
```bash
dotnet publish src/BarbellTracker.WPF_DesktopClient -c Release -r win-x64 --self-contained -p:Version=1.0.0 -o artifacts/publish
iscc /DMyAppVersion=1.0.0 /DPublishDir=..\artifacts\publish installer\BarbellTracker.iss
```
Die Installer-Pipeline ist noch nie auf GitHub gelaufen; das Skript wurde nur geschrieben, nicht mit Inno Setup übersetzt. Der Publish-Ordner wurde lokal gebaut und gestartet (Fenster erscheint, Logdatei wird geschrieben, Version 1.0.0). Er ist ca. 250 MB groß, vor allem wegen der OpenCV-Bibliotheken.

## Messwerte
Tracking mit dem CSRT-Tracker (Entwicklungsrechner, synthetische MJPEG-Videos, Rechteck ca. 100–150 px):

| Video | Dauer des Videos | Rechenzeit | Durchsatz |
|---|---|---|---|
| 1280×720, 30 fps | 10 s | 10 s | ca. 30 Frames/s |
| 1920×1080, 60 fps | 10 s | 28 s | ca. 21 Frames/s |

Das Tracking läuft im Hintergrund mit Fortschrittsanzeige und kann abgebrochen werden. H.264-Videos von Handys (Dekodierung) sind nicht gemessen. Anzeigen eines Frames (Slider): ca. 50–100 ms.

## Testabdeckung (Zeilen, `XPlat Code Coverage`)
| Projekt | Abdeckung |
|---|---|
| DomainCode | 100 % |
| Services | 94 % |
| Tracking | 92 % |
| ApplicationCode | 87 % |
| Adapter | 84 % |
| AbstractionCode | 74 % |
| Plugins | 92 % (Pipeline-Tests JSON → Event-Kette → CSV/Tabellen) |
| WPF_DesktopClient | nicht getestet (UI) |
| **Gesamt (ohne UI)** | **87,5 %** |
