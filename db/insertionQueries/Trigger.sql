CREATE TRIGGER Add_Slots_After_Inserting_User
ON dbo.Users
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Slots AS (
        SELECT 1 AS SlotID
        UNION ALL
        SELECT SlotID + 1 FROM Slots WHERE SlotID < 24
    ),
    Days AS (
        SELECT 1 AS DayID
        UNION ALL
        SELECT DayID + 1 FROM Days WHERE DayID < 7
    )

    -- =========================
    -- Student / Child Slots
    -- =========================
    INSERT INTO dbo.StudentSlots (SlotID, UserID, DaysID, Status)
    SELECT
        s.SlotID,
        i.UserID,
        d.DayID,
        'available'
    FROM inserted i
    CROSS JOIN Days d
    CROSS JOIN Slots s
    WHERE i.UserType IN ('Student', 'Child');

    -- =========================
    -- Tutor Slots
    -- =========================
    INSERT INTO dbo.TutorSlots (SlotID, UserID, DayID, Status)
    SELECT
        s.SlotID,
        i.UserID,
        d.DayID,
        'available'
    FROM inserted i
    CROSS JOIN Days d
    CROSS JOIN Slots s
    WHERE i.UserType = 'Tutor'

    OPTION (MAXRECURSION 100);
END;
GO