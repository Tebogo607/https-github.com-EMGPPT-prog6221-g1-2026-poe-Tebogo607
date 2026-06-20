

USE PoePart3;

CREATE TABLE IF NOT EXISTS Tasks (
    TaskId        INT AUTO_INCREMENT PRIMARY KEY,
    Title         VARCHAR(255)    NOT NULL,
    Description   VARCHAR(1000)   NOT NULL,
    IsComplete    BOOLEAN         NOT NULL DEFAULT FALSE,
    HasReminder   BOOLEAN         NOT NULL DEFAULT FALSE,
    ReminderDate  DATETIME        NULL,
    CreatedAt     DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS ActivityLog (
    LogId         INT AUTO_INCREMENT PRIMARY KEY,
    Description   VARCHAR(500)    NOT NULL,
    LoggedAt      DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


SELECT 'Tasks table ready' AS Status;
SELECT 'ActivityLog table ready' AS Status;
