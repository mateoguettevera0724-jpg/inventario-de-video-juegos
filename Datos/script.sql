CREATE TABLE IF NOT EXISTS videojuegos (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    formato TEXT NOT NULL,
    titulo TEXT NOT NULL,
    plataforma TEXT NOT NULL,
    genero TEXT NOT NULL,
    anio INTEGER NOT NULL,
    precio_base REAL NOT NULL,
    stock INTEGER NOT NULL
);
