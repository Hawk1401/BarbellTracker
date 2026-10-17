# BarbellTracker – Roadmap zu Version 1.0

## 1. Ist-Zustand

**Was funktioniert**
- Saubere Schichtenarchitektur (Abstraction → Domain → Application → Adapter → Services → Plugins → WPF-Client), Event-System, Plugin-Manager, DI.
- Eine Tracker-Quelle: `JsonLoader` liest **fertige** Positionsdaten (`Example Data/example.json`).
- Berechnung von Geschwindigkeit und Beschleunigung (`VelocityCalculator`, `AccelerationCalculator`) inkl. Cache.
- Ausgabe als CSV-Datei (in `%TEMP%\BarbellTracker`) und als Tabelle im WPF-Tab.
- Unit-Tests (Abstraction, Domain, Adapter, Services), CI mit Release-Build (self-contained, win-x64).

**Was fehlt bzw. unfertig ist**
- Der eigentliche Kern: **Es gibt keine Hantel-Erkennung/Tracking aus Video.** Die App kann nur vorgefertigte JSON-Positionen einlesen. Der Video-Tab (`AdapterVideoPlayerViewModel`) ist leer/ungenutzt, `UIVideoAdapter` hat nur eine Property.
- Keine echten physikalischen Einheiten: Geschwindigkeit = Positionsdifferenz pro Frame in Pixeln. `FrameRate` und `PixelPerCm` werden mitgeführt, aber nicht verwendet.
- Keine Trainingsmetriken (Wiederholungen, Peak-/Mean-Velocity, Range of Motion, Bar-Path, Satz-Auswertung).
- Keine Diagramme – nur CSV-Tabelle.
- Settings-Seite ist leer (`SettingsControlViewModel` ohne Inhalt).
- Keine Persistenz/Historie: Daten liegen nur im Temp-Ordner, keine Sessions, kein Export-Dialog.
- Kein Fehler-Feedback für den Nutzer (`JsonLoader` verschluckt Exceptions).

**Bekannte technische Schulden**
| Stelle | Problem |
|---|---|
| `JsonLoader.StartTracking` | `FileSelected`-Handler wurde nie wieder abgemeldet (reagierte auf jede spätere Dateiauswahl); `async void`; leerer `catch` |
| `FileManager.CreateFileIfNotExist` | `File.Create` wird nicht geschlossen (Handle-Leak, Append kann fehlschlagen) |
| `TrackedInformation.Equals/GetHashCode` | nur über `Id` → Cache-Kollisionen bei verschiedenen Dateien mit gleicher Id (Beispiel-Id: `someId`) |
| `TrackedInformation.PixelPerCm` | `int` – zu grob, Skalierung braucht `double` |
| `AccelerationCalculator.Description`, `AccelerationCSVTranslater.Name/Description` | `throw new NotImplementedException()` |
| `BooleanToVisibilityConverter.ConvertBack` | wirft `NotImplementedException` |
| `ConsoleClient` | hartkodierter Pfad (`C:\Users\schal\Desktop\...`), `while(true);` Busy-Loop |
| `EventSystem.Fire` | liest `Map` ohne Lock (Race mit `Subscribe`) |
| `MainWindow` | Titel „MainWindow", feste Größe 800×450, Fenster nicht responsiv |
| Repo | `.mp4` im WPF-Projekt, Mutation-HTML-Reports und PDF eingecheckt |
| CI | erzeugt bei **jedem** Push ein Release mit UUID-Tag; `ApplicationCode_Tester` hat keine Tests; Plugins/WPF ungetestet |
| Framework | .NET 6 ist End-of-Life → auf .NET 8 (LTS) heben |

---

## 2. Definition von „1.0"

> Ein Nutzer filmt einen Satz, öffnet das Video in der App, markiert die Hantel, und bekommt Wiederholungen mit Geschwindigkeit (m/s), Bar-Path und ROM angezeigt, kann die Session speichern, wieder laden und exportieren.

