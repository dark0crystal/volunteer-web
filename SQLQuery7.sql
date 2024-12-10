
ALTER TABLE register
DROP CONSTRAINT [PK_register];

ALTER TABLE register
ADD CONSTRAINT PK_register PRIMARY KEY (id, email);