PRAGMA foreign_keys = ON;

CREATE TABLE assignment(
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	user TEXT NOT NULL,
	description TEXT NOT NULL,
	bid_id_assigned INTEGER NULL REFERENCES bids(id),
	created DateTime NOT NULL DEFAULT CURRENT_TIME,
	updated DateTime NOT NULL DEFAULT CURRENT_TIME
);

CREATE TABLE bid(
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	assignment_id INTEGER REFERENCES assignments(id) NOT NULL,
	cleaner TEXT NOT NULL,
	price NUMERIC NOT NULL,
	description TEXT NOT NULL
);

CREATE TABLE subscription(
	name TEXT PRIMARY KEY,
	web_hook TEXT
);
