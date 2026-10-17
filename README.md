# Programmentwurf für Advanced Software-Engineering von Jan Metzger und Florian Koch
## An der DHBW Karlsruhe im Studiengang medizinische Informatik für das 5 & 6 Semester

### Inhalt
<ul>
  <li>Entwurfsmuster</li>
  <li>Test Daten</li>
  <li>Programm Code</li>
  <li>Unit Tests</li>
  <li>Mutation Testergebnisse</li>
  <li>Die schriftliche Ausarbeitung</li>
</ul>

## Download Akutelle Version
Die aktuelle Version kann man im Github Repo herunterladen, alternativ auch über diesen Link (https://github.com/Hawk1401/BarbellTracker/releases).
Dort gibt es einen Installer (`BarbellTracker-Setup-<version>.exe`, ohne Administratorrechte) und ein ZIP zum Entpacken. Windows 10/11 (x64), .NET muss nicht installiert sein.

Zum Selbstbauen, Testen und Veröffentlichen siehe [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md).

## Nutzung
Die App (.NET 8, Windows) hat vier Bereiche:

**Video** – der Hauptweg
1. *Video öffnen* und mit dem Slider einen Frame wählen, in dem die Hantel startet.
2. *Hantelscheibe markieren*: Rechteck um die Scheibe ziehen.
3. *Maßstab markieren*: eine Linie über eine bekannte Strecke ziehen (z. B. Scheibendurchmesser, 45 cm) und die Länge in cm eingeben.
4. *Tracking starten*. Danach erscheinen Wiederholungen mit Dauer, Bewegungsumfang, mittlerer/maximaler/propulsiver Geschwindigkeit, seitlicher Abweichung und Geschwindigkeitsverlust, dazu der Geschwindigkeitsverlauf und die Hantelbahn.
5. Optional Übung, Gewicht und Notizen eintragen und *Session speichern*.

**Verlauf** – gespeicherte Sessions öffnen, löschen, als CSV oder JSON exportieren und die beste Geschwindigkeit je Übung über die Zeit vergleichen.

**Start** – die Plugins (CSV-Dateien, Tabellen) aktivieren und Positionsdaten aus einer JSON-Datei laden (Beispiele in `Example Data`, z. B. `squat_set.json`).

**Settings** – Glättung, Mindestbewegung für eine Wiederholung und Ausgabeordner der CSV-Dateien.

### Wo liegen die Daten?
In `%AppData%\BarbellTracker`: `settings.json`, `sessions\` (eine JSON-Datei je Session) und `exports\` (CSV-Dateien der aktivierten Plugins, Standard-Ausgabeordner).

### Probleme?
Unerwartete Fehler zeigt die App in einem Dialog an und schreibt Details nach `%AppData%\BarbellTracker\logs` (Settings → „Log-Ordner öffnen“). Tipps für gutes Tracking: Kamera fest auf einem Stativ, Hantelscheibe gut sichtbar und von der Seite gefilmt, Rechteck eng um die Scheibe. Verliert das Tracking die Scheibe, meldet die App den Frame – dann einen früheren Startframe oder ein größeres Rechteck wählen.

### Kommandozeile
```
BarbellTracker.ConsoleClient <tracking.json>
BarbellTracker.ConsoleClient track <video> <x> <y> <breite> <hoehe> <pixelProCm>
```
