
-- SQLite: Oppretter tabellene og legger inn testdata.
-- Skal kjøres én gang mot en tom database.

-- En bruker har ett kort.
-- TEXT bevarer innledende nuller i kort-ID og PIN.

CREATE TABLE Brukar
(
    brukar_id VARCHAR(20) PRIMARY KEY,
    fornavn VARCHAR(20) NOT NULL,
    etternavn VARCHAR(20) NOT NULL,
    epost VARCHAR(254) NOT NULL,

    kort_id VARCHAR(4) UNIQUE NOT NULL
        CHECK (length(kort_id) = 4 AND kort_id NOT GLOB '*[^0-9]*'),

    -- PIN skal inneholde fire tegn.
    pin VARCHAR(4) NOT NULL
        CHECK (length(pin) = 4),

    -- NULL betyr ingen gyldighetsgrense.
    gyldig_fra TEXT,
    gyldig_til TEXT,

    CHECK (gyldig_fra IS NULL OR gyldig_til IS NULL OR gyldig_fra <= gyldig_til)
);

-- Kortlesere

CREATE TABLE Kortleser
(
    kortlesar_nr VARCHAR(4) PRIMARY KEY
        CHECK (length(kortlesar_nr) = 4 AND kortlesar_nr NOT GLOB '*[^0-9]*'),

    romnummer VARCHAR(20) NOT NULL
);

-- Logger godkjente og avviste adgangsforsøk.
-- Samme kort kan loggføres flere ganger.

CREATE TABLE Adgangsforsøk
(
    forsøk_id INTEGER PRIMARY KEY AUTOINCREMENT,

    -- Ukjent kort gir NULL.
    brukar_id VARCHAR(20) REFERENCES Brukar(brukar_id),

    kortlesar_nr VARCHAR(4) NOT NULL
        REFERENCES Kortleser(kortlesar_nr),

    kort_id VARCHAR(4) NOT NULL
        CHECK (length(kort_id) = 4 AND kort_id NOT GLOB '*[^0-9]*'),

    tidspunkt TEXT NOT NULL,
    godkjent BOOLEAN NOT NULL
);

-- Alarmtabell

CREATE TABLE Alarm
(
    alarm_id INTEGER PRIMARY KEY AUTOINCREMENT,

    kortlesar_nr VARCHAR(4) NOT NULL
        REFERENCES Kortleser(kortlesar_nr),

    alarmtype VARCHAR(20) NOT NULL
        CHECK (alarmtype IN ('Dør broten opp', 'Dør open for lenge')),

    mottatt_tidspunkt TEXT NOT NULL,

    sist_brukt_kort_id VARCHAR(4)
        CHECK (
            sist_brukt_kort_id IS NULL OR
            (length(sist_brukt_kort_id) = 4
             AND sist_brukt_kort_id NOT GLOB '*[^0-9]*')
        )
);


-- ============================================
-- TESTDATA
-- ============================================

-- Kortlesere

INSERT INTO Kortleser (kortlesar_nr, romnummer)
VALUES
    ('0000', '0000'),
    ('1111', '0001'),
    ('2222', '0010'),
    ('3333', '0011'),
    ('0001', 'A101'),
    ('0002', 'A102');


-- Brukere
-- Datoformat: YYYY-MM-DD HH:MM:SS

INSERT INTO Brukar
(brukar_id, fornavn, etternavn, epost, kort_id, pin, gyldig_fra, gyldig_til)
VALUES
    ('11947', 'Anders', 'Langevåg', '122945@outlook.com', '0000', '0000',
     '2025-08-01 00:00:00', '2030-08-01 00:00:00'),

    ('11416', 'Geir', 'Myklebust', '11416@outlook.com', '1111', '0001',
     '2024-01-01 00:00:00', '2029-01-01 00:00:00'),

    ('14823', 'Else', 'Dalseid', '14823@outlook.com', '2222', '0010',
     '2026-05-01 00:00:00', '2031-05-01 00:00:00'),

    ('10245', 'Selma', 'Hellir', '10245@outlook.com', '3333', '0011',
     '2021-03-01 00:00:00', '2026-03-01 00:00:00');


-- Adgangsforsøk
-- Godkjent, utgått kort og ukjent kort.

INSERT INTO Adgangsforsøk
(brukar_id, kortlesar_nr, kort_id, tidspunkt, godkjent)
VALUES
    ('11947', '0001', '0000', '2026-11-04 10:00:00', TRUE),
    ('10245', '0001', '3333', '2026-11-04 10:05:00', FALSE),
    (NULL,    '0001', '9999', '2026-11-04 10:10:00', FALSE);


-- Alarmer

INSERT INTO Alarm
(kortlesar_nr, alarmtype, mottatt_tidspunkt, sist_brukt_kort_id)
VALUES
    ('0001', 'Dør broten opp',     '2026-11-04 10:00:00', '0000'),
    ('0002', 'Dør open for lenge', '2026-11-04 11:00:00', '1111'),
    ('0001', 'Dør open for lenge', '2026-11-05 12:00:00', '2222'),
    ('0002', 'Dør broten opp',     '2026-11-05 13:00:00', NULL);
