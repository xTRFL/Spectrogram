# Spectrogram
Projekt zrealizowany na 3 roku studiów w ramach przedmiotu: "Cyfrowe przetwarzanie sygnałów"

## 1. Wstęp
Założeniem projektu było wygenerowanie spektrogramu oraz umożliwienie nakładania na niego okien oraz zmienianie stopnia nakładania się na siebie kolejnych próbek.

Do zrealizowania projektu wykorzystałem biblioteki:
- NAudio 
- FFTSharp
- Spectrogram

Program napisałem w języku C#, a do interfejsu skorzystałem z frameworku Windows Forms.

Wykorzystana biblioteka NAudio korzysta z dostępnych systemowo kodeków audio, przez co do analizy niektórych plików WAV może być potrzebne pobranie odpowiednich narzędzi.
Szczególnie dotyczy to plików z kompresją. 

## 2. Dostępne opcje
### Wczytanie pliku
Najpierw należy wczytać plik za pomocą przycisku Load File.

Od razu wygeneruje się spektrogram o standardowych parametrach: intensity: 1, overlap: 50, type: narrow, window: default.
Spektrogram można zapisać w każdej chwili przyciskiem Save File.

### Parametry
- Select window: pozwala wybrać kilka z dostępnych okien; okna służą do uwydatnienia niektórych elementów spektrogramu, co umożliwia lepszą jego analizę.
- Overlap: stopień nachodzenia na siebie kolejnych próbek, reprezentowany przez wartości 1-100 bedące procentową liczbą nałożenia.
- Intensity: jasność spektrogramu, każda kolejna próbka jest pomnożona przez tę wartość, dzięki czemu spektrogram staje się bardziej czytelny.
- Type: Pozwala wygenerować spektrogram wąskopasmowy lub szerokopasmowy. 
Różnią się one analizowanymi częstotliwościami, dzięki czemu uzyskuje się albo lepszą rozdzielczość częstotliwościową, albo lepszą rozdzielczość czasową.
