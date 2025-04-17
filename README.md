# Verslag – Zelflerende agent die obstakels ontwijkt

## Doel van de opdracht

Het doel van deze opdracht is om een zelflerende agent te maken die leert om bewegende obstakels te ontwijken door eroverheen te springen. Deze agent wordt getraind met behulp van reinforcement learning. Bij iedere episode beweegt een obstakel met een willekeurige snelheid richting de agent. De agent moet zelf leren wanneer hij moet springen om botsingen te vermijden.

Ik heb ervoor gekozen om deze uitbreiding te implementeren:

> **De agent wordt geconfronteerd met een rij van continu bewegende obstakels.**

---

### Werking

- De agent kan 2 acties uitvoeren: springen (`1`) of niets doen (`0`)
- Een obstakel vergroot constant in de hoogte dit is random
- De snelheid van het obstakel wordt aan het begin van elke episode willekeurig gekozen binnen een bepaald bereik

### Observatieruimte

De observatie wordt als een vector meegegeven aan het model en bevat:

- Y-positie van de agent (springstatus)
- X-positie van het obstakel
- Huidige snelheid van het obstakel

### Beloningsstructuur

| Situatie                      | Beloning |
| ----------------------------- | -------- |
| Obstakel geraakt              | -1       |
| Obstakel succesvol gesprongen | +1       |

---

### Analyse

- In het begin is de agent puur aan het gokken en krijgt hij vaak negatieve beloningen.
- Na enkele honderden episodes leert hij dat springen soms voordelig is.
- Rond de **1000–2000 episodes** stijgt de gemiddelde beloning duidelijk.
- Uiteindelijk leert de agent op een redelijk consistente manier obstakels te ontwijken..

---

## Evaluatie

Na de training is het model geëvalueerd op obstakels met nieuwe snelheden (binnen het trainingsbereik, maar met andere waardes dan tijdens training).

Het resultaat: de agent kon in meer dan 90% van de gevallen succesvol springen en vermijden, ook bij hogere obstakel-snelheden.

---

## Reflectie en uitbreidingen

De agent presteert goed, maar de omgeving blijft vrij eenvoudig. Mogelijke uitbreidingen zijn:

- Meerdere obstakels tegelijk (rijen met meerdere timings)

---
