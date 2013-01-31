TntMPDConverter
===============

Dieses Programm erlaubt die Umwandlung der Projektabrechnung von Wycliff e.V. ins TntMPD-Format.

Voraussetzungen
---------------
Lauffähig unter Windows und Linux.
Microsoft .NET Framework 2 (oder neuer) oder Mono muss installiert sein.

Installation
------------
Einfach in ein beliebiges Verzeichnis entpacken.

Benutzung
---------
In TntMPDConvert kann die Projektabrechnung im Feld "Projektabrechnung" ausgewählt werden. 
Außerdem kann ein Zielverzeichnis ausgewählt werden, in dem die konvertierte Datei gespeichert
wird. 

Wenn im Programmverzeichnis eine Datei namens "replace.config" angelegt wird, können
Ersetzungen vorgenommen werden. 

Das Konvertierungsprogramm erstellt Buchungsnummern basierend auf dem Buchungsdatum und
Spendernummer, deshalb kann die selbe Projektabrechnung problemlos mehrfach konvertiert und
in TntMPD importiert werden.
