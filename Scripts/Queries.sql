SELECT * FROM Member;
SELECT * FROM Reader;
SELECT * FROM EndUser;
SELECT * FROM Librarian;
SELECT * FROM Book;
SELECT * FROM Borrow;

DELETE Member;
DELETE Reader;
DELETE EndUser;
DELETE Librarian;
DELETE Book;
DELETE Borrow;

DELETE FROM EndUser
WHERE ID = 'L88476329-EndUser';

INSERT INTO EndUser(ID, Username, Password)
VALUES ('L20258624K-EndUser', 'Lib1','Lib12024');

INSERT INTO Librarian(ID, EndUser)
VALUES ('L20.258.624-K', 'L20258624K-EndUser');

UPDATE Book
SET Available = 1
WHERE ID = 'Think&GrowRich-NapoleonHill-1965-Wealth-N°1';
		