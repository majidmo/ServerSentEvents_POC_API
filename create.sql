CREATE TABLE Notifications (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    IsRead BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Insert sample notifications for UserId = 1
INSERT INTO Notifications (UserId, Message, IsRead, CreatedAt)
VALUES 
(1, 'You have a new message.', 0, GETDATE()),
(1, 'Your order has been shipped.', 0, GETDATE()),
(1, 'Reminder: Your appointment is tomorrow.', 0, GETDATE());

-- Insert sample notifications for UserId = 2
INSERT INTO Notifications (UserId, Message, IsRead, CreatedAt)
VALUES 
(2, 'Your password was changed successfully.', 0, GETDATE()),
(2, 'Welcome to the app!', 0, GETDATE());

-- Insert sample notifications for UserId = 3
INSERT INTO Notifications (UserId, Message, IsRead, CreatedAt)
VALUES 
(3, 'You have a new friend request.', 0, GETDATE()),
(3, 'Your subscription is about to expire.', 0, GETDATE());

-- Insert some read notifications (for testing filtering)
INSERT INTO Notifications (UserId, Message, IsRead, CreatedAt)
VALUES 
(1, 'Old message (read).', 1, DATEADD(DAY, -5, GETDATE())),
(2, 'Another old message (read).', 1, DATEADD(DAY, -10, GETDATE()));
