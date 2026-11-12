-- PostgreSQL: Køyr denne fila først i ein tom prosjektdatabase.
-- CREATE TABLE opprettar tabellane; fila er ikkje ei oppgradering av eksisterande tabellar.

-- Ein brukar har eitt kort. Tekst bevarer innleiande nullar i kort-ID og PIN.
CREATE TABLE Brukar
(
    brukar_id VARCHAR(20) PRIMARY KEY,
    fornavn VARCHAR(20) NOT NULL,
    etternavn VARCHAR(20) NOT NULL,
    epost VARCHAR(254) NOT NULL,
    kort_id VARCHAR(4) UNIQUE NOT NULL CHECK (kort_id ~ '^[0-9]{4}$'),
    -- Oppgåva krev fire teikn i PIN, ikkje nødvendigvis berre siffer.
    pin VARCHAR(4) NOT NULL CHECK (char_length(pin) = 4),
    -- NULL betyr inga grense. Utelat begge datoane for eit alltid gyldig kort.
    gyldig_fra TIMESTAMP,
    gyldig_til TIMESTAMP,
    CHECK (gyldig_fra IS NULL OR gyldig_til IS NULL OR gyldig_fra <= gyldig_til)
);

-- Alle kortlesarar ligg i same tabell, med ei rad per kortlesar.
CREATE TABLE Kortleser
(
    kortlesar_nr VARCHAR(4) PRIMARY KEY CHECK (kortlesar_nr ~ '^[0-9]{4}$'),
    romnummer VARCHAR(20) NOT NULL
);

-- Logg både godkjende og avviste forsøk. SENTRAL avgjer resultatet.
CREATE TABLE Adgangsforsøk
(
    forsøk_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    -- Ukjent kort gir NULL. Standard FK-regel hindrar sletting av refererte brukarar.
    brukar_id VARCHAR(20) REFERENCES Brukar(brukar_id),
    kortlesar_nr VARCHAR(4) NOT NULL REFERENCES Kortleser(kortlesar_nr),
    -- Ikkje UNIQUE: same kort kan brukast mange gonger.
    -- Ikkje FK til Brukar: ukjende kort skal òg kunne loggførast.
    kort_id VARCHAR(4) NOT NULL CHECK (kort_id ~ '^[0-9]{4}$'),
    tidspunkt TIMESTAMP NOT NULL,
    godkjent BOOLEAN NOT NULL
);

-- Alarmen viser mottakstidspunktet og kortet som sist blei brukt ved døra.
CREATE TABLE Alarm
(
    alarm_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    kortlesar_nr VARCHAR(4) NOT NULL REFERENCES Kortleser(kortlesar_nr),
    alarmtype VARCHAR(20) NOT NULL
        CHECK (alarmtype IN ('Dør broten opp', 'Dør open for lenge')),
    mottatt_tidspunkt TIMESTAMP NOT NULL,
    -- NULL dersom ingen kort er brukte enno. Kortet kan òg vere ukjent.
    sist_brukt_kort_id VARCHAR(4) CHECK (sist_brukt_kort_id ~ '^[0-9]{4}$')
);

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
