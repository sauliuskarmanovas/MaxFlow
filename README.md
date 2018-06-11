# MaxFlow

## Programos naudojimo instrukcijos:

Programa paleidžiant per komandinę eilutę / terminalą galima nurodyti tokias papildomas komandas:

*	Jokios papildomos komandos – grafo įvedimas bus per komandinę eilutę sekant ekrane spausdinamas instrukcijas

*	-file [FailoPavadinimas] ¬– grafas bus skaitomas iš failo. Pirmas skaičius faile yra viršūnių skaičius n, sekantys n*n skaičiai reiškia svorius. Jie atskirti tarpais, naujom eilutėm ar tabuliacija. Lankų matrica, kur eilutės numeris – iš kurios viršūnės lankas išeiną,  o stulpelio numeris – į kurią ateina.

*	-v [ViršūniųSkaičius] – grafas sugeneruojamas atsitiktinai su nurodytų kiekiu viršūnių (jei nurodytą < 3 pasirenkamas atsitiktinis skaičius intervale [3, 100]). Lankų skaičius atsitiktinis.

*	-e [LankųSkaičius] – atsitiktinis grafas su nurodytu skaičiu lankų (lankų skaičiaus m leistinas intervalas priklauso nuo viršūnių skaičiaus n, t.y m ∈ [n-1, (n-1)*(n-1)*2]). Pastaba nenurodžius viršūnių skaičias jis parenkamas antsitiktinai, todėl briaunų skaičius gali būti irgi sugeneruotas atsitiktinai.
