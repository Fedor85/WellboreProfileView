CREATE TABLE WELLBORE
(
   ID bigint NOT NULL IDENTITY (1, 1),
   CONSTRAINT PK_WELLBORE PRIMARY KEY (ID),
   WELL_ID bigint NOT NULL,
   NAME varchar(20) NOT NULL,
   CONSTRAINT UN_WELLBORE_WELLNAME UNIQUE(WELL_ID, NAME),
);

ALTER TABLE WELLBORE     
ADD CONSTRAINT FK_WELLBORE_WELL FOREIGN KEY (WELL_ID)     
    REFERENCES WELL (ID)     
    ON DELETE CASCADE    
    ON UPDATE CASCADE    