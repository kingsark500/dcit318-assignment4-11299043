CREATE DATABASE MedicalDB;
GO
USE MedicalDB;
GO

CREATE TABLE dbo.Doctors (
    DoctorID      INT IDENTITY(1,1) PRIMARY KEY,
    FullName      VARCHAR(100) NOT NULL,
    Specialty     VARCHAR(100) NOT NULL,
    Availability  BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.Patients (
    PatientID  INT IDENTITY(1,1) PRIMARY KEY,
    FullName   VARCHAR(100) NOT NULL,
    Email      VARCHAR(200) NOT NULL UNIQUE
);

CREATE TABLE dbo.Appointments (
    AppointmentID   INT IDENTITY(1,1) PRIMARY KEY,
    DoctorID        INT NOT NULL,
    PatientID       INT NOT NULL,
    AppointmentDate DATETIME NOT NULL,
    Notes           VARCHAR(500) NULL,
    CONSTRAINT FK_Appointments_Doctors FOREIGN KEY (DoctorID) REFERENCES dbo.Doctors(DoctorID),
    CONSTRAINT FK_Appointments_Patients FOREIGN KEY (PatientID) REFERENCES dbo.Patients(PatientID)
);

CREATE INDEX IX_Appointments_DoctorDate ON dbo.Appointments(DoctorID, AppointmentDate);

-- Seed data
INSERT INTO dbo.Doctors (FullName, Specialty, Availability) VALUES
('Dr. Ama Boateng', 'Cardiology', 1),
('Dr. Kwesi Mensah', 'Dermatology', 1),
('Dr. Efua Owusu', 'Pediatrics', 0);

INSERT INTO dbo.Patients (FullName, Email) VALUES
('Kofi Adjei', 'kofi@example.com'),
('Akosua Nyarko', 'akosua@example.com'),
('Yaw Owusu', 'yaw@example.com');
