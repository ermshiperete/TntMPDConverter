TntMPDConverter
===============

Dieses Programm erlaubt die Umwandlung der Projektabrechnung von Wycliff e.V. ins TntMPD-Format.

Voraussetzungen
---------------
Microsoft .NET Framework 2 (oder neuer) oder Mono muss installiert sein.

Installation
------------
Einfach in ein beliebiges Verzeichnis entpacken.

Benutzung
---------
Die Projektabrechnung muss zun�chst in Word ge�ffnet werden und als Plain-Text-Datei abgespeichert 
werden (Erweiterung .txt).
Dann kann TntMPDConvert aufgerufen werden und die gespeicherte Datei im Feld "Projektabrechnung"
ausgew�hlt werden. 
Au�erdem kann ein Zielverzeichnis ausgew�hlt werden, in dem die konvertierte Datei gespeichert
wird. 

Wenn im Programmverzeichnis eine Datei namens "replace.config" angelegt wird, k�nnen
Ersetzungen vorgenommen werden. 

Einschr�nkungen
---------------
TntMPD unterscheidet Buchungen beim Import anhand einer eindeutigen Nummer. Leider sind
in der Projektabrechnung von Wycliff jedoch keine Buchungsnummern enthalten, deshalb wird
fortlaufend gez�hlt. Die aktuelle Nummer wird in C:\Users\<Benutzer>\AppData\Local\TntMPDConverter\user.config
gespeichert (unter Windows XP: C:\Documents and Settings\<Benutzer>\...).

Wenn nun eine Projektabrechnung mehrfach konvertiert und importiert wird,
tauchen die Buchungen in TntMPD mehrfach auf.

Wenn die selben Buchungsnummern von TntMPDConvert mehrfach vergeben werden (etwa weil das 
Programm von einem anderen Computer aufgerufen wird) kann es vorkommen, dass TntMPD
alte Buchungen l�scht weil es eine Korrektur der Buchung zu Erkennen meint.

Um dies auszuschlie�en sollte nach einem Upgrade von TntMPDConvert oder Umzug auf einen 
neuen Computer �berpr�ft werden, dass die Buchungsnummern der neu konvertierten Datei 
an die Buchungsnummern der zuletzt konvertierten Datei anschliessen. Wenn dies nicht
der Fall ist, muss die Datei user.config (siehe oben) editiert werden und der Wert
f�r die Variable BookingId auf einen entsprechenden Wert gesetzt werden.

Auch sollte darauf geachtet werden, dass TntMPDConvert immer im selben Verzeichnis
installiert/aufgerufen wird.
