# napredne_baze_podataka-main

Ovo je objedinjeni projekat iz predmeta Napredne baze podataka koji obuhvata rad za prva 2 projekta. Koriscene baze podataka su Redis i Neo4j. Primarno se koristi Neo4j za skladistenje podataka modela Korisnika i Biblioteke, kao i za kreiranje potega pri uclanjenju korisnika u neku od biblioteka (Korisnik - UclanjenU - Biblioteka). Redis se koristi za chat preko pub/sub metode izmedju biblioteke i korisnika u nekom od slucajeva: korisnik nije vratio knjigu nakon dogovorenog vremena, korisnik nije platio clanarinu itd.. (ovo nije implementirano u okviru projekta, samo su hipoteticke situacije).

Generalni use-case kako bi se prosle sve funkcionalnosti je sledeci.

Testiranje generalnih CRUD operacija, i rad Neo4j-a:
1) Registracija 2 Korisnika
2) Registracija 2 biblioteke
3) Login korisnika
4) Promene sifre korisnika
5) Brisanje korisnika
6) Login biblioteke
7) Promena sifre biblioteke
8) Brisanje biblioteke
9) Login korisnika
10) Uclanjenje u biblioteku

Testiranje chat funkcionalnosti i rad Redis-a:
1) Login korisnika u jednom browseru i navigacija do chat-a
2) Login biblioteke u novom incognito prozoru i navigacija do chat-a
3) Medjusobna komunikacija na globalnom chat-u (poruke stizu svim strana koje su trenutno u chat-u)
4) Slanje privatnih poruka medjusobno
5) Pogled na privatne poruke preko button-a "Poslednja privatna poruka"