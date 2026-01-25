-- User
CREATE TABLE [Users] (
    userID INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    email VARCHAR(200) NOT NULL,
    password VARCHAR(255) NULL,
    gender VARCHAR(20) NULL, 
    dateOfBirth DATE NULL,
    userType VARCHAR(50) NULL,
    country VARCHAR(100) NULL,
    city VARCHAR(100) NULL,
    timezone VARCHAR(100) NULL,
    profile varchar(100) null,
    about varchar(400) null,
    preferred_tutor varchar(20) null,
    subjectid int null,
    foreign key (subjectid) references subjects(subjectid)
);



--children 

create table children(
childrenID int primary Key identity(1,1),
childId int not null,
guardianID int not null,
foreign key (childID) references users(userid),
foreign key (guardianID) references users(userid)
)


-- Subject
CREATE TABLE Subjects (
    subjectID INT IDENTITY(1,1) PRIMARY KEY,
    subjectName VARCHAR(200) NOT NULL
);
-- Days
CREATE TABLE Days (
    dayID INT IDENTITY(1,1) PRIMARY KEY,
    dayName VARCHAR(20) NOT NULL
);

-- Slots
CREATE TABLE Slots (
    slotID INT IDENTITY(1,1) PRIMARY KEY,
    startTime TIME NOT NULL,
    endTime TIME NOT NULL,
    createdAt DATETIME DEFAULT GETDATE()
);

-- Certificate
CREATE TABLE Certificate (
    certificateID INT IDENTITY(1,1) PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    issued_by_department VARCHAR(200),
    image VARBINARY(MAX),
    imageType VARCHAR(20),
    userid INT NOT NULL,
    createdAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Certificate_Tutor FOREIGN KEY (USERID) REFERENCES users(USERID)
);

-- TutorSlots
CREATE TABLE TutorSlots (
    tutorSlotID INT IDENTITY(1,1) PRIMARY KEY,
    userid INT NOT NULL,
    slotID INT NOT NULL,
    dayID INT NOT NULL,
    status VARCHAR(20) DEFAULT 'available',
    CONSTRAINT FK_TutorSlots_Tutor FOREIGN KEY (userid) REFERENCES users(userid),
    CONSTRAINT FK_TutorSlots_Slot FOREIGN KEY (slotID) REFERENCES Slots(slotID),
    CONSTRAINT FK_TutorSlots_Day FOREIGN KEY (dayID) REFERENCES Days(dayID)
);

-- StudentSlots
CREATE TABLE StudentSlots (
    studentSlotID INT IDENTITY(1,1) PRIMARY KEY,
    slotID INT NOT NULL,
    userid INT NOT NULL,
    daysID INT NOT NULL,
    status VARCHAR(20) DEFAULT 'available',
    CONSTRAINT FK_StudentSlots_Slot FOREIGN KEY (slotID) REFERENCES Slots(slotID),
    CONSTRAINT FK_StudentSlots_Student FOREIGN KEY (userid) REFERENCES users(userid),
    CONSTRAINT FK_StudentSlots_Days FOREIGN KEY (daysID) REFERENCES Days(dayID)
);

-- TutorSubjects
CREATE TABLE TutorSubjects (
    tutorSubjectID INT IDENTITY(1,1) PRIMARY KEY,
    userid INT NOT NULL,
    subjectID INT NOT NULL,
    CONSTRAINT FK_TutorSubjects_Tutor FOREIGN KEY (userid) REFERENCES users(userid),
    CONSTRAINT FK_TutorSubjects_Subject FOREIGN KEY (subjectID) REFERENCES Subjects(subjectID)
);

-- StudentTutorRequests
CREATE TABLE StudentTutorRequests (
    RequestID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    TutorID INT NOT NULL,
    SubjectID INT NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'pending',
    createdAt DATETIME DEFAULT GETDATE(),
    updatedAt DATETIME DEFAULT GETDATE(),
    Surahid int not null,
    CONSTRAINT FK_Request_Student FOREIGN KEY (StudentID) REFERENCES users(userid),
    CONSTRAINT FK_Tutor_Request FOREIGN KEY (TutorID) REFERENCES users(userid),
    CONSTRAINT FK_Request_Subject FOREIGN KEY (SubjectID) REFERENCES Subjects(subjectID),
    CONSTRAINT FK_SurahID FOREIGN KEY (Surahid) REFERENCES surahs(id)
);


-- Surahs
CREATE TABLE Surahs (
    SurahsID INT IDENTITY(1,1) PRIMARY KEY,
    SurahUrduName NVARCHAR(200) NOT NULL,
    SurahEngName NVARCHAR(200) NOT NULL
);

-- Quran
CREATE TABLE Quran (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    VerseID INT NOT NULL,
    AyatText NVARCHAR(MAX) NOT NULL,
    SurahID INT NOT NULL,
    CONSTRAINT FK_Verses_Surah FOREIGN KEY (SurahID) REFERENCES Surahs(SurahsID)
);

-- LessonPlan
CREATE TABLE LessonPlan (
    lessonPlanID INT IDENTITY(1,1) PRIMARY KEY,
    lessonName VARCHAR(200) NOT NULL
);

-- Lessons
CREATE TABLE Lessons (
    LessonsID INT IDENTITY(1,1) PRIMARY KEY,
    LessonPlanID INT NOT NULL,
    QuranID INT NOT NULL,
    CONSTRAINT FK_Lessons_LessonPlan FOREIGN KEY (LessonPlanID) REFERENCES LessonPlan(lessonPlanID),
    CONSTRAINT FK_Lessons_Quran FOREIGN KEY (QuranID) REFERENCES Quran(ID)
);

-- Classes
CREATE TABLE Classes (
    ClassID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    TutorID INT NOT NULL,
    SlotID INT NOT NULL,
    SubjectID INT NOT NULL,
    LessonPlanID INT NOT NULL,
    StudentRequestTutorID INT NOT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT 'Scheduled',
    Corrections VARCHAR(MAX) NULL,
    ClassDate DATE NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Classes_Student FOREIGN KEY (StudentID) REFERENCES users(userid),
    CONSTRAINT FK_Classes_Tutor FOREIGN KEY (TutorID) REFERENCES users(userid),
    CONSTRAINT FK_Classes_Slot FOREIGN KEY (SlotID) REFERENCES Slots(slotID),
    CONSTRAINT FK_Classes_Subject FOREIGN KEY (SubjectID) REFERENCES Subjects(subjectID),
    CONSTRAINT FK_Classes_LessonPlan FOREIGN KEY (LessonPlanID) REFERENCES LessonPlan(lessonPlanID),
    CONSTRAINT FK_Classes_StudentRequestTutor FOREIGN KEY (StudentRequestTutorID) REFERENCES StudentTutorRequests(RequestID)
);

-- Reviews
CREATE TABLE Reviews (
    ReviewID INT IDENTITY(1,1) PRIMARY KEY,
    ClassID INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment VARCHAR(MAX) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Reviews_Class FOREIGN KEY (ClassID) REFERENCES Classes(ClassID)
);
