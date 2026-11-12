# SQL for adgangskontroll

1. Kople til ein tom PostgreSQL-prosjektdatabase.
2. Køyr `01_tabeller.sql` for å opprette tabellane.
3. Køyr `02_testdata.sql` éin gong for å leggje inn testdata.
4. `03_rapport-queries.sql` er førebels tom; dei fem rapportane står att.

`adgangskontroll_kommentert.sql` inneheld same tabellar og testdata i éi fil.
Bruk anten denne samla fila eller dei to nummererte filene, ikkje begge.
`original_datagrip.sql.bak` er ein kopi av koden før gjennomgangen.

NULL i gyldigheitsgrensene betyr inga grense. SENTRAL må handtere dette ved validering.
Testdataa bruker uttrykkelege tidspunkt, slik at testane ikkje er avhengige av dagens dato.
Alarmradene er separate rapporttestar, ikkje ein fullstendig hendingssekvens.

Før innlevering står mellom anna dette att:
- Dei fem rapportspørjingane, inkludert avklaring av kva «brukarnamn» betyr.
- Testar for feil PIN, framtidig gyldigheit, alltid gyldig og over ti avviste forsøk.
- Val av slettestrategi: framandnøklane hindrar no sletting av brukarar/kortlesarar med loggar.
  Avklar korleis administrasjonen skal slette/deaktivere utan å miste rapporthistorikken.
- UML-modell og dokumentasjon av kolonnar, reglar og testresultat.
- Integrasjon med SENTRAL, som skal bruke parameteriserte spørjingar.
