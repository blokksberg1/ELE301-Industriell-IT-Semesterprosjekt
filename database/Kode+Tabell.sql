-- auto-generated definition
CREATE TABLE Brukar
(
    brukar_id VARCHAR(20) PRIMARY KEY NOT NULL,
    fornavn VARCHAR(20) NOT NULL,
    etternavn VARCHAR(20) NOT NULL,
    epost VARCHAR(254) NOT NULL,
    kort_id VARCHAR(4) UNIQUE,
    pin VARCHAR(4) NOT NULL,
    gyldig_fra TIMESTAMP NULL,
    gyldig_til TIMESTAMP NULL
);


CREATE TABLE Kortleser
(
    kortlesar_nr VARCHAR(4) PRIMARY KEY NOT NULL,
    romnummer VARCHAR(20) NOT NULL
);


CREATE TABLE Adgangsforsøk
(
    forsøk_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    brukar_id VARCHAR(20) REFERENCES Brukar(brukar_id),
    kortlesar_nr VARCHAR(20) NOT NULL REFERENCES Kortleser(kortlesar_nr),
    kort_id VARCHAR(4) UNIQUE NOT NULL CHECK (kort_id ~ '^[0-9]{4}$'),
    tidspunkt TIMESTAMP NOT NULL,
    godkjent BOOLEAN NOT NULL
);

CREATE TABLE Alarm
(
    alarm_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY ,
    kortlesar_nr VARCHAR(4) NOT NULL REFERENCES Kortleser(kortlesar_nr),
    alarmtype VARCHAR(20) NOT NULL,
    mottatt_tidspunkt TIMESTAMP NOT NULL ,
    sist_brukt_kort_id VARCHAR(20)
);

INSERT INTO Kortleser(kortlesar_nr, romnummer)
VALUES
        ('0000', '0000'),
        ('1111', '0001'),
        ('2222', '0010'),
        ('3333', '0011');

-- Unexpected: LocalDataSource
