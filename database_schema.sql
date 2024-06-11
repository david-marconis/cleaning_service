PRAGMA foreign_keys = ON;

CREATE TABLE assignments(
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	user TEXT NOT NULL,
	description TEXT NOT NULL,
	assigned_to TEXT NULL,
	created DateTime NOT NULL DEFAULT CURRENT_TIME,
	updated DateTime NOT NULL DEFAULT CURRENT_TIME
);

CREATE TABLE bids(
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	assignment_id INTEGER REFERENCES assignments(id) NOT NULL,
	cleaner TEXT NOT NULL,
	price NUMERIC NOT NULL,
	description TEXT NOT NULL
);

CREATE TABLE subscriptions(
	name TEXT PRIMARY KEY,
	web_hook TEXT
);
