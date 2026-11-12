-- Køyr etter 01_tabeller.sql. Testdataa skal leggjast inn éin gong.
-- Desse seks kortlesarane er bevarte frå det opphavlege skriptet ditt.
-- Dei fire påkravde kort/PIN-para høyrer til Brukar, ikkje Kortleser.
INSERT INTO Kortleser (kortlesar_nr, romnummer)
VALUES
    ('0000', '0000'),
    ('1111', '0001'),
    ('2222', '0010'),
    ('3333', '0011'),
    ('0001', 'A101'),
    ('0002', 'A102');

-- Dei fire påkravde kort/PIN-para. Datoformatet er ÅÅÅÅ-MM-DD.
-- Tidspunkt 00:00:00 er starten på dagen, ikkje slutten.
-- Selma sitt kort er utgått ved testtidspunkta i november 2026.
INSERT INTO Brukar (brukar_id, fornavn, etternavn, epost, kort_id, pin, gyldig_fra, gyldig_til)
VALUES
    ('11947', 'Anders', 'Langevåg', '122945@outlook.com', '0000', '0000', '2025-08-01 00:00:00', '2030-08-01 00:00:00'),
    ('11416', 'Geir', 'Myklebust', '11416@outlook.com', '1111', '0001', '2024-01-01 00:00:00', '2029-01-01 00:00:00'),
    ('14823', 'Else', 'Dalseid', '14823@outlook.com', '2222', '0010', '2026-05-01 00:00:00', '2031-05-01 00:00:00'),
    ('10245', 'Selma', 'Hellir', '10245@outlook.com', '3333', '0011', '2021-03-01 00:00:00', '2026-03-01 00:00:00');

-- Tre manuelle testtilfelle: gyldig kort, utgått kort og ukjent kort.
-- Resultatet er skrive inn manuelt her; databasen validerer ikkje PIN automatisk.
-- forsøk_id blir generert automatisk og skal ikkje vere med i INSERT.
INSERT INTO Adgangsforsøk (brukar_id, kortlesar_nr, kort_id, tidspunkt, godkjent)
VALUES
    ('11947', '0001', '0000', '2026-11-04 10:00:00', TRUE),
    ('10245', '0001', '3333', '2026-11-04 10:05:00', FALSE),
    (NULL,    '0001', '9999', '2026-11-04 10:10:00', FALSE);

-- Fire sjølvstendige alarmeksempel for å teste alarmrapporten.
-- Desse er ikkje ein samanhengande simulering av adgangsloggen over.
-- Ved integrasjon skal SENTRAL registrere faktisk sist brukte kort ved kvar dør.
-- alarm_id blir generert automatisk. NULL blir skrive utan hermeteikn.
INSERT INTO Alarm (kortlesar_nr, alarmtype, mottatt_tidspunkt, sist_brukt_kort_id)
VALUES
    ('0001', 'Dør broten opp',     '2026-11-04 10:00:00', '0000'),
    ('0002', 'Dør open for lenge', '2026-11-04 11:00:00', '1111'),
    ('0001', 'Dør open for lenge', '2026-11-05 12:00:00', '2222'),
    ('0002', 'Dør broten opp',     '2026-11-05 13:00:00', NULL);
