CREATE TABLE Users (
    Id INTEGER PRIMARY KEY,
    Username TEXT NOT NULL,
    Email TEXT NOT NULL,
    PasswordHash TEXT NOT NULL
);

CREATE TABLE Roles (
    Id INTEGER PRIMARY KEY,
    Name TEXT NOT NULL,
    Description TEXT
);

CREATE TABLE UserRoles (
    UserId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users (Id),
    FOREIGN KEY (RoleId) REFERENCES Roles (Id)
);
CREATE TABLE RefreshTokens (
    Id INTEGER PRIMARY KEY,       
    Token TEXT NOT NULL,          
    Expires DATETIME NOT NULL,    
    UserId INTEGER NOT NULL,      
    FOREIGN KEY (UserId) REFERENCES Users (Id)  
        ON DELETE CASCADE
);
