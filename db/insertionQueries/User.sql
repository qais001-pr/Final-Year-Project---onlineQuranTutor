-- Student

INSERT INTO [Users] (name, email, password, gender, dateOfBirth, userType, country, city, timezone, profile,preferred_tutor,subjectid)
VALUES ('Ali Khan', 'ali.khan@example.com', 'pass123', 'male', '1990-05-12', 'student', 'Pakistan', 'Lahore', 'Asia/Karachi', NULL, 'male',1);

--  Tutor
INSERT INTO [Users] (name, email, password, gender, dateOfBirth, userType, country, city, timezone, profile)
VALUES ('Ahmer Khan', 'ahmerkhan123@gmail.com', '12345678', 'male', '1990-05-12', 'tutor', 'Pakistan', 'Lahore', 'Asia/Karachi', NULL);

--  Guardian
INSERT INTO [Users] (name, email, password, gender, dateOfBirth, userType, country, city, timezone, profile)
VALUES ('Haseeeb Khan', 'haseebkhan123@gmail.com', '12345678', 'male', '1990-05-12', 'guardian', 'Pakistan', 'Karachi', 'Asia/Karachi', NULL);


--  Child
INSERT INTO [Users] (name, email, password, gender, dateOfBirth, userType, country, city, timezone, profile)
VALUES ('Umer', 'umer123@gmail.com', '12345678', 'male', '2019-05-12', 'child', 'Pakistan', 'Karachi', 'Asia/Karachi', NULL);
insert into children (childId,guardianId) values (4,3)
