SELECT * FROM Member;
SELECT * FROM Reader;
SELECT * FROM EndUser;
SELECT * FROM Librarian;

DELETE Reader;
DELETE EndUser;
DELETE Member;

INSERT INTO EndUser(ID, Username, Password)
VALUES ('L20258624K-EndUser', 'Lib1','Lib12024');

INSERT INTO Librarian(ID, EndUser)
VALUES ('L20.258.624-K', 'L20258624K-EndUser');
		