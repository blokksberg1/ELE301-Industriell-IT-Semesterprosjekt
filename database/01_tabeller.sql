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
