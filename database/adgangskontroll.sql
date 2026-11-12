-- auto-generated definition
CREATE TABLE Bruker
(
    bruker_id VARCHAR(20) PRIMARY KEY NOT NULL,
    fornavn VARCHAR(20) NOT NULL,
    etternavn VARCHAR(20) NOT NULL,
    epost VARCHAR(254) NOT NULL,
    kort_id VARCHAR(4) UNIQUE NOT NULL
        CHECK (length(kort_id) = 4 AND kort_id NOT GLOB '*[^0-9]*'),
    pin VARCHAR(4) NOT NULL,
    gyldig_fra TEXT NULL,
    gyldig_til TEXT NULL
);


CREATE TABLE Kortleser
(
    kortleser_nr VARCHAR(4) PRIMARY KEY NOT NULL,
    romnummer VARCHAR(20) NOT NULL
);


CREATE TABLE Adgangsforsøk
(
    forsøk_id INTEGER PRIMARY KEY AUTOINCREMENT,
    bruker_id VARCHAR(20) REFERENCES Bruker(bruker_id),
    kortleser_nr VARCHAR(4) NOT NULL REFERENCES Kortleser(kortleser_nr),
    kort_id VARCHAR(4) NOT NULL
        CHECK (length(kort_id) = 4 AND kort_id NOT GLOB '*[^0-9]*'),
    tidspunkt TEXT NOT NULL,
    godkjent BOOLEAN NOT NULL,
    begrunnelse TEXT
);


CREATE TABLE Alarm
(
    alarm_id INTEGER PRIMARY KEY AUTOINCREMENT,
    kortleser_nr VARCHAR(4) NOT NULL REFERENCES Kortleser(kortleser_nr),
    alarmtype VARCHAR(20) NOT NULL,
    mottatt_tidspunkt TEXT NOT NULL,
    sist_brukt_kort_id VARCHAR(4)
);


INSERT INTO Kortleser(kortleser_nr, romnummer)
VALUES
        ('0000', '0000'),
        ('1111', '0001'),
        ('2222', '0010'),
        ('3333', '0011');


-- legger til dette for å utfylle eksempel databasen vår

INSERT INTO Bruker
(bruker_id, fornavn, etternavn, epost, kort_id, pin, gyldig_fra, gyldig_til)
VALUES
('1', 'Anna', 'Hansen', 'anna@example.com', '0000', '0000', NULL, NULL),
('2', 'Jonas', 'Olsen', 'jonas@example.com', '1111', '0001', NULL, NULL),
('3', 'Sara', 'Nilsen', 'sara@example.com', '2222', '0010', NULL, NULL),
('4', 'Emil', 'Berg', 'emil@example.com', '3333', '0011', NULL, NULL);