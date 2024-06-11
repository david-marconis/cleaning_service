CREATE TABLE assignments(
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	user TEXT NOT NULL,
	assigned_to TEXT NULL,
	created_time INTEGER NOT NULL,
	updated_time INTEGER NOT NULL
);