Muss-Kriterien:
1. Video-Import und Hantel-Tracking (halbautomatisch genügt).
2. Korrekte Einheiten (cm, m/s, m/s²).
3. Wiederholungserkennung + Kernmetriken je Wiederholung/Satz.
4. Visualisierung (Video + Overlay, Geschwindigkeits-Chart, Bar-Path).
5. Persistente Sessions + CSV-Export.
6. Stabil: Fehlerbehandlung, Tests, Installer/Release-Pipeline.

---

## 3. Meilensteine

### M0 – Fundament aufräumen (klein, ~1 Wochenende)
- [x] Upgrade auf .NET 8, NuGet-Pakete aktualisieren, Build+Tests grün.
- [x] Bugs aus der Schuldentabelle beheben (JsonLoader-Handler bleibt nach dem Laden abonniert, File-Handle, Cache-Key, NotImplemented, EventSystem-Lock + Snapshot beim Dispatch).
- [x] `PixelPerCm` → `double`. (JSON-`Version` folgt in M2.)
- [x] ConsoleClient entweder entfernen oder per Argument (`--file`) nutzbar machen.
- [x] CI: Release nur bei Tag/`v*`, Tests für **alle** Projekte, `dotnet test` über die Solution.
- [x] Repo ausmisten (mp4 entfernt, Mutation-Reports/PDF/Entwurfsmuster → `docs/`), `.editorconfig`.
- [x] Neue Tests für `EventSystem` und `FileManager` (ApplicationCode hatte keine).

### M1 – Korrekte Physik & Metriken (Domain/Services)
- [x] Umrechnung: `v = Δs[px] / PixelPerCm / 100 · FPS` → m/s; Beschleunigung analog; Y-Achse (Bild unten = +) korrekt invertieren.
- [x] Glättung (Savitzky-Golay, `SignalSmoother`) der Positionsdaten (z. B. Savitzky-Golay oder gleitender Mittelwert), da Pixel-Tracking rauscht.
- [x] Wiederholungserkennung (in `SetAnalyzer`, Zigzag-Wendepunkte mit Mindest-ROM): Wiederholungen anhand Bewegungsumkehr (konzentrisch/exzentrisch) erkennen.
- [x] Metriken je Wiederholung: Mean-/Peak-Velocity, Mean Propulsive Velocity, ROM, Dauer, Bar-Path-Abweichung; je Satz: Velocity-Loss in %.
- [x] Unit-Tests mit synthetischen Bewegungen. (Mutation-Test-Lauf steht noch aus.)
- [x] Processing-Plugin `SetMetricsToCSVFile` (schreibt `SetMetrics.CSV`). `MetricsToAdapterTable` kommt mit der Tabellenansicht in M3.
- Beispieldaten: `Example Data/squat_set.json` (3 Wiederholungen, Geschwindigkeitsverlust).

### M2 – Video-Tracking (der größte Brocken)
- [x] Entscheidung zur Technik: OpenCvSharp4 + CSRT-Tracker (Spike erfolgreich, Test mit synthetischem Video inkl. Analyse) (Vorschlag: **OpenCvSharp** mit CSRT/KCF-Tracker; Alternativen: ML-Modell für Hantelscheiben, später).
- [x] Kern: `BarbellTracker.Tracking.VideoTracker` (Video + ROI → `TrackedInformation`, FPS aus Video, Tracking-Verlust wird als `LostAtFrame` gemeldet).
- [ ] `ITrackerPlugin` `OpenCvTracker` + Events: Video wählen → ROI im 1. Frame (braucht UI, M3) → Tracking → `ExtracedVideoInfo`.
- [x] ConsoleClient: `track <video> <x> <y> <w> <h> <pixelPerCm>` erzeugt `*.tracking.json` und startet die Auswertung.
- [x] Kalibrierung: `VideoTracker.CalculatePixelPerCm(pixel, cm)`. [ ] UI zum Markieren der Strecke.
- [x] Fortschritt (`IProgress`) + Abbruch (`CancellationToken`). [ ] Anbindung an die UI (Hintergrund-Task).
- [ ] Manuelle Korrektur: Tracking-Verlust erkennen, Frame zurück, ROI neu setzen.
- [ ] `JsonLoader` bleibt als Import-/Test-Plugin erhalten.

