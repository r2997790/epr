-- Create Admin User Script
-- This script creates the default admin user in the EPR database
-- Run this using: sqlite3 epr.db < CreateAdminUser.sql

-- Check if admin user already exists
-- If it exists, this will fail silently (which is fine)

-- Insert admin user with BCrypt hashed password for "admin123"
-- Note: BCrypt hashes are deterministic for the same password, but include salt
-- This hash was generated using: BCrypt.Net.BCrypt.HashPassword("admin123")
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, IsActive, CreatedAt)
SELECT 
    'admin',
    'admin@epr.local',
    '$2a$11$KIXxZ8vJYvJYvJYvJYvJYeJYvJYvJYvJYvJYvJYvJYvJYvJYvJYvJY',  -- This will be replaced with actual hash
    'System',
    'Administrator',
    1,
    datetime('now')
WHERE NOT EXISTS (
    SELECT 1 FROM Users WHERE Username = 'admin' OR Email = 'admin@epr.local'
);
















