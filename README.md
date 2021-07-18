# programowanie_wielowątkowe

## O projekcie

Projekt stworzony podczas zajęć z Programowania Wielowątkowego (Informatyka, SGGW) wykorzystujący znajomość <b>semaforów</b>, napisany w <b>C#</b>.

Zadanie polega na rozwiązaniu sytuacji przypominającej problem ucztujących filozofów (https://pl.wikipedia.org/wiki/Problem_ucztuj%C4%85cych_filozof%C3%B3w). Istnieją jednak dwie modyfikacje:
1) 'Filozofowie' przy jedzeniu czytają. Mają do dyspozycji k książek (zaimplementowane jako zamki lub semafory) i każdy przed rozpoczęciem jedzenia bierze jedną książkę - za każdym razem inną.
2) 'Filozofowie' za każdym razem mogą usiąść przy innym miejscu. Widelce biorą zawsze z sąsiedztwa wybranego nakrycia, ale miejsce wybierają losowo.

<b>Zapętlony ciąg działań czytelnika:</b>
* Czytelnik 1 myśli...
* Czytelnik 1 zgłodniał.
* Czytelnik 1 usiadł przy nakryciu 2.             <- będzie próbował wziąć widelce z tego nakrycia; samo nakrycie jest wylosowane
* Czytelnik 1 chwycił za książkę 5.               <- książka wylosowana z k książek
* Czytelnik 1 zaczyna jeść i czytać książkę 5...
* Czytelnik 1 skończył jeść i czytać książkę 5.
* Czytelnik 1 odszedł od nakrycia 2.

## Opis zastosowanej synchronizacji:

Synchronizacja przebiega dzięki użyciu kilku semaforów.
1) SemaphoreSlim[] widelce
2) SemaphoreSlim stol
3) SemaphoreSlim[] ksiazki
4) SemaphoreSlim[] nakrycia

Semafory widelce, ksiazki i nakrycia to tablice semaforów binarnych.

* Dzięki semaforom widelce każdy czytelnik ma kontrolowany dostęp do widelców z jego nakrycia. Jeśli jego widelce są w użyciu przez sąsiadów, semafory te powodują, że czytelnik czeka na swoją kolej, póki zasoby te nie wrócą.
* Dzięki semaforowi stol nie dojdzie do zakleszczenia w synchronizacji. Powoduje on, że przy stole może być maksymalnie N-1 czytelników, dzięki czemu nie ma sytuacji, że N czytelników na raz chwyci za lewy widelec.
* Semafory ksiazki powodują, że każdy czytelnik losuje książkę i zanim po nią sięgnie, upewni się, że nie jest ona w użyciu przez kogoś innego. W takim przypadku czytelnik zacznie losować kolejną książkę, aż do skutku.
* Dzięki semaforom nakrycia zanim czytelnik usiądzie w jakimś wylosowanym miejscu, najpierw upewni się, że może tam usiąść, tj. że nikogo tam nie ma. Jeśli ktoś tam jest, następuje ponowne losowanie miejsca, aż do skutku.


### Przyjęte w programie liczby (możliwe do zmiany):
- czytelnicy: 10
- książki: 30