### M3 – UI/UX
- [x] Video-Seite `Video & Analyse`: Video öffnen, Frame-Scrubbing, Rechteck um die Hantelscheibe, Maßstab-Linie + Länge in cm, Tracking mit Fortschritt/Abbrechen, Overlay (Bahn + aktuelle Position). [ ] Play/Pause fehlt noch (nur Scrubbing).
- [x] Charts (OxyPlot): Geschwindigkeit über Zeit mit markierten Wiederholungen, Hantelbahn. [ ] Beschleunigungs-Chart optional.
- [x] Workflow als Seite mit Hinweistext pro Schritt (kein eigener Wizard). Ergebnistabelle mit Metriken je Wiederholung; zeigt auch JSON-Importe.
- [x] Settings-Seite: Glättungsfenster und Mindestbewegung (Ergebnis wird sofort neu berechnet). [ ] Ausgabeordner, Einheiten, Sprache, Theme; Einstellungen noch nicht gespeichert (M4).
- [x] Fehlermeldungen in der Statusleiste, Fenstertitel, größeres Fenster. [ ] Icon, Plugins (CSV-Export) standardmäßig aktivieren?
- [ ] Lokalisierung (DE/EN) über Resource-Dateien. (UI ist aktuell deutsch.)
- [x] Bugfix: Seitenumschaltung im Hauptfenster (die Sichtbarkeits-Bindung traf das falsche DataContext).

### M4 – Persistenz & Export
- [x] Sessions dauerhaft in `%AppData%\BarbellTracker\sessions` (JSON-Ordner statt SQLite): Übung, Gewicht, Datum, Notizen, Positionsdaten + Kurzergebnis. Auch der CSV-Ausgabeordner liegt jetzt dort (`exports`).
- [x] Verlauf-Seite: Sessions auflisten, filtern, öffnen (Doppelklick), löschen; Chart der besten Geschwindigkeit je Übung.
- [x] Export: CSV (Wiederholungsmetriken) und JSON (wieder importierbar). [ ] PDF/PNG-Report optional.
- [x] Einstellungen persistieren (`settings.json`: Glättung, Mindestbewegung, Ausgabeordner).

### M5 – Qualität & Release
- [ ] Test-Coverage: Application-Layer, Plugins, ViewModels (ohne UI) abdecken; Ziel ≥ 80 % im Domain/Services-Bereich.
- [ ] Integrationstest: JSON → Event-Kette → CSV-Output (Golden-File).
- [ ] Logging (Serilog) in Datei, globaler Exception-Handler.
- [ ] Performance: Tracking großer Videos (60 fps, 1080p) messen; Frames asynchron verarbeiten.
- [ ] Installer (MSIX oder Inno Setup) + Versionierung (SemVer, `v1.0.0`-Tag → GitHub Release).
- [ ] Nutzerdokumentation mit Screenshots, Beispielvideo, Lizenz (LICENSE) wählen.
- [ ] Manuelle Testrunde mit echten Trainingsvideos (Bankdrücken, Kniebeuge, Kreuzheben), Genauigkeit gegen bekannte Referenz prüfen.

---

## 4. Empfohlene Reihenfolge
`M0 → M1 → M2 → M3 → M4 → M5`

M0 und M1 sind klein und liefern sofort echten Mehrwert (korrekte Werte aus den vorhandenen JSON-Daten). M2 ist das Risiko-Projekt – am besten früh mit einem Spike (1 Video, OpenCV-Tracker, Konsolenausgabe) validieren, bevor die UI darauf aufgebaut wird. M3–M4 können teilweise parallel laufen.

## 5. Offene Entscheidungen
1. **Plattform:** Bei WPF bleiben (Windows-only) oder später MAUI/Avalonia für Mobile? (Training passiert am Handy – langfristig relevant, aber nicht für 1.0.)
2. **Tracking-Ansatz:** Manuelle ROI + klassischer Tracker (schnell) vs. ML-Erkennung der Hantelscheibe (robuster, aufwändiger).
3. **Persistenz:** SQLite vs. JSON-Dateien.
4. **Lizenz** des Projekts.
