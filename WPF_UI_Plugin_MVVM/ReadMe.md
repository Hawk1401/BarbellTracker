# GUI Implementierung 

Für die GUI wird WPF mit dem MVVM Pattern genutz. Hierbei wird aber lediglich View und ViewModel direkt in dieser Projektmappe genutzt. 
Diese zwei Modelle ergeben dann ein Plugin für die gesamte Applikation. 
Das Model des MVVM Patter wird von anderen Schichten in der Clean Architecture übernommen. 
So wollen wir das Framework von unserer Applikation loslösen und die Abhängigkeiten reduzieren.. 