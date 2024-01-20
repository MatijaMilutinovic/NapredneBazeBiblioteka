# napredne_baze_podataka-main

Ovo je objedinjeni projekat iz predmeta Napredne baze podataka koji obuhvata rad za prva 2 projekta. Koriscene baze podataka su Redis i Neo4j. Primarno se koristi Neo4j za skladistenje podataka modela Korisnika i Biblioteke, kao i za kreiranje potega pri uclanjenju korisnika u neku od biblioteka (Korisnik - UclanjenU - Biblioteka). Redis se koristi za chat preko pub/sub metode izmedju biblioteke i korisnika u nekom od slucajeva: korisnik nije vratio knjigu nakon dogovorenog vremena, korisnik nije platio clanarinu itd.. (ovo nije implementirano u okviru projekta, samo su hipoteticke situacije).

Generalni use-case kako bi se prosle sve funkcionalnosti je:
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
11) Slanje poruke preko chat-a
12) Primanje poruke preko chat-a

Comentar: chat