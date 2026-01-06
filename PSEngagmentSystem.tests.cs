using System;
using System.IO;
using System.Linq;
using Xunit;
using PSEngagementSystem;

namespace PSEngagementSystem.Tests
{
    public class RepositoryTests
    {
        private static string CreateTempDbPath()
        {
            string fileName = $"ps_test_{Guid.NewGuid():N}.db";
            return Path.Combine(Path.GetTempPath(), fileName);
        }

        [Fact]
        public void CheckInRepository_AddCheckIn_ThenGetCheckIns_ReturnsSavedCheckInWithComment()
        {
            // Arrange
            string dbPath = CreateTempDbPath();
            CheckInRepository.ConnectionString = $"Data Source={dbPath}";
            CheckInRepository.InitializeDatabase();

            // Act
            CheckInRepository.AddCheckIn(studentId: 1, mood: 4, comment: "Feeling a bit stressed");
            var all = CheckInRepository.GetCheckIns();

            // Assert
            Assert.NotEmpty(all);

            var latest = all.OrderByDescending(c => c.ID).First();
            Assert.Equal(1, latest.StudentID);
            Assert.Equal(4, latest.Mood);
            Assert.Equal("Feeling a bit stressed", latest.Comment);

           
        }

        [Fact]
        public void MeetingRepository_AddMeeting_DefaultStatusIsRequested()
        {
            // Arrange
            string dbPath = CreateTempDbPath();
            MeetingRepository.ConnectionString = $"Data Source={dbPath}";
            MeetingRepository.InitializeDatabase();

            var dt = DateTime.Parse("2026-01-06 10:30");

            // Act
            MeetingRepository.AddMeeting(studentId: 1, supervisorId: 1, scheduledTime: dt, requestedBy: "Student");
            var meetings = MeetingRepository.GetMeetingsByStudent(1);

            // Assert
            Assert.NotEmpty(meetings);

            var latest = meetings.OrderByDescending(m => m.ID).First();
            Assert.Equal(1, latest.StudentID);
            Assert.Equal(1, latest.SupervisorID);
            Assert.Equal(MeetingStatus.Requested, latest.Status);

        }

        [Fact]
        public void MeetingRepository_UpdateMeetingStatus_ChangesStatusPersistently()
        {
            // Arrange
            string dbPath = CreateTempDbPath();
            MeetingRepository.ConnectionString = $"Data Source={dbPath}";
            MeetingRepository.InitializeDatabase();

            var dt = DateTime.Parse("2026-01-06 11:00");
            MeetingRepository.AddMeeting(studentId: 2, supervisorId: 1, scheduledTime: dt, requestedBy: "Student");

            var created = MeetingRepository.GetMeetingsByStudent(2)
                .OrderByDescending(m => m.ID)
                .First();

            // Act
            MeetingRepository.UpdateMeetingStatus(created.ID, MeetingStatus.Confirmed);

            var after = MeetingRepository.GetMeetingsByStudent(2)
                .First(m => m.ID == created.ID);

            // Assert
            Assert.Equal(MeetingStatus.Confirmed, after.Status);

           
        }
    }
}
